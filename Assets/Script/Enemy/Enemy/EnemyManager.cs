using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public event Action<Enemy> OnEnemySpawn;

    public event Action<Enemy> OnEnemyDeath;

    private List<Enemy> aliveEnemies = new();
    private int totalKills;
    public int AliveEnemies => aliveEnemies.Count;
    public int TotalKills => totalKills;

    public static EnemyManager Instance { get; private set; }


    public void RegisterSpawn(Enemy enemy)
    {
        if (!aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Add(enemy);

            OnEnemySpawn?.Invoke(enemy);
        }
    }

    public void RegisterDeath(Enemy enemy)
    {
        if (aliveEnemies.Remove(enemy))
        {
            totalKills++;

            OnEnemyDeath?.Invoke(enemy);
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public List<Enemy> GetAliveEnemies()
    {
        return aliveEnemies;
    }


}


