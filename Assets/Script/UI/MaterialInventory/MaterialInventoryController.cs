using UnityEngine;

public class MaterialInventoryController : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;

    private void Start()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null)
            return;

        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    public void OpenInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }
}