using UnityEngine;

public class Chest : MonoBehaviour
{
    private bool isOpened = false;

    public bool IsOpened => isOpened;

    public void Open()
    {
        if (isOpened)
            return;

        isOpened = true;

        LootManager lootManager =
            FindFirstObjectByType<LootManager>();

        if (lootManager == null)
        {
            Debug.LogWarning("LootManager introuvable !");
            return;
        }

        ArmorItem armor = lootManager.GetRandomLoot();

        if (armor == null)
        {
            Debug.LogWarning("Le coffre n'a obtenu aucun équipement.");
            return;
        }

        GameObject itemObject = Instantiate(
            armor.gameObject,
            transform.position,
            Quaternion.identity
        );

        WorldItem worldItem =
            itemObject.GetComponent<WorldItem>();

        if (worldItem == null)
        {
            worldItem = itemObject.AddComponent<WorldItem>();
        }

        worldItem.Initialize(armor);

        Debug.Log(
            "Coffre ouvert ! Équipement obtenu : " +
            armor.ArmorName
        );
    }
}