using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCam;

    void Awake()
    {
        gameObject.SetActive(false); // desativa logo no início do jogo
    }
    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            // Gira o canvas para olhar na direção da câmera
            transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        }
    }
}
