using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;

    private ArmorItem armorItem;

    public void Setup(ArmorItem armor)
    {
        if (armor == null)
            return;

        armorItem = armor;

        itemName.text = armor.ArmorName;
        itemIcon.sprite = armor.Icon;
    }
}