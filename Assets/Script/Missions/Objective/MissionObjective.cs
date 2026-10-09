using UnityEngine;
public abstract class MissionObjective : MonoBehaviour
{
    [Header("Objective")]
    [SerializeField] private string objectiveName;

    protected MissionObjectiveState state =
        MissionObjectiveState.NotStarted;

    public virtual void StartObjective()
    {
        state = MissionObjectiveState.InProgress;
    }

    public virtual void CompleteObjective()
    {
        if (state == MissionObjectiveState.Completed)
            return;

        state = MissionObjectiveState.Completed;
    }

    public bool IsCompleted =>
        state == MissionObjectiveState.Completed;

    public MissionObjectiveState State => state;

    public string ObjectiveName => objectiveName;
}