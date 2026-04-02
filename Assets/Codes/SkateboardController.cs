using UnityEngine;

public class SkateboardController : MonoBehaviour
{
    [Header("Configurações de movimento")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float rotationSpeed = 100f;
    public float acceleration = 5f;

    [Header("Configurações de Chão")]
    public float groundCheckDistance = 1f; // Distância do Raycast para medir altura do chão
    public LayerMask groundLayer;          // Layer do chão

    [Header("Referências")]
    public Animator animator;

    private float currentSpeed = 0f;
    private float targetSpeed = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.freezeRotation = true;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // Velocidade alvo
        targetSpeed = (isRunning ? runSpeed : walkSpeed) * moveInput;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // Movimento horizontal
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Rotação e tilt
        if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);
            float tilt = turnInput * 15f;
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -tilt);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        }

        // Ajuste de altura automática (mantém o skate colado ao chão)
        UpdateVerticalPosition();

        // Atualiza animações
        UpdateAnimations(isRunning);
    }

    void UpdateVerticalPosition()
    {
        float groundHeight = GetGroundHeight();
        Vector3 pos = transform.position;
        pos.y = groundHeight;
        transform.position = pos;
    }

    void UpdateAnimations(bool isRunning)
    {
        if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            animator.SetBool("isIdle", false);
            if (isRunning)
            {
                animator.SetBool("isRunning", true);
                animator.SetBool("isScooterIdle", false);
            }
            else
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isScooterIdle", true);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            // Corrigido para manter a lógica original onde o skate fica parado
            animator.SetBool("isScooterIdle", false); 
            animator.SetBool("isIdle", true);
        }
    }

    // Retorna a altura do chão abaixo do personagem
    float GetGroundHeight()
    {
        RaycastHit hit;
        // O Raycast começa um pouco acima (0.1f) para evitar detectar o próprio skate se o pivô estiver no chão
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            return hit.point.y;
        }
        else
        {
            return transform.position.y;
        }
    }
}