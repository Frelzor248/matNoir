using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject equipmentScrollView;
    [SerializeField] private Transform content;
    [SerializeField] private InventoryItemUI inventoryItemPrefab;

    private void Start()
    {
        // Aucune catégorie sélectionnée au lancement
        equipmentScrollView.SetActive(false);
    }

    public void ShowHead()
    {
        ShowCategory(ArmorSlot.Head);
    }

    public void ShowChest()
    {
        ShowCategory(ArmorSlot.Chest);
    }

    public void ShowLegs()
    {
        ShowCategory(ArmorSlot.Legs);
    }

    public void ShowFeet()
    {
        ShowCategory(ArmorSlot.Feet);
    }

    private void ShowCategory(ArmorSlot category)
    {
        equipmentScrollView.SetActive(true);

        ClearInventory();

        InventoryManager inventoryManager = InventoryManager.Instance;

        if (inventoryManager == null)
            return;

        foreach (ArmorItem armor in inventoryManager.ArmorItems)
        {
            if (armor.ArmorSlot != category)
                continue;

            InventoryItemUI itemUI =
                Instantiate(inventoryItemPrefab, content);

            itemUI.Setup(armor);
        }
    }

    private void ClearInventory()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}