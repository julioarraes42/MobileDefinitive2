using UnityEngine;

public class RandomCircularDistributor : MonoBehaviour
{
    [Header("Configurações de Distribuição")]
    public GameObject objetoParaClonar;
    public int quantidade = 20;
    public float raio = 5f;

    void Start()
    {
        DistribuirObjetosEmEsfera();
    }

    void DistribuirObjetosEmEsfera()
    {
        for (int i = 0; i < quantidade; i++)
        {
            // Gera uma direção aleatória normalizada
            Vector3 direcao = Random.onUnitSphere;

            // Posição final na superfície da esfera
            Vector3 posicao = transform.position + direcao * raio;

            // Instancia o clone e o torna filho deste objeto
            Instantiate(objetoParaClonar, posicao, Quaternion.identity, transform);
        }
    }
}
