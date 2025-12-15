using UnityEngine;
using System.Collections;

public class PatrollerController : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float leftLimit = 6f;
    [SerializeField] private float rightLimit = 20f;

    [Header("Enemy Stats")]
    [SerializeField] private int maxHits = 2;         // cantidad de disparos para morir
    [SerializeField] private int damageToPlayer = 15; // daño al jugador

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = new Color(1, 0.3f, 0.3f); // color del blink
    private Color originalColor;

    private bool movingRight = false;
    private int currentHits = 0;
    private bool isDead = false;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (isDead) return;

        float dir = movingRight ? 1 : -1;
        transform.Translate(Vector3.right * dir * speed * Time.deltaTime);

        if (movingRight && transform.position.x >= rightLimit)
            FlipDirection(false);
        else if (!movingRight && transform.position.x <= leftLimit)
            FlipDirection(true);
    }

    private void FlipDirection(bool toRight)
    {
        movingRight = toRight;
        spriteRenderer.flipX = movingRight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(leftLimit, transform.position.y - 0.5f),
                        new Vector3(leftLimit, transform.position.y + 0.5f));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(rightLimit, transform.position.y - 0.5f),
                        new Vector3(rightLimit, transform.position.y + 0.5f));
    }

    // ------------------------------------------
    //   COLISIÓN CON EL JUGADOR
    // ------------------------------------------

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Health health = collision.gameObject.GetComponent<Player_Health>();
            if (health != null)
                health.TakeDamage(damageToPlayer);

            // cambiar de dirección al chocar
            FlipDirection(!movingRight);
        }
    }
    
    
    // ------------------------------------------
    //  COLISIÓN CON BALAS
    // ------------------------------------------
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Bullet"))
        {
            TakeHit();
            collision.gameObject.SetActive(false); // Desactivamos la bala al impactar
        }
    }
    
    
    private void TakeHit()
    {
        currentHits++;

        StartCoroutine(BlinkFeedback());

        if (currentHits >= maxHits)
            Die();
    }
    

    // ------------------------------------------
    //   FEEDBACK DE IMPACTO
    // ------------------------------------------

    
    private IEnumerator BlinkFeedback()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }
    

    // ------------------------------------------
    //   MUERTE DEL ENEMIGO
    // ------------------------------------------
    private void Die()
    {
        isDead = true;

        // podés poner animación de muerte acá
        // animator.SetTrigger("Die");

        Destroy(gameObject, 0.15f);
    }
}
