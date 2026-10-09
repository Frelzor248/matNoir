using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 100;

    private Queue<GameObject> pool = new();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.GetComponent<Bullet>().SetPool(this);
            bullet.SetActive(false);

            pool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        if (pool.Count == 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.GetComponent<Bullet>().SetPool(this);
            bullet.SetActive(false);

            return bullet;
        }

        GameObject bulletFromPool = pool.Dequeue();

        bulletFromPool.SetActive(true);

        return bulletFromPool;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);

        pool.Enqueue(bullet);
    }
}