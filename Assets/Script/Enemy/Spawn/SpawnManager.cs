using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    private void Awake()
    {
        spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
    }

    private void Spawn(SpawnPoint spawnPoint)
    {
        if (spawnPoint.EnemyPrefab == null)
            return;

        StartCoroutine(SpawnRoutine(spawnPoint));
    }

    public void SpawnAllEnemies()
    {
        SpawnById("Spawn_01");
    }

    public void SpawnById(string spawnId)
    {
        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnId != spawnId)
                continue;

            Spawn(spawnPoint);
        }
    }

    private IEnumerator SpawnRoutine(SpawnPoint spawnPoint)
    {
        yield return new WaitForSeconds(spawnPoint.StartDelay);

        for (int i = 0; i < spawnPoint.SpawnCount; i++)
        {
            Vector2 offset =
                Random.insideUnitCircle * spawnPoint.SpawnRadius;

            Instantiate(
                spawnPoint.EnemyPrefab,
                (Vector2)spawnPoint.transform.position + offset,
                spawnPoint.transform.rotation);

            yield return new WaitForSeconds(spawnPoint.SpawnDelay);
        }
    }
}