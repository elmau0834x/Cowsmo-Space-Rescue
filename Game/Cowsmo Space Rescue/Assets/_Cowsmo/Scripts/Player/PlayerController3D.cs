using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 7f;
    public float jumpForce = 6.5f;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float checkRadius = 0.25f;

    private Rigidbody2D rb;
    public Animator anim; // <-- Esta es la línea que faltaba declarar
    private float horizontalInput;
    private bool isGrounded;
    private bool facingRight = true;
    private float fixedZPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(); // <-- Aquí se obtiene el componente
        fixedZPosition = transform.position.z;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // 1. Lectura compatible con el nuevo Input System (Teclas A/D o Flechas)
        horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;
        }

        // 2. Detección de suelo en 2D
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        }

        // 3. Salto (Tecla Espacio)
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }
        }

        // 4. Actualizar animación de caminar
        UpdateAnimations();

        FlipModel();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (transform.position.z != fixedZPosition)
        {
            Vector3 pos = transform.position;
            pos.z = fixedZPosition;
            transform.position = pos;
        }
    }

    void UpdateAnimations()
    {
        if (anim != null)
        {
            // Envía 1.0 al moverse y 0.0 al detenerse usando tu parámetro Float "walk"
            anim.SetFloat("walk", Mathf.Abs(horizontalInput));
            anim.SetBool("isGrounded", isGrounded);
        }
    }

    void FlipModel()
    {
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}
