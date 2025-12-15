using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Bullet_Damage : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 10;
    public float speed = 10f;

    private Rigidbody2D rb;

    // Esto se usa para ignorar colisión con la torreta
    private Collider2D ignoreCollider;

    /// <summary>
    /// Inicializa la bala. Llama desde la torreta después de instanciarla.
    /// </summary>
    public void Initialize(Transform target, Collider2D turretCollider)
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        // Calcular dirección hacia el target en el momento del disparo
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Ignorar colisión con la torreta
        Collider2D bulletCollider = GetComponent<Collider2D>();
        if (turretCollider != null && bulletCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, turretCollider);
        }

        ignoreCollider = turretCollider;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Colisión con jugador
        if (collision.CompareTag("Player"))
        {
            Player_Health health = collision.GetComponent<Player_Health>();
            if (health != null)
                health.TakeDamage(damage);

            gameObject.SetActive(false);
        }
        // Colisión con balas del jugador
        else if (collision.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);
        }
    }
}
