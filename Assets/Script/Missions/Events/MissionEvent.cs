using UnityEngine;

public abstract class MissionEvent : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private string eventName;

    private bool hasTriggered = false;

    public virtual void Trigger()
    {
        if (hasTriggered)
            return;

        hasTriggered = true;
    }

    public bool HasTriggered => hasTriggered;

    public string EventName => eventName;
}