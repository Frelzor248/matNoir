using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    private BulletPool pool;

    private Vector2 direction;

    private int damage;

    private float speed;

    private float lifetime;

    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(BulletPool pool)
    {
        this.pool = pool;
    }

    private void DisableBullet()
    {
        rb.linearVelocity = Vector2.zero;

        if (pool != null)
        {
            pool.ReturnBullet(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void Initialize(
        Vector2 direction,
        int damage,
        float speed,
        float lifetime)
    {
        this.direction = direction.normalized;

        this.damage = damage;

        this.speed = speed;

        this.lifetime = lifetime;

        rb.linearVelocity = Vector2.zero;

        timer = lifetime;

        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;

        timer -= Time.fixedDeltaTime;

        if (timer <= 0f)
        {
            DisableBullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            DisableBullet();
        }
    }
}