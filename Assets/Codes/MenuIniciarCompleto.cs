using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuFinalInfinito : MonoBehaviour
{
    [Header("Configurações de Cena")]
    public string nomeDaCenaParaCarregar;

    [Header("Configurações do Fundo")]
    public Image imagemFundo;
    public float velocidadeX = 0.1f;

    [Header("Correção de Esticamento (Tiling)")]
    [Tooltip("Aumente se a imagem estiver esticada horizontalmente")]
    public float repetiçãoX = 1f;
    [Tooltip("Aumente se a imagem estiver esticada verticalmente")]
    public float repetiçãoY = 1f;

    private Material materialInstanciado;

    void Start()
    {
        if (imagemFundo != null)
        {
            // Criamos um novo material baseado no Shader padrão de UI
            // Isso evita que a gente altere o arquivo original do material no projeto
            materialInstanciado = new Material(Shader.Find("UI/Default"));
            imagemFundo.material = materialInstanciado;

            // Define a repetição (Tiling) para corrigir o aspecto da imagem
            materialInstanciado.mainTextureScale = new Vector2(repetiçãoX, repetiçãoY);
        }
        else
        {
            Debug.LogError("Por favor, arraste o componente Image para o script!");
        }
    }

    void Update()
    {
        if (materialInstanciado != null)
        {
            // Move o Offset da textura continuamente
            Vector2 offset = materialInstanciado.mainTextureOffset;
            offset.x += velocidadeX * Time.deltaTime;
            materialInstanciado.mainTextureOffset = offset;
        }
    }

    // --- Funções para os Botões ---

    public void BotaoIniciar()
    {
        if (!string.IsNullOrEmpty(nomeDaCenaParaCarregar))
        {
            SceneManager.LoadScene(nomeDaCenaParaCarregar);
        }
        else
        {
            Debug.LogWarning("Nome da cena não foi configurado!");
        }
    }

    public void BotaoSair()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }

    // Limpeza de memória ao destruir o objeto
    void OnDestroy()
    {
        if (materialInstanciado != null)
        {
            Destroy(materialInstanciado);
        }
    }
}