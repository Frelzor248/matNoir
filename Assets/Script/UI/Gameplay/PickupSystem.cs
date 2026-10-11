using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pickupPrompt;
    [SerializeField] private GameObject OpenChestPrompt;

    [Header("Detection")]
    [SerializeField] private float pickupRange = 1.5f;

    private WorldItem currentItem;
    private WorldMaterial currentMaterial;
    private Chest currentChest;

    private void Start()
    {
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
        if (OpenChestPrompt != null)
            OpenChestPrompt.SetActive(false);
    }

    private void Update()
    {
        FindClosestObject();

        // On cache les deux prompts par défaut
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);

        if (OpenChestPrompt != null)
            OpenChestPrompt.SetActive(false);

        // Équipement à récupérer
        if (currentItem != null)
        {
            if (pickupPrompt != null)
                pickupPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentItem.Pickup();
                currentItem = null;
            }
        }
        // Matériau à récupérer
        else if (currentMaterial != null)
        {
            if (pickupPrompt != null)
                pickupPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentMaterial.Pickup();
                currentMaterial = null;
            }
        }
        // Coffre à ouvrir
        else if (currentChest != null)
        {
            if (OpenChestPrompt != null)
                OpenChestPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentChest.Open();
                currentChest = null;
            }
        }
    }

    private void FindClosestObject()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(
            transform.position,
            pickupRange
        );

        WorldItem closestItem = null;
        WorldMaterial closestMaterial = null;
        Chest closestChest = null;

        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in nearbyObjects)
        {
            // Équipement
            WorldItem worldItem = collider.GetComponent<WorldItem>();

            if (worldItem != null)
            {
                float distance = Vector2.Distance(
                    transform.position,
                    worldItem.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = worldItem;
                    closestMaterial = null;
                    closestChest = null;
                }

                continue;
            }

            // Matériau
            WorldMaterial worldMaterial =
                collider.GetComponent<WorldMaterial>();

            if (worldMaterial != null)
            {
                float distance = Vector2.Distance(
                    transform.position,
                    worldMaterial.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = null;
                    closestMaterial = worldMaterial;
                    closestChest = null;
                }

                continue;
            }

            // Coffre
            Chest chest = collider.GetComponent<Chest>();

            if (chest != null && chest.IsOpened)
                continue;

            if (chest != null)
            {
                float distance = Vector2.Distance(
                    transform.position,
                    chest.transform.position
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestItem = null;
                    closestMaterial = null;
                    closestChest = chest;
                }
            }
        }

        currentItem = closestItem;
        currentMaterial = closestMaterial;
        currentChest = closestChest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            pickupRange
        );
    }
}