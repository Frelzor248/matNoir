using UnityEngine;

public class Mission001 : Mission
{
    [Header("Managers")]
    [SerializeField] private SpawnManager spawnManager;

    [Header("Objectives")]
    [SerializeField] private KillObjective killObjective;

    private void Update()
    {
        if (killObjective.State == MissionObjectiveState.Completed && !IsCompleted)
        {
            CompleteMission();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            spawnManager.SpawnById("Waves_01");

            Debug.Log("Spawn Reinforcement !");
        }
    }

    public override void StartMission()
    {
        base.StartMission();

        spawnManager.SpawnAllEnemies();

        killObjective.StartObjective();

        Debug.Log("Mission 001 démarrée");
    }

    public override void CompleteMission()
    {
        base.CompleteMission();

        Debug.Log("Mission 001 terminée");

        Time.timeScale = 0f;
    }

    public override void FailMission()
    {
        base.FailMission();

        Debug.Log("Mission 001 échouée");
    }
}