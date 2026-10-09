using UnityEngine;

public class MissionUI : MonoBehaviour
{
    [SerializeField] private GameObject missionPanel;

    public void OpenMissionPanel()
    {
        missionPanel.SetActive(true);
    }
}