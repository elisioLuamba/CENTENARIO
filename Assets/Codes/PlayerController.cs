using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Necessário para mudar de cena

public class GerenciadorUnico : MonoBehaviour
{
    [Header("Configurações de Tags")]
    public string tagPlayer = "Player";
    public string tagFinal = "FINAL";
    public string tagInimigo = "Inimigo";
    public string tagChaoMortal = "chao";

    [Header("Status (Lógica Original)")]
    public int dominioCultural = 100;
    public int penalidade = 25;

    [Header("Interface UI")]
    public TextMeshProUGUI textoDistancia;
    public TextMeshProUGUI textoDominio;
    public Image imagemDano; 
    
    [Header("Configurações do Flash de Dano")]
    public float duracaoDano = 2.0f;
    [Range(0, 1)] public float opacidadeMaxima = 0.5f;

    [Header("Lógica de Rebobinar (2s)")]
    public float tempoHistorico = 2.0f;
    private List<RegistroPosicao> historicoPosicoes = new List<RegistroPosicao>();

    private Transform playerTransform;
    private Transform finalTransform;
    private Coroutine flashRoutine;

    private struct RegistroPosicao {
        public Vector3 posicao;
        public Quaternion rotacao;
        public float timestamp;
        public RegistroPosicao(Vector3 p, Quaternion r, float t) {
            posicao = p; rotacao = r; timestamp = t;
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject pObj = GameObject.FindGameObjectWithTag(tagPlayer);
            if (pObj != null)
            {
                playerTransform = pObj.transform;
                if (pObj.GetComponent<DetectorColisaoSimples>() == null)
                {
                    var detector = pObj.AddComponent<DetectorColisaoSimples>();
                    // Passamos as tags necessárias para o detector
                    detector.Configurar(this, tagInimigo, tagChaoMortal, tagFinal);
                }
            }
            return;
        }

        GravarPosicao();

        if (finalTransform == null)
        {
            GameObject fObj = GameObject.FindGameObjectWithTag(tagFinal);
            if (fObj != null) finalTransform = fObj.transform;
            return;
        }

        AtualizarUI();

        // Condição de Derrota: Domínio Cultural Zero
        if (dominioCultural <= 0)
        {
            SceneManager.LoadScene("Jango");
        }
    }

    void GravarPosicao()
    {
        historicoPosicoes.Add(new RegistroPosicao(playerTransform.position, playerTransform.rotation, Time.time));
        if (historicoPosicoes.Count > 0 && Time.time - historicoPosicoes[0].timestamp > tempoHistorico + 0.5f)
        {
            historicoPosicoes.RemoveAt(0);
        }
    }

    public void VoltarNoTempo()
    {
        if (historicoPosicoes.Count > 0)
        {
            float alvoTempo = Time.time - tempoHistorico;
            RegistroPosicao checkpoint = historicoPosicoes[0];
            foreach (var reg in historicoPosicoes)
            {
                if (reg.timestamp <= alvoTempo) checkpoint = reg;
                else break;
            }
            playerTransform.position = checkpoint.posicao;
            playerTransform.rotation = checkpoint.rotacao;
            historicoPosicoes.Clear();
        }
    }

    public void ChegouAoFinal()
    {
        // Condição de Vitória: Colisão com a tag FINAL
        SceneManager.LoadScene("FINAL");
    }

    void AtualizarUI()
    {
        float distancia = Vector3.Distance(playerTransform.position, finalTransform.position);
        if (textoDistancia != null)
            textoDistancia.text = "Caminho Da Leba: " + distancia.ToString("F1") + "m";
        
        if (textoDominio != null)
            textoDominio.text = "Domínio Cultural: " + dominioCultural + "%";
    }

    public void ProcessarBatida()
    {
        dominioCultural = Mathf.Max(0, dominioCultural - penalidade);
        if (imagemDano != null)
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(RotinaFlashDano());
        }
    }

    IEnumerator RotinaFlashDano()
    {
        float tempoDecorrido = 0f;
        Color corAtual = imagemDano.color;
        while (tempoDecorrido < duracaoDano)
        {
            tempoDecorrido += Time.deltaTime;
            float progresso = tempoDecorrido / duracaoDano;
            corAtual.a = Mathf.Lerp(opacidadeMaxima, 0f, progresso);
            imagemDano.color = corAtual;
            yield return null;
        }
        corAtual.a = 0f;
        imagemDano.color = corAtual;
    }
}

public class DetectorColisaoSimples : MonoBehaviour
{
    private GerenciadorUnico mestre;
    private string tagObstaculo;
    private string tagChao;
    private string tagFinal;

    public void Configurar(GerenciadorUnico g, string tagInim, string tagCh, string tagF)
    {
        mestre = g;
        tagObstaculo = tagInim;
        tagChao = tagCh;
        tagFinal = tagF;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(tagObstaculo))
        {
            mestre.ProcessarBatida();
        }
        else if (collision.gameObject.CompareTag(tagChao))
        {
            mestre.VoltarNoTempo();
        }
        else if (collision.gameObject.CompareTag(tagFinal))
        {
            mestre.ChegouAoFinal();
        }
    }
}