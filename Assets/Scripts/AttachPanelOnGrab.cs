using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachPanelOnGrab : MonoBehaviour
{
    public GameObject panel;  // O painel que será exibido
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(ShowPanel);
            grabInteractable.selectExited.AddListener(HidePanel);
        }
        panel.SetActive(false);
    }

    private void ShowPanel(SelectEnterEventArgs args)
    {
        // Ativa o painel e posiciona próximo ao objeto
        panel.SetActive(true);
        panel.transform.SetParent(grabInteractable.transform, false);
        //panel.transform.localPosition = new Vector3(0, 0, 0.2f); // Ajuste a posição conforme necessário
    }

    private void HidePanel(SelectExitEventArgs args)
    {
        // Oculta o painel quando o objeto é solto
        panel.SetActive(false);
    }
}
