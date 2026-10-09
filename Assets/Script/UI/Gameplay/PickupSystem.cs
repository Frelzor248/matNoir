using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pickupPrompt;

    [Header("Detection")]
    [SerializeField] private float pickupRange = 1.5f;

    private WorldItem currentItem;

    private void Start()
    {
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    private void Update()
    {
        FindClosestItem();

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
        else
        {
            if (pickupPrompt != null)
                pickupPrompt.SetActive(false);
        }
    }

    private void FindClosestItem()
    {
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(
            transform.position,
            pickupRange
        );

        WorldItem closestItem = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D collider in nearbyObjects)
        {
            WorldItem worldItem = collider.GetComponent<WorldItem>();

            if (worldItem == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                worldItem.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = worldItem;
            }
        }

        currentItem = closestItem;
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