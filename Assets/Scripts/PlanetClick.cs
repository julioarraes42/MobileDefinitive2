using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetClickHandler : MonoBehaviour
{
    public Text infoText;        // Referência ao texto no Canvas
    public GameObject panel;     // Referência ao painel no Canvas
    public Canvas canvas;        // Referência ao Canvas

    public float cameraDistance = 5f;   // Distância entre a câmera e o planeta
    public float offsetHorizontal = 2f; // Distância para deslocar o planeta para a direita

    private PlanetInfo currentPlanetInfo; // Variável para armazenar o script do planeta clicado
    private Vector3 originalCameraPosition; // Variável para armazenar a posição original da câmera
    private Quaternion originalCameraRotation; // Variável para armazenar a rotação original da câmera
    private Camera mainCamera; // Referência à câmera principal
    private Renderer[] allRenderers; // Armazena todos os renderizadores na cena
    private List<Renderer> disabledRenderers = new List<Renderer>(); // Armazena os renderizadores desativados

    void Start()
    {
        panel.SetActive(false);  // Desativa o painel no início

        // Encontra a câmera dentro do GameObject "Cube" com a tag player
        mainCamera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();

        if (mainCamera == null)
        {
            Debug.LogError("A câmera principal não foi encontrada dentro do GameObject 'cube'.");
            return;
        }

        originalCameraPosition = mainCamera.transform.position; // Armazena a posição original da câmera
        originalCameraRotation = mainCamera.transform.rotation; // Armazena a rotação original da câmera

        // Encontra todos os renderizadores na cena
        allRenderers = FindObjectsOfType<Renderer>();
    }

    void Update()
    {
        // Verifica se o botão esquerdo do mouse foi pressionado
        if (Input.GetMouseButtonDown(0))
        {
            if (mainCamera == null)
            {
                Debug.LogError("A câmera principal foi destruída.");
                return;
            }

            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);  // Cria um raio a partir da posição do mouse

            if (Physics.Raycast(ray, out hit))
            {
                // Verifica se o objeto atingido é um planeta
                if (hit.transform.CompareTag("Planet"))
                {
                    // Acessa o script PlanetInfo do planeta clicado
                    currentPlanetInfo = hit.transform.GetComponent<PlanetInfo>();

                    if (currentPlanetInfo != null)
                    {
                        // Atualiza o texto com a descrição do planeta
                        infoText.text = currentPlanetInfo.planetDescription;

                        // Move a câmera para a posição adequada e organiza o texto
                        MoveCameraToPlanet(hit.transform);

                        // Torna o painel visível
                        panel.SetActive(true);
                        Debug.Log("Painel ativado.");

                        // Desativa todos os outros objetos
                        HideOtherObjects(hit.transform);

                        PauseManager.isPaused = true;
                    }
                }
            }
            else
            {
                // Se o clique foi fora de qualquer planeta, desativa o painel
                panel.SetActive(false);
                Debug.Log("Painel desativado.");

                // Limpa o texto (fazendo ele ficar em branco)
                infoText.text = "";

                // Retoma o movimento dos planetas
                PauseManager.isPaused = false;

                // Reativa todos os objetos
                ShowAllObjects();
            }
        }

        // Verifica se a tecla Esc foi pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Retorna a câmera para a posição original
            mainCamera.transform.position = originalCameraPosition;
            mainCamera.transform.rotation = originalCameraRotation;

            // Desativa o painel
            panel.SetActive(false);
            Debug.Log("Painel desativado pelo ESC.");

            // Limpa o texto (fazendo ele ficar em branco)
            infoText.text = "";

            // Retoma o movimento dos planetas
            PauseManager.isPaused = false;

            // Reativa todos os objetos
            ShowAllObjects();
        }
    }

    private void MoveCameraToPlanet(Transform planet)
    {
        if (mainCamera == null)
        {
            Debug.LogError("A câmera principal foi destruída.");
            return;
        }

        // Calcula a direção do planeta a partir da câmera
        Vector3 directionToPlanet = (planet.position - mainCamera.transform.position).normalized;

        // Calcula a nova posição da câmera a uma distância fixa em frente ao planeta
        Vector3 newCameraPosition = planet.position - directionToPlanet * cameraDistance;

        // Ajusta o deslocamento horizontal (direita)
        newCameraPosition += mainCamera.transform.right * offsetHorizontal;

        // Atualiza a posição da câmera
        mainCamera.transform.position = newCameraPosition;

        // Garante que a câmera fique voltada para o planeta
        mainCamera.transform.LookAt(planet);

        // Posiciona o painel ao lado do planeta
        PositionPanel(planet);
    }

    private void PositionPanel(Transform planet)
    {
        // Calcula a posição do painel ao lado do planeta
        Vector3 panelPosition = planet.position + planet.right * offsetHorizontal;

        // Ajusta o painel para a nova posição
        panel.transform.position = panelPosition;

        // Faz o painel olhar para a câmera
        panel.transform.rotation = Quaternion.LookRotation(panel.transform.position - mainCamera.transform.position);
    }

    private void HideOtherObjects(Transform clickedPlanet)
    {
        foreach (Renderer renderer in allRenderers)
        {
            if (renderer.transform != clickedPlanet && renderer.transform != panel.transform)
            {
                renderer.enabled = false;
                disabledRenderers.Add(renderer);
            }
        }
    }

    private void ShowAllObjects()
    {
        foreach (Renderer renderer in disabledRenderers)
        {
            renderer.enabled = true;
        }
        disabledRenderers.Clear();
    }
}

