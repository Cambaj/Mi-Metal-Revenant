using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Shoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private InputActionReference shootAction;

    [Header("Bullet Settings")]
    //[SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    public static int bulletDamage = 10;

    void Start()
    {
        if(shootAction != null)
        {
            shootAction.action.Enable();
            shootAction.action.started += HandleShootInput;
        }
        else
        {
            Debug.LogError("Shoot Action no esta asignado en PlayerShoot");
        }
    }

    private void OnDestroy()
    {
        if (shootAction != null)
        {
            shootAction.action.started -= HandleShootInput;
        }

    }

    private void HandleShootInput(InputAction.CallbackContext context)
    {
        Shoot();
    }

    private void Shoot()
    {
        GameObject bullet = Bullet_Pool.Instance.GetBullet();

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint. rotation;

        float direction = spriteRenderer.flipX ? -1 : 1f;

        

        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
        if (bulletRigidbody != null)
        {
            bulletRigidbody.linearVelocity = new Vector2(direction * bulletSpeed, 0f);
        }
    }

}
