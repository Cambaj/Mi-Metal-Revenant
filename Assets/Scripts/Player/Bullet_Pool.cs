using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;

public class Bullet_Pool : MonoBehaviour
{
    public static Bullet_Pool Instance;

    [Header("Pool Settings")]    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 20;
   
    private Queue<GameObject> bullets = new Queue<GameObject>();    

    private void Awake()
    {
        Instance = this;
        InitializePool();
     
    }

    private void InitializePool() 
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private GameObject CreateNewBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform);
        bullet.SetActive(false);
        bullets.Enqueue(bullet);
        return bullet;
    
    }
    public GameObject GetBullet()
    {
        if (bullets.Count > 0)
        {
            GameObject bullet = bullets.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else 
        { 
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(true);
            return bullet;

        }
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullets.Enqueue(bullet);

    }
}
