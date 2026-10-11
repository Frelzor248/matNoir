using UnityEngine;

public class WorldMaterial : MonoBehaviour
{
    [Header("Material")]
    [SerializeField] private MaterialItem materialItem;
    [SerializeField] private int quantity = 1;

    private MaterialInventoryManager inventoryManager;

    public MaterialItem MaterialItem => materialItem;
    public int Quantity => quantity;

    public void Initialize(MaterialItem material, int amount)
    {
        materialItem = material;
        quantity = amount;
    }

    private void Awake()
    {
        inventoryManager = FindFirstObjectByType<MaterialInventoryManager>();
    }

    public void Pickup()
    {
        if (materialItem == null)
            return;

        if (inventoryManager == null)
        {
            Debug.LogWarning("MaterialInventoryManager introuvable !");
            return;
        }

        inventoryManager.AddMaterial(materialItem, quantity);

        Debug.Log(
            "Matériau récupéré : " +
            materialItem.MaterialName +
            " x" +
            quantity
        );

        Destroy(gameObject);
    }
}