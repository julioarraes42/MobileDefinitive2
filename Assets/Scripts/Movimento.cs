using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimento : MonoBehaviour
{
    public float velocidadeMovimento = 8.0f;
    public float sensibilidadeMouse = 2.0f;
    public Transform cameraTransform;

    private float rotacaoVertical = 0.0f;

    private Vector3 velocidadeAtual;
    private Vector3 velocidadeSuave = Vector3.zero;
    public float suavidadeMovimento = 0.1f;

    private Quaternion rotacaoAtual;
    private Quaternion rotacaoSuave;
    public float suavidadeRotacao = 0.1f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        print("Olá Mundo");
    }

    void Update()
    {
        if (PauseManager.isPaused)
            return; // Sai do Update se estiver pausado

        // Movimento horizontal e frontal
        float movimentoX = Input.GetAxis("Horizontal");
        float movimentoZ = Input.GetAxis("Vertical");

        // Movimento vertical relativo à câmera
        float movimentoY = 0.0f;
        if (Input.GetKey(KeyCode.Q)) movimentoY = 1.0f; // Subir
        if (Input.GetKey(KeyCode.E)) movimentoY = -1.0f; // Descer

        // Direção do movimento considerando a câmera
        Vector3 movimento =
            (cameraTransform.right * movimentoX) +
            (cameraTransform.forward * movimentoZ) +
            (cameraTransform.up * movimentoY);

        // Normaliza para evitar aumento de velocidade diagonal
        movimento = movimento.normalized * velocidadeMovimento * Time.deltaTime;

        // Suaviza o movimento
        velocidadeAtual = Vector3.SmoothDamp(velocidadeAtual, movimento, ref velocidadeSuave, suavidadeMovimento);
        transform.Translate(velocidadeAtual, Space.World);

        // Movimento do mouse para girar
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadeMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadeMouse;

        rotacaoVertical -= mouseY;

        // Suaviza a rotação
        rotacaoAtual = Quaternion.Euler(rotacaoVertical, transform.eulerAngles.y + mouseX, 0f);
        rotacaoSuave = Quaternion.Slerp(transform.rotation, rotacaoAtual, suavidadeRotacao);
        transform.rotation = rotacaoSuave;

        cameraTransform.localRotation = Quaternion.Euler(rotacaoVertical, 0f, 0f);
    }
}
