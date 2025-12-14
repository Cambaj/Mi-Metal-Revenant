using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    private void OnEnable()
    {
        StartCoroutine(DeactivateRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator DeactivateRoutine()
    { 
        yield return new WaitForSeconds(lifeTime);
        Bullet_Pool.Instance.ReturnBullet(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Bullet_Pool.Instance.ReturnBullet(this.gameObject);
        
        }
    }

}
