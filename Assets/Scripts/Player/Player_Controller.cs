using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("References")]

    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;

    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private Animator_Controller animController;
    [SerializeField] private SpriteRenderer spriteRenderer; 

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Ground Checking")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Inputs")]
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDashing;
    private bool canDash = true;
    private float originalGravity;


    [Header("Player States")]
    public bool Is_idle;
    public bool Is_moving;
    public bool Is_jumping;
    public bool Is_dashing;
    public bool Is_falling;

    private void Awake()
    {
        originalGravity = playerRigidbody.gravityScale;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        dashAction.action.Enable();

        jumpAction.action.started += HandleJumpInput;
        dashAction.action.started += HandleDashInput;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        dashAction.action.Disable();

       jumpAction.action.started-= HandleJumpInput;
        dashAction.action.started -= HandleDashInput;
    }

    private void Update()
    {
        if (isDashing) return;

       moveInput = moveAction.action.ReadValue<Vector2>();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (moveInput.x > 0) spriteRenderer.flipX = false;
        else if (moveInput.x < 0) spriteRenderer.flipX = true;

        Is_moving = Mathf.Abs(moveInput.x) > 0.1f;
        Is_jumping = !isGrounded && playerRigidbody.linearVelocity.y > 0.1f;
        Is_falling = !isGrounded && playerRigidbody.linearVelocity.y < -0.1f;
        Is_dashing = isDashing;
        Is_idle = !Is_moving && !Is_jumping && !Is_falling && !Is_dashing;

        if (animController != null)
        {
            animController.UpdateAnimation(Is_moving, Is_idle, Is_jumping, Is_falling, Is_dashing);
        }

    }
    //JUMP 
    private void FixedUpdate()
    {
        if(isDashing) return;

        playerRigidbody.linearVelocity = new Vector2(moveInput.x * moveSpeed, playerRigidbody.linearVelocity.y);
    }

    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        if (isGrounded && !isDashing)
        {
            playerRigidbody.linearVelocity = new Vector2(playerRigidbody.linearVelocity.x, jumpForce);
        }
    }



//DASH
private void HandleDashInput(InputAction.CallbackContext context) 
    {
        if (isGrounded && !isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }
    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;

        playerRigidbody.gravityScale = 0f;

        float dashDirection = spriteRenderer.flipX ? -1 : 1f;
        if (moveInput.x != 0) dashDirection = Mathf.Sign(moveInput.x);

        playerRigidbody.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        playerRigidbody.gravityScale = originalGravity;
        playerRigidbody.linearVelocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

}


    /*
    [Header("References")]

    
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Input System References")]
    // Arrastra aquí las acciones desde tu Input Action Asset
    [SerializeField] private InputActionReference moveActionToUse;
    [SerializeField] private InputActionReference jumpActionToUse;
    [SerializeField] private InputActionReference dashActionToUse;
    [SerializeField] private InputActionReference ShootActionToUse;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 60f;

    private Vector2 rawInputMovement; // Almacena X e Y
    private float currentSpeed;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    private bool isGrounded;
    private float originalGravity;

    [Header("Better Jump")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.15f;
    [SerializeField] private float dashCooldown = 0.5f;

    private bool isDashing;
    private float lastDashTime = -10f;

    private void Awake()
    {
        // Buena práctica inicializar referencias si no están asignadas (opcional)
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        originalGravity = rb.gravityScale;
    }

    // --------------------------------------------------
    // ACTIVAR/DESACTIVAR INPUTS
    // --------------------------------------------------
    private void OnEnable()
    {
        moveActionToUse.action.Enable();
        jumpActionToUse.action.Enable();
        dashActionToUse.action.Enable();
    }

    private void OnDisable()
    {
        moveActionToUse.action.Disable();
        jumpActionToUse.action.Disable();
        dashActionToUse.action.Disable();
    }

    private void Update()
    {
        ReadInputs();
        CheckGround();
        ApplyBetterJump();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
            Move();
    }

    // --------------------------------------------------
    // INPUTS (ADAPTADO)
    // --------------------------------------------------
    private void ReadInputs()
    {
        // Leemos el Vector2 completo (útil para dash direccional)
        rawInputMovement = moveActionToUse.action.ReadValue<Vector2>();

        // Detectar si se presionó el botón de salto este frame
        if (jumpActionToUse.action.WasPressedThisFrame())
            Jump();

        // Detectar si se presionó el botón de dash este frame
        if (dashActionToUse.action.WasPressedThisFrame())
            TryDash();
    }

    // --------------------------------------------------
    // MOVEMENT
    // --------------------------------------------------
    private void Move()
    {
        // Usamos rawInputMovement.x en lugar de moveInput variable
        float targetSpeed = rawInputMovement.x * moveSpeed;

        if (Mathf.Abs(targetSpeed) > 0.1f)
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);

        // Nota: linearVelocity es de Unity 6. Si usas una versión anterior, cambia a rb.velocity
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);

        if (rawInputMovement.x != 0)
            sprite.flipX = rawInputMovement.x < 0;
    }

    // --------------------------------------------------
    // JUMP
    // --------------------------------------------------
    private void Jump()
    {
        if (!isGrounded || isDashing) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // --------------------------------------------------
    // BETTER JUMP (ADAPTADO)
    // --------------------------------------------------
    private void ApplyBetterJump()
    {
        if (isDashing) return;

        // Caída más rápida
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Salto corto: Si estamos subiendo Y NO estamos manteniendo el botón presionado
        else if (rb.linearVelocity.y > 0 && !jumpActionToUse.action.IsPressed())
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // --------------------------------------------------
    // DASH (ADAPTADO)
    // --------------------------------------------------
    private void TryDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;
        if (isDashing) return;

        // Usamos el input que ya leímos en ReadInputs
        float dashX = rawInputMovement.x;
        float dashY = rawInputMovement.y;

        // Si no hay input, dash hacia adelante según el sprite
        if (Mathf.Approximately(dashX, 0f) && Mathf.Approximately(dashY, 0f))
            dashX = sprite.flipX ? -1f : 1f;

        Vector2 dashDirection = new Vector2(dashX, dashY).normalized;

        StartCoroutine(DashRoutine(dashDirection));
    }

    private System.Collections.IEnumerator DashRoutine(Vector2 direction)
    {
        isDashing = true;
        lastDashTime = Time.time;

        rb.gravityScale = 0;
        rb.linearVelocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    // --------------------------------------------------
    // ANIMATOR
    // --------------------------------------------------
    private void UpdateAnimator()
    {
        bool running = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool jumping = rb.linearVelocity.y > 0.1f && !isGrounded;
        bool falling = rb.linearVelocity.y < -0.1f && !isGrounded;
        bool idle = !running && isGrounded && !isDashing;

        animator.SetBool("isIdle", idle);
        animator.SetBool("isRunning", running);
        animator.SetBool("isJumping", jumping);
        animator.SetBool("isFalling", falling);
        animator.SetBool("isDashing", isDashing);
    }

    // Métodos públicos auxiliares
    public bool IsRunning() => Mathf.Abs(rb.linearVelocity.x) > 0.1f && isGrounded && !isDashing;
    public bool IsJumping() => rb.linearVelocity.y > 0.1f && !isGrounded;

    // --------------------------------------------------
    // GIZMOS
    // --------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
    */

