using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int attack = 10;
    [SerializeField] protected int defense = 5;
    [SerializeField] protected int resistance = 0;
    [SerializeField] protected float moveSpeed = 3f;

    [Header("Separation")]
    [SerializeField, Range(0.1f, 5f)]
    protected float separationRadius = 1.2f;

    [SerializeField, Range(0f, 10f)]
    protected float separationForce = 2f;

    public int CurrentHealth => currentHealth;

    public int MaxHealth => maxHealth;

    public int Attack => attack;

    protected int currentHealth;

    private LootManager lootManager;

    protected virtual void Start()
    {
        EnemyManager.Instance.RegisterSpawn(this);
    }

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        lootManager = FindFirstObjectByType<LootManager>();
    }

    public virtual void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - defense);

        currentHealth -= finalDamage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RegisterDeath(this);
        }

        if (lootManager != null)
        {
            lootManager.DropLoot(transform.position);
        }

        Destroy(gameObject);
    }

    protected Vector2 GetSeparationDirection()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            separationRadius);

        Vector2 separationDirection = Vector2.zero;

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject == gameObject)
                continue;

            Enemy otherEnemy = enemy.GetComponent<Enemy>();

            if (otherEnemy == null)
                continue;

            Vector2 direction =
                (Vector2)(transform.position - enemy.transform.position);

            float distance = direction.magnitude;

            if (distance > 0f)
            {
                separationDirection += direction.normalized / distance;
            }
        }

        return separationDirection * separationForce;
    }
}