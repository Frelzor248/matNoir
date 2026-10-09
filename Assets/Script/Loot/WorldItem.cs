using UnityEngine;

public class WorldItem : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private ArmorItem armorItem;

    private InventoryManager inventoryManager;

    public ArmorItem ArmorItem => armorItem;

    public void Initialize(ArmorItem armor)
    {
        armorItem = armor;
    }

    private void Awake()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void Pickup()
    {
        if (armorItem == null)
            return;

        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager introuvable !");
            return;
        }

        inventoryManager.AddArmor(armorItem);

        Debug.Log("Objet récupéré : " + armorItem.ArmorName);

        Destroy(gameObject);
    }
}