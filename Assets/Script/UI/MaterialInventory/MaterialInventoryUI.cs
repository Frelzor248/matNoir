using UnityEngine;

public class MaterialInventoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private MaterialEntryUI materialEntryPrefab;

    private void OnEnable()
    {
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        ClearContent();

        if (MaterialInventoryManager.Instance == null)
        {
            Debug.LogWarning("MaterialInventoryManager introuvable !");
            return;
        }

        foreach (MaterialItem material in MaterialInventoryManager.Instance.GetMaterials())
        {
            int quantity =
                MaterialInventoryManager.Instance.GetMaterialQuantity(material);

            MaterialEntryUI entry =
                Instantiate(materialEntryPrefab, content);

            entry.Setup(material, quantity);
        }
    }

    private void ClearContent()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}