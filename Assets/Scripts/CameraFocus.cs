using UnityEngine;

public class PlanetCameraFocus : MonoBehaviour
{
    public float orbitDistance = 10f;
    public float orbitSpeed = 5f;
    public float verticalLimit = 80f;

    private Transform currentPlanet;
    private Transform orbitAnchor;
    private bool isFocusing = false;

    private Vector2 orbitAngles = new Vector2(30f, 0f); // pitch, yaw
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    private PlayerMovement playerMovement; // Referência ao script de movimento

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
    }

    void StopFocus()
    {
        isFocusing = false;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (playerMovement != null)
            playerMovement.enabled = true;

        Destroy(orbitAnchor.gameObject);
        orbitAnchor = null;
    }

    Vector3 GetPlanetCenter(Transform planet)
    {
        Collider col = planet.GetComponent<Collider>();
        return col ? col.bounds.center : planet.position;
    }
}
