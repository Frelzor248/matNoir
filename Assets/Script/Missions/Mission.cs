using UnityEngine;

public abstract class Mission : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private string missionName;

    [Header("Objectives")]
    [SerializeField] protected MissionObjective[] objectives;

    protected bool AreAllObjectivesCompleted()
    {
        foreach (MissionObjective objective in objectives)
        {
            if (!objective.IsCompleted)
            {
                return false;
            }
        }

        return true;
    }

    protected bool isStarted = false;
    protected bool isCompleted = false;
    protected bool isFailed = false;

    public virtual void StartMission()
    {
        isStarted = true;

        Debug.Log($"{missionName} démarrée.");
    }

    public virtual void CompleteMission()
    {
        if (isCompleted)
            return;

        isCompleted = true;

        Debug.Log($"{missionName} terminée.");
    }

    public virtual void FailMission()
    {
        if (isFailed)
            return;

        isFailed = true;

        Debug.Log($"{missionName} échouée.");
    }

    public string MissionName => missionName;

    public bool IsStarted => isStarted;

    public bool IsCompleted => isCompleted;

    public bool IsFailed => isFailed;

}