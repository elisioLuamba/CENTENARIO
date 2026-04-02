using UnityEngine;
using UnityEngine.UI;

public class MenuSelecao : MonoBehaviour
{
    [Header("Configurações de Prefabs")]
    public GameObject[] prefabsPersonagens; // Arraste os PREFABS da pasta Assets para aqui
    public Transform pontoDeSpawn;         // Crie um objeto vazio no centro para ser o local do boneco
    
    private GameObject personagemAtual;     // Guarda o boneco que está na tela agora
    private int indiceAtual = 0;

    void Start()
    {
        indiceAtual = PlayerPrefs.GetInt("PersonagemEscolhido", 0);
        SpawnPersonagemNoMenu();
    }

    public void Avancar()
    {
        indiceAtual++;
        if (indiceAtual >= prefabsPersonagens.Length) indiceAtual = 0;
        SalvarEAtualizar();
    }

    public void Recuar()
    {
        indiceAtual--;
        if (indiceAtual < 0) indiceAtual = prefabsPersonagens.Length - 1;
        SalvarEAtualizar();
    }

    private void SalvarEAtualizar()
    {
        PlayerPrefs.SetInt("PersonagemEscolhido", indiceAtual);
        PlayerPrefs.Save();
        SpawnPersonagemNoMenu();
    }

    private void SpawnPersonagemNoMenu()
    {
        // 1. Destrói o boneco anterior para não encher a cena
        if (personagemAtual != null) Destroy(personagemAtual);

        // 2. Instancia o novo baseado no Prefab
        if (prefabsPersonagens[indiceAtual] != null)
        {
            personagemAtual = Instantiate(prefabsPersonagens[indiceAtual], pontoDeSpawn.position, pontoDeSpawn.rotation);
            
            // Opcional: Se o boneco vier com scripts de movimento, desative-os no menu
            // Ex: personagemAtual.GetComponent<PlayerMovement>().enabled = false;
        }
    }
}