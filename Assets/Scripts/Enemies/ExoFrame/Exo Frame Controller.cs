using UnityEngine;
using System.Collections;

public class ExoFrameController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHits = 5;
    [SerializeField] private int damageToPlayer = 15;
    [SerializeField] private float shootInterval = 1.5f;

    [Header("Movement AI")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float stoppingDistance = 4f;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab; // Prefab de BulletEnemy
    [SerializeField] private float bulletSpeed = 12f;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;


    private int currentHits = 0;
    private float lastShootTime = 0f;
    private bool isDead = false;
    private GameObject player;
    private Color originalColor;
    private Rigidbody2D rb;

    private int currentPatrolIndex = 0;
    private bool isChasing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isDead) return;

        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > detectionRange * 1.5f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
        UpdateSpriteDirection();
    }

    private void FixedUpdate()
    {
        
    }

    private void ChasePlayer(float distance)
    {
        if (player == null) return;

        if (distance > stoppingDistance)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= lastShootTime + shootInterval) 
            {
                ShootAtPlayer();
                lastShootTime = Time.time;
            }
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        float directionX = Mathf.Sign(targetPoint.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(directionX * (moveSpeed * 0.5f), rb.linearVelocity.y);

        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.2f)
        {
            currentPatrolIndex++;
            if (currentPatrolIndex >= patrolPoints.Length)
            {
                currentPatrolIndex = 0;
            }
        }
    }

    private void UpdateSpriteDirection()
    {
        if (isChasing && player != null)
        {
            bool playerOnRight = player.transform.position.x > transform.position.x;
            spriteRenderer.flipX = !playerOnRight;
        }
        else if (Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0;
        }
    }

    //Logica de disparo al jugador
    private void ShootAtPlayer()
    {
        if (firePoint == null || bulletPrefab == null || player == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                Vector2 direction = (player.transform.position - firePoint.position).normalized;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        //Cambia la linea si es que tenes que cambiar el nombre de Bulleta_Damage por un script de cero s
        Bullet_Damage be = bullet.GetComponent<Bullet_Damage>();
        if( be != null)
        {
            be.damage = damageToPlayer;
        }
    }

    //Daño y muerte 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("¡CONTACTO! El enemigo tocó: " + collision.gameObject.name);

        if (isDead) return;
        if (collision.CompareTag("Bullet"))
        {
            TakeHit();
            collision.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero;

            Player_Health health = collision.gameObject.GetComponent<Player_Health>();
            if (health != null)
                health.TakeDamage(damageToPlayer);
        }
    }
    private void TakeHit()
    {
        currentHits++;
        StartCoroutine(BlinkFeedback());
        if (currentHits >= maxHits)
            Die();
    }
    private IEnumerator BlinkFeedback()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.2f);
    }
}
