using UnityEngine;

[ExecuteInEditMode]
public class CameraVerticalFocada : MonoBehaviour
{
    [Header("Configurações de Aspecto")]
    public float aspectoAlvo = 9f / 16f; 
    public Color corDasBordas = Color.black;

    private Camera cam;
    private Camera camFundo;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        CriarCameraDeFundo();
        AjustarJanela();
    }

    void Update()
    {
        AjustarJanela();
    }

    void CriarCameraDeFundo()
    {
        // Verifica se já existe uma câmera de fundo para não duplicar
        foreach (Camera c in GetComponentsInChildren<Camera>())
        {
            if (c.name == "Camera_Fundo_Sistema") return;
        }

        // Cria a câmera via código
        GameObject fundoObj = new GameObject("Camera_Fundo_Sistema");
        fundoObj.transform.SetParent(transform);
        
        camFundo = fundoObj.AddComponent<Camera>();
        camFundo.clearFlags = CameraClearFlags.SolidColor;
        camFundo.backgroundColor = corDasBordas;
        camFundo.cullingMask = 0;          // Não renderiza objetos, apenas cor
        camFundo.depth = cam.depth - 1;    // Fica atrás da câmera principal
        camFundo.farClipPlane = 1f;        // Economiza processamento
    }

    void AjustarJanela()
    {
        if (cam == null) cam = GetComponent<Camera>();

        float aspectoJanela = (float)Screen.width / Screen.height;
        float escalaAlvo = aspectoJanela / aspectoAlvo;

        Rect rect = cam.rect;

        if (escalaAlvo < 1.0f)
        {
            // Janela mais larga que 9:16
            rect.width = 1.0f;
            rect.height = escalaAlvo;
            rect.x = 0;
            rect.y = (1.0f - escalaAlvo) / 2.0f;
        }
        else
        {
            // Janela mais alta que 9:16
            float escalaInversa = 1.0f / escalaAlvo;
            rect.width = escalaInversa;
            rect.height = 1.0f;
            rect.x = (1.0f - escalaInversa) / 2.0f;
            rect.y = 0;
        }

        cam.rect = rect;

        // A câmera de fundo sempre ocupa 100% da tela para cobrir os vãos
        if (camFundo != null) camFundo.rect = new Rect(0, 0, 1, 1);
    }

    // Limpa a câmera auxiliar se o script for removido
    void OnDisable()
    {
        if (camFundo != null) DestroyImmediate(camFundo.gameObject);
    }
}