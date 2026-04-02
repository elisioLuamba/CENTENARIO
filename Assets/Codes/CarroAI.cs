using UnityEngine;
using System.Collections.Generic;

public class ControladorSerraDefinitivo : MonoBehaviour
{
    [System.Serializable]
    public class ConfigCarro
    {
        public string nome = "Novo Carro";
        public GameObject prefabCarro;
        [Tooltip("Ajuste individual de rotação para este modelo específico")]
        public float offsetRotacaoY = 90f; 
    }

    [Header("Configurações de Spawn (Estilo Subway Surfers)")]
    public ConfigCarro[] tiposDeCarros; 
    public Transform[] waypoints; 
    public float intervaloMinimoEntreCarros = 3f; // Segundos entre um carro e outro
    public float intervaloMaximoEntreCarros = 6f; // Variação para não ser previsível

    [Header("Parâmetros de Movimento")]
    public float velocidade = 10f;
    public float suavidadeRotacao = 5f;
    public float distanciaProximoPonto = 2f;

    [Header("Inteligência de Curva")]
    public float raioInícioCurva = 4f;

    private float cronometroProximoSpawn;
    private Dictionary<Transform, int> indicesDosCarros = new Dictionary<Transform, int>();
    private Dictionary<Transform, float> offsetsDosCarros = new Dictionary<Transform, float>();
    private List<Transform> carrosAtivos = new List<Transform>();

    void Start()
    {
        // Define o tempo para o primeiro carro aparecer
        DefinirNovoTempoSpawn();
    }

    void Update()
    {
        GerenciarFilaDeSpawn();
        MoverCarrosAtivos();
    }

    void GerenciarFilaDeSpawn()
    {
        if (waypoints.Length == 0 || tiposDeCarros.Length == 0) return;

        cronometroProximoSpawn -= Time.deltaTime;

        if (cronometroProximoSpawn <= 0)
        {
            // Escolhe um carro aleatório da lista
            int indiceAleatorio = Random.Range(0, tiposDeCarros.Length);
            SpawnarCarro(tiposDeCarros[indiceAleatorio]);
            
            // Reinicia o cronômetro com um novo tempo aleatório
            DefinirNovoTempoSpawn();
        }
    }

    void DefinirNovoTempoSpawn()
    {
        cronometroProximoSpawn = Random.Range(intervaloMinimoEntreCarros, intervaloMaximoEntreCarros);
    }

    void SpawnarCarro(ConfigCarro config)
    {
        if (config.prefabCarro == null) return;

        GameObject novoCarro = Instantiate(config.prefabCarro, waypoints[0].position, waypoints[0].rotation);
        Transform t = novoCarro.transform;
        
        offsetsDosCarros.Add(t, config.offsetRotacaoY);
        indicesDosCarros.Add(t, 0);
        carrosAtivos.Add(t);

        t.rotation = waypoints[0].rotation * Quaternion.Euler(0, -config.offsetRotacaoY, 0);
    }

    void MoverCarrosAtivos()
    {
        for (int i = carrosAtivos.Count - 1; i >= 0; i--)
        {
            Transform carro = carrosAtivos[i];

            if (carro == null) {
                carrosAtivos.RemoveAt(i);
                continue;
            }

            int indiceAtual = indicesDosCarros[carro];
            float offsetIndividual = offsetsDosCarros[carro];

            if (indiceAtual < waypoints.Length)
            {
                Transform pontoAlvo = waypoints[indiceAtual];
                Vector3 posicaoDestino = pontoAlvo.position;
                float distanciaParaOPonto = Vector3.Distance(carro.position, posicaoDestino);
                
                Quaternion rotacaoAlvo = pontoAlvo.rotation * Quaternion.Euler(0, -offsetIndividual, 0);

                if (distanciaParaOPonto < raioInícioCurva)
                {
                    carro.rotation = Quaternion.Slerp(carro.rotation, rotacaoAlvo, Time.deltaTime * suavidadeRotacao);
                }

                carro.position = Vector3.MoveTowards(carro.position, posicaoDestino, velocidade * Time.deltaTime);

                if (distanciaParaOPonto < distanciaProximoPonto)
                {
                    indicesDosCarros[carro]++;
                    
                    if (indicesDosCarros[carro] >= waypoints.Length)
                    {
                        indicesDosCarros.Remove(carro);
                        offsetsDosCarros.Remove(carro);
                        carrosAtivos.RemoveAt(i);
                        Destroy(carro.gameObject);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i+1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i+1].position);
        }
    }
}