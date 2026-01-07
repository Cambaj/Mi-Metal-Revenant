using UnityEngine;
using System.Collections;
using System;

public class Bullet_Behaviour : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Time seconds before the bullet recycles if doesn't collisions with anything")]
    [SerializeField] private float lifeTime = 2f;

    private void OnEnable()
    {
        StartCoroutine(Desactivationroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("The bullet hit the enemy");

            RecycleBullet();
        }
        else if (collision.CompareTag("Ground"))
        {
            RecycleBullet();
        }
    }

    private IEnumerator Desactivationroutine()
    {
        yield return new WaitForSeconds(lifeTime);
                RecycleBullet();
    }

    private void RecycleBullet()
    {
        if (Bullet_Pool.Instance != null)
        {
            Bullet_Pool.Instance.ReturnBullet(this.gameObject);
        }
        else 
        {
            gameObject.SetActive(false);
            Debug.LogWarning("Bullet Pool not found,manual bullet desactivation");
        }
    }
}
