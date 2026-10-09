using UnityEngine;

public class Goblin : Enemy
{
    [Header("Combat")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("IA")]
    [SerializeField] private float detectionRange = 8f;

    private Transform player;
    private PlayerStats playerStats;
    private float attackTimer;

    protected override void Awake()
    {
        base.Awake();

        GameObject playerObject =
     GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerStats = playerObject.GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(transform.position,
                             player.position);

        if (distance > detectionRange)
        {
            return;
        }

        if (distance > attackRange)
        {
            MoveToPlayer();
        }
        else
        {
            Attack();
        }
    }

    private void MoveToPlayer()
    {
        Vector2 directionToPlayer =
            (player.position - transform.position).normalized;

        Vector2 finalDirection =
            (directionToPlayer + GetSeparationDirection()).normalized;

        transform.position +=
            (Vector3)(finalDirection * moveSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
    }

    private void Attack()
    {
        if (playerStats == null || playerStats.IsDead)
            return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (playerStats != null)
            {
                playerStats.TakeDamage(attack);
            }

            attackTimer = attackCooldown;
        }
    }
}