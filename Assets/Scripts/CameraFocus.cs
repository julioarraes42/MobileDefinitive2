using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minOrbitDistance = 2f;
    public float maxOrbitDistance = 50f;

    public float orbitDistance = 10f;
    public float orbitSpeed = 5f;
    public float verticalLimit = 80f;
    public GameObject miraCanvas;

    private Transform currentPlanet;
    private Transform orbitAnchor;
    private bool isFocusing = false;
    private GameObject planetCanvas;
    private Transform originalCanvasParent;

    private Vector2 orbitAngles = new Vector2(30f, 0f); // pitch, yaw
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    private PlayerMovement playerMovement; // Referência ao script de movimento
    private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerMovement = playerObj.GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogWarning("PlayerMovement component not found on Player.");
            }
        }
        else
        {
            Debug.LogWarning("Player object with tag 'Player' not found.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFocusing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag("Planet"))
            {
                StartFocus(hit.collider.transform);
            }
        }

        if (isFocusing && orbitAnchor != null)
        {
            HandleOrbitInput();

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                orbitDistance -= scroll * zoomSpeed;
                orbitDistance = Mathf.Clamp(orbitDistance, minOrbitDistance, maxOrbitDistance);
            }

            Vector3 offset = orbitAnchor.rotation * new Vector3(0, 0, -orbitDistance);
            transform.position = orbitAnchor.position + offset;
            transform.LookAt(orbitAnchor.position);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopFocus();
            }
        }

    }

    void HandleOrbitInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * orbitSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * orbitSpeed;

        orbitAngles.x -= mouseY;
        orbitAngles.y += mouseX;

        orbitAngles.x = Mathf.Clamp(orbitAngles.x, -verticalLimit, verticalLimit);

        orbitAnchor.rotation = Quaternion.Euler(orbitAngles.x, orbitAngles.y, 0f);
    }

    void StartFocus(Transform planet)
    {
        currentPlanet = planet;

        planetCanvas = currentPlanet.GetComponentInChildren<Canvas>(true)?.gameObject;
        if (planetCanvas != null)
            planetCanvas.SetActive(true);

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        orbitAnchor = new GameObject("OrbitAnchor").transform;
        orbitAnchor.position = GetPlanetCenter(planet);
        orbitAnchor.SetParent(planet);

        transform.SetParent(null);
        Vector3 offset = orbitAnchor.rotation * new Vector3(0, 0, -orbitDistance);
        transform.position = orbitAnchor.position + offset;
        transform.LookAt(orbitAnchor.position);

        isFocusing = true;

        // Desativa o PlayerMovement
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (miraCanvas != null)
            miraCanvas.SetActive(false);


        // Ativa o canvas e reparenta para a âncora de rotação
        Canvas canvasComp = currentPlanet.GetComponentInChildren<Canvas>(true);
        if (canvasComp != null)
        {
            planetCanvas = canvasComp.gameObject;
            planetCanvas.SetActive(true);

            // Salva o pai original e faz o canvas ser filho da âncora
            originalCanvasParent = canvasComp.transform.parent;
            canvasComp.transform.SetParent(orbitAnchor.transform);
        }

        trailRenderers.Clear();
        TrailRenderer[] trails = FindObjectsOfType<TrailRenderer>();

        foreach (var trail in trails)
        {
            if (trail.enabled)
            {
                trail.enabled = false;
                trailRenderers.Add(trail); // Guarda pra reativar depois
            }
        }
    }

    void StopFocus()
    {
        isFocusing = false;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (planetCanvas != null)
        {
            // Restaura o parent original e desativa o canvas
            planetCanvas.transform.SetParent(originalCanvasParent);
            planetCanvas.SetActive(false);
        }
        planetCanvas = null;
        originalCanvasParent = null;

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (miraCanvas != null)
            miraCanvas.SetActive(true);

        if (planetCanvas != null)
            planetCanvas.SetActive(false);

        planetCanvas = null;

        Destroy(orbitAnchor.gameObject);
        orbitAnchor = null;

        foreach (var trail in trailRenderers)
        {
            if (trail != null)
                trail.enabled = true;
        }

        trailRenderers.Clear();
    }

    Vector3 GetPlanetCenter(Transform planet)
    {
        Collider col = planet.GetComponent<Collider>();
        return col ? col.bounds.center : planet.position;
    }
}
