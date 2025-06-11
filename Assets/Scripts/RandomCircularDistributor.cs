using UnityEngine;

public class RandomCircularDistributor : MonoBehaviour
{
    [Header("Configura��es de Distribui��o")]
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
            // Gera uma dire��o aleat�ria normalizada
            Vector3 direcao = Random.onUnitSphere;

            // Posi��o final na superf�cie da esfera
            Vector3 posicao = transform.position + direcao * raio;

            // Instancia o clone e o torna filho deste objeto
            Instantiate(objetoParaClonar, posicao, Quaternion.identity, transform);
        }
    }
}
