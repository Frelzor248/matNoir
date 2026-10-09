using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Header("Mission")]
    [SerializeField] private Mission currentMission;

    private void Start()
    {
        StartMission();
    }

    public void StartMission()
    {
        if (currentMission == null)
        {
            Debug.LogError("Aucune mission assignée !");
            return;
        }

        currentMission.StartMission();
    }

    public void CompleteMission()
    {
        currentMission.CompleteMission();
    }

    public void FailMission()
    {
        currentMission.FailMission();
    }

    public Mission CurrentMission => currentMission;
}