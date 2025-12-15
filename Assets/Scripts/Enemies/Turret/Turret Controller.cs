using UnityEngine;
using System.Collections;

public class TurretController : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private int maxHits = 3;
    [SerializeField] private int damageToPlayer = 10;
    [SerializeField] private float shootInterval = 1.5f;

    [Header("Detection & Shooting")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab; // Prefab de BulletEnemy
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;

    private int currentHits = 0;
    private float lastShootTime = 0f;
    private bool isDead = false;
    private GameObject player;
    private Color originalColor;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (isDead || player == null) return;

        // Flip del sprite según lado del jugador
        bool playerOnRight = player.transform.position.x > transform.position.x;
        spriteRenderer.flipX = !playerOnRight;

        // Disparo si jugador en rango y cooldown listo
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= detectionRange && Time.time >= lastShootTime + shootInterval)
        {
            ShootAtPlayer();
            lastShootTime = Time.time;
        }
    }

    private void ShootAtPlayer()
    {
        if (firePoint == null || bulletPrefab == null || player == null) return;

        // Instanciar bala
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);


        Vector2 direction = (player.transform.position - firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Bullet_Damage be = bullet.GetComponent<Bullet_Damage>();
        if (be != null)
        {
            be.damage = damageToPlayer;
        }


    }

    // ------------------------------
    // Recepción de daño de balas del jugador
    // ------------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Bullet")) // balas del player
        {
            TakeHit();
            collision.gameObject.SetActive(false);
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
        Destroy(gameObject, 0.1f);
    }

    // ------------------------------
    // Daño al jugador al chocar
    // ------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Health health = collision.gameObject.GetComponent<Player_Health>();
            if (health != null)
                health.TakeDamage(damageToPlayer);
        }
    }
}
