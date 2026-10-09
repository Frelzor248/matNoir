using UnityEngine;

public class LootManager : MonoBehaviour
{
    [Header("Loot")]
    [SerializeField] private ArmorItem[] headArmors;
    [SerializeField] private ArmorItem[] chestArmors;
    [SerializeField] private ArmorItem[] legsArmors;
    [SerializeField] private ArmorItem[] feetArmors;

    public ArmorItem GetRandomArmor(LootCategory category)
    {
        ArmorItem[] armors = null;

        switch (category)
        {
            case LootCategory.Head:
                armors = headArmors;
                break;

            case LootCategory.Chest:
                armors = chestArmors;
                break;

            case LootCategory.Legs:
                armors = legsArmors;
                break;

            case LootCategory.Feet:
                armors = feetArmors;
                break;
        }

        if (armors == null || armors.Length == 0)
        {
            Debug.LogWarning($"Aucun loot disponible pour la catégorie {category}.");
            return null;
        }

        int randomIndex = Random.Range(0, armors.Length);

        return armors[randomIndex];
    }

    public LootCategory GetRandomCategory()
    {
        int randomIndex = Random.Range(1, 5);

        return (LootCategory)randomIndex;
    }

    public ArmorItem GetRandomLoot()
    {
        LootCategory category = GetRandomCategory();

        Debug.Log("Catégorie de loot choisie : " + category);

        ArmorItem armor = GetRandomArmor(category);

        if (armor == null)
            return null;

        Debug.Log("Loot obtenu : " + armor.ArmorName);

        return armor;
    }

    public void DropLoot(Vector2 position)
    {
        LootCategory category = GetRandomCategory();

        if (category == LootCategory.Weapon)
        {
            Debug.Log("Weapon non disponible pour le moment.");
            return;
        }

        ArmorItem armor = GetRandomArmor(category);

        if (armor == null)
            return;

        GameObject itemObject = Instantiate(
         armor.gameObject,
         position,
         Quaternion.identity
        );

        WorldItem worldItem = itemObject.GetComponent<WorldItem>();

        if (worldItem == null)
        {
            worldItem = itemObject.AddComponent<WorldItem>();
        }

        worldItem.Initialize(armor);
    }
}