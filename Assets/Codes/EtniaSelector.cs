using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialMenuCompleto : MonoBehaviour
{
    [Header("Configurações do Layout")]
    [Range(50f, 500f)] public float raio = 200f;
    [Range(0f, 360f)] public float anguloInicial = 0f;

    [Header("Escalas")]
    [Range(1f, 1.5f)] public float escalaHover = 1.1f;
    [Range(1f, 2f)] public float escalaDestaque = 1.3f;

    [Header("Cores")]
    public Color corInativo = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color corAtivo = Color.white;

    private int indexSelecionado = -1;

    void Start() => AtualizarLayout();

    [ContextMenu("Atualizar Layout")]
    public void AtualizarLayout()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);
            
            // 1. Posicionamento
            float angle = (i * 360f / count + anguloInicial) * Mathf.Deg2Rad;
            child.localPosition = new Vector3(Mathf.Cos(angle) * raio, Mathf.Sin(angle) * raio, 0);

            // 2. Configura botões e eventos
            ConfigurarInteracoes(child.gameObject, i);
        }
    }

    void ConfigurarInteracoes(GameObject obj, int index)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn == null) btn = obj.AddComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => Selecionar(index));

        // Event Trigger para Hover (sem criar componentes novos no Start)
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = obj.AddComponent<EventTrigger>();
        
        trigger.triggers.Clear();

        // Hover Enter
        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        entryEnter.callback.AddListener((data) => { if (index != indexSelecionado) obj.transform.localScale = Vector3.one * escalaHover; });
        trigger.triggers.Add(entryEnter);

        // Hover Exit
        EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        entryExit.callback.AddListener((data) => { if (index != indexSelecionado) obj.transform.localScale = Vector3.one; });
        trigger.triggers.Add(entryExit);

        ResetarItem(obj.transform);
    }

    void Selecionar(int index)
    {
        indexSelecionado = index;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform item = transform.GetChild(i);
            if (i == indexSelecionado)
            {
                item.GetComponent<Image>().color = corAtivo;
                item.localScale = Vector3.one * escalaDestaque;
            }
            else ResetarItem(item);
        }
    }

    void ResetarItem(Transform item)
    {
        item.GetComponent<Image>().color = corInativo;
        item.localScale = Vector3.one;
    }
}