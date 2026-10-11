
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject equipmentScrollView;
    [SerializeField] private Transform content;
    [SerializeField] private InventoryItemUI inventoryItemPrefab;

    [Header("Equipment Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;

    private ArmorItem selectedArmor;
    private InventoryItemUI selectedItemUI;

    private void Start()
    {
        equipmentScrollView.SetActive(false);

        if (equipButton != null)
            equipButton.onClick.AddListener(EquipSelectedArmor);

        if (unequipButton != null)
            unequipButton.onClick.AddListener(UnequipSelectedArmor);
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

        selectedArmor = null;
        selectedItemUI = null;

        InventoryManager inventoryManager = InventoryManager.Instance;

        Debug.Log("InventoryManager : " + inventoryManager);

        if (inventoryManager != null)
        {
            Debug.Log("Nombre d'armures : " + inventoryManager.ArmorItems.Count);
        }

        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager introuvable !");
            return;
        }

        foreach (ArmorItem armor in inventoryManager.ArmorItems)
        {
            if (armor == null)
            {
                Debug.LogWarning("Une armure de l'inventaire est nulle !");
                continue;
            }

            Debug.Log(
                "Armure trouvée : " + armor.ArmorName +
                " | Slot : " + armor.ArmorSlot +
                " | Catégorie affichée : " + category
            );

            if (armor.ArmorSlot != category)
                continue;

            InventoryItemUI itemUI = Instantiate(inventoryItemPrefab, content);

            Debug.Log("Création de l'élément UI : " + armor.ArmorName);

            itemUI.Setup(armor, this);
        }
    }

    public void SelectArmor(ArmorItem armor, InventoryItemUI itemUI)
    {
        if (armor == null)
            return;

        if (selectedItemUI != null)
            selectedItemUI.SetSelected(false);

        selectedArmor = armor;
        selectedItemUI = itemUI;

        selectedItemUI.SetSelected(true);

        Debug.Log("Équipement sélectionné : " + armor.ArmorName);
    }

    private void EquipSelectedArmor()
    {
        if (selectedArmor == null)
        {
            Debug.Log("Sélectionne d'abord un équipement.");
            return;
        }

        if (EquippedArmorManager.Instance == null)
        {
            Debug.LogWarning("EquippedArmorManager introuvable !");
            return;
        }

        EquippedArmorManager.Instance.SetArmor(selectedArmor);

        Debug.Log("Équipement choisi : " + selectedArmor.ArmorName);
    }

    private void UnequipSelectedArmor()
    {
        if (selectedArmor == null)
        {
            Debug.Log("Sélectionne d'abord un équipement.");
            return;
        }

        if (EquippedArmorManager.Instance == null)
        {
            Debug.LogWarning("EquippedArmorManager introuvable !");
            return;
        }

        EquippedArmorManager.Instance.RemoveArmor(selectedArmor.ArmorSlot);

        Debug.Log("Équipement retiré : " + selectedArmor.ArmorName);
    }

    private void ClearInventory()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }
}
