using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private float spawnRadius = 2f;

    [Header("Identification")]
    [SerializeField] private string spawnId = "";

    [Header("Timing")]
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private float spawnDelay = 0.5f;


    public Enemy EnemyPrefab => enemyPrefab;
    public int SpawnCount => spawnCount;
    public float SpawnRadius => spawnRadius;
    public float StartDelay => startDelay;
    public float SpawnDelay => spawnDelay;
    public string SpawnId => spawnId;

    // Utilisé par la carte du Map Maker (MatNoirMapIntegration) pour régler un spawner créé au chargement.
    public void Configure(Enemy prefab, int count, float radius, string id, float delayBeforeStart, float delayBetweenSpawns)
    {
        enemyPrefab = prefab;
        spawnCount = count;
        spawnRadius = radius;
        spawnId = id;
        startDelay = delayBeforeStart;
        spawnDelay = delayBetweenSpawns;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }

}