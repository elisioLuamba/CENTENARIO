using UnityEngine;

public class CarregarPersonagem : MonoBehaviour
{
    public GameObject[] prefabsPersonagens;
    public SmoothCameraFollow cameraScript; 

    void Start()
    {
        DynamicGI.UpdateEnvironment();

        int indice = PlayerPrefs.GetInt("PersonagemEscolhido", 0);

        if (indice < prefabsPersonagens.Length)
        {
            // Instancia o personagem escolhido no mapa da Serra da Leba
            GameObject novoPlayer = Instantiate(prefabsPersonagens[indice], transform.position, transform.rotation);
            
            if (cameraScript != null)
            {
                cameraScript.SetTarget(novoPlayer);
            }
        }
    }
}