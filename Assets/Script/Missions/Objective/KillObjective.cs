using UnityEngine;

public class KillObjective : MissionObjective
{
    [Header("Kill Objective")]
    [SerializeField] private int enemiesToKill = 100;

    private int currentKills = 0;

    public override void StartObjective()
    {
        base.StartObjective();

        currentKills = 0;

        EnemyManager.Instance.OnEnemyDeath += RegisterKill;
    }

    private void OnDestroy()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.OnEnemyDeath -= RegisterKill;
        }
    }

    private void RegisterKill(Enemy enemy)
    {
        if (State != MissionObjectiveState.InProgress)
            return;

        currentKills++;

        Debug.Log($"Kills : {currentKills}/{enemiesToKill}");

        if (currentKills >= enemiesToKill)
        {
            CompleteObjective();
        }
    }

    public int CurrentKills => currentKills;

    public int EnemiesToKill => enemiesToKill;
}