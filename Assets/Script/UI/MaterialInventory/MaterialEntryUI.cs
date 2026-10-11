using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialEntryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text materialName;
    [SerializeField] private TMP_Text quantity;

    public void Setup(MaterialItem material, int amount)
    {
        if (material == null)
            return;

        icon.sprite = material.Icon;
        materialName.text = material.MaterialName;
        quantity.text = "x" + amount;
    }
}