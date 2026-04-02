using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("Referências Dinâmicas")]
    public Transform player; // Agora será preenchido automaticamente
    private Rigidbody playerRb; 

    [Header("Configurações de posição")]
    public Vector3 offset = new Vector3(0, 4, -7);
    public float followSpeed = 5f;
    public float rotationSpeed = 5f;

    [Header("Efeito de balanço")]
    public float swayAmount = 0.2f;
    public float swaySpeed = 4f;
    public float bobAmount = 0.1f;
    public float bobSpeed = 6f;

    private Vector3 velocity = Vector3.zero;

    // Função que o Spawner vai chamar para entregar o alvo à câmara
    public void SetTarget(GameObject target)
    {
        player = target.transform;
        playerRb = target.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        // Se o personagem ainda não foi instanciado, a câmara fica parada
        if (player == null) return;

        Vector3 targetPos = player.position + offset;

        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        Vector3 dynamicOffset = new Vector3(sway, bob, 0);
        targetPos += transform.TransformDirection(dynamicOffset);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1f / followSpeed);

        Quaternion targetRot = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }
}