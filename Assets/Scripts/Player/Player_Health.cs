using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public Image healthBarFill;

    [Header("Damage Settings")]
    public float damageCooldown = 2f;
    private float lastDamageTime = -10f;

    [Header("Visual Effects")]
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        string sceneName = SceneManager.GetActiveScene().name;

        // Si empezás el Nivel 1, siempre vida máxima
        if (sceneName == "Level1")
        {
            currentHealth = maxHealth;
            PlayerPrefs.SetInt("PlayerHealth", currentHealth);
            PlayerPrefs.Save();
        }
        else
        {
            // En niveles posteriores mantenés la vida
            if (PlayerPrefs.HasKey("PlayerHealth"))
                currentHealth = PlayerPrefs.GetInt("PlayerHealth");
            else
                currentHealth = maxHealth;
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Recibiste daño!");
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lastDamageTime = Time.time;

        UpdateHealthBar();

        StartCoroutine(FlashRedEffect());

        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.Save();

        Debug.Log("vida: " + currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    //red effect on player
    private IEnumerator FlashRedEffect()
    {

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.material.color = Color.red;

            Debug.Log("PAINTED RED");

            yield return new WaitForSeconds(0.2f);
            spriteRenderer.material.color = Color.white;
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.Save();
    }
    
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float fillAmount = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = fillAmount;

            healthBarFill.color =
                fillAmount > 0.6f ? Color.green :
                fillAmount > 0.3f ? Color.yellow :
                Color.red;
        }
    }
    
    private void Die()
    {
        PlayerPrefs.SetString("GameResult", "Game Over");
        PlayerPrefs.DeleteKey("PlayerHealth");

      //LevelManager.instance.LoadScene("ResultScene");

        Destroy(gameObject);
    }
    
}
