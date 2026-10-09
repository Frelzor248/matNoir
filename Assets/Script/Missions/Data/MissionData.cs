using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "LB/Missions/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("Informations")]
    [SerializeField] private string missionID;
    [SerializeField] private string missionName;

    [Header("Gameplay")]
    [SerializeField] private int difficulty;

    public string MissionID => missionID;
    public string MissionName => missionName;
    public int Difficulty => difficulty;
}