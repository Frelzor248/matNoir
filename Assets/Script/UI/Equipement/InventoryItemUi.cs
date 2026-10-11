
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class InventoryItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private Image selectionHighlight;

    private ArmorItem armorItem;
    private InventoryUI inventoryUI;
    private Button itemButton;

    private void Awake()
    {
        itemButton = GetComponent<Button>();
        itemButton.onClick.AddListener(SelectArmor);
    }

    public void Setup(ArmorItem armor, InventoryUI ui)
    {
        if (armor == null)
            return;

        armorItem = armor;
        inventoryUI = ui;

        itemName.text = armor.ArmorName;
        itemIcon.sprite = armor.Icon;

        SetSelected(false);
    }

    private void SelectArmor()
    {
        if (armorItem == null || inventoryUI == null)
            return;

        inventoryUI.SelectArmor(armorItem, this);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.enabled = selected;
    }
}
