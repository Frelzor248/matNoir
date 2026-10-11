using UnityEngine;

public class MaterialItem : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private string materialName;
    [SerializeField] private Sprite icon;
    [TextArea]
    [SerializeField] private string description;

    public string MaterialName => materialName;
    public Sprite Icon => icon;
    public string Description => description;
}