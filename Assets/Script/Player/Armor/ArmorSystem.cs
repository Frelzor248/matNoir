using UnityEngine;

public class ArmorSystem : MonoBehaviour
{
    [Header("Armor Slots")]
    [SerializeField] private ArmorItem headArmor;
    [SerializeField] private ArmorItem chestArmor;
    [SerializeField] private ArmorItem legsArmor;
    [SerializeField] private ArmorItem feetArmor;

    [Header("Available Armor")]
    [SerializeField] private ArmorItem[] headArmors;
    [SerializeField] private ArmorItem[] chestArmors;
    [SerializeField] private ArmorItem[] legsArmors;
    [SerializeField] private ArmorItem[] feetArmors;

    public ArmorItem HeadArmor => headArmor;
    public ArmorItem ChestArmor => chestArmor;
    public ArmorItem LegsArmor => legsArmor;
    public ArmorItem FeetArmor => feetArmor;


    public int TotalHealthBonus =>
       GetArmorHealthBonus(headArmor) +
       GetArmorHealthBonus(chestArmor) +
       GetArmorHealthBonus(legsArmor) +
       GetArmorHealthBonus(feetArmor);

    public int TotalAttackBonus =>
        GetArmorAttackBonus(headArmor) +
        GetArmorAttackBonus(chestArmor) +
        GetArmorAttackBonus(legsArmor) +
        GetArmorAttackBonus(feetArmor);

    public int TotalDefenseBonus =>
        GetArmorDefenseBonus(headArmor) +
        GetArmorDefenseBonus(chestArmor) +
        GetArmorDefenseBonus(legsArmor) +
        GetArmorDefenseBonus(feetArmor);

    public int TotalResistanceBonus =>
        GetArmorResistanceBonus(headArmor) +
        GetArmorResistanceBonus(chestArmor) +
        GetArmorResistanceBonus(legsArmor) +
        GetArmorResistanceBonus(feetArmor);

    public float TotalMoveSpeedBonus =>
        GetArmorMoveSpeedBonus(headArmor) +
        GetArmorMoveSpeedBonus(chestArmor) +
        GetArmorMoveSpeedBonus(legsArmor) +
        GetArmorMoveSpeedBonus(feetArmor);

    private int GetArmorHealthBonus(ArmorItem armor)
    {
        return armor != null ? armor.HealthBonus : 0;
    }

    private int GetArmorAttackBonus(ArmorItem armor)
    {
        return armor != null ? armor.AttackBonus : 0;
    }

    private int GetArmorDefenseBonus(ArmorItem armor)
    {
        return armor != null ? armor.DefenseBonus : 0;
    }

    private int GetArmorResistanceBonus(ArmorItem armor)
    {
        return armor != null ? armor.ResistanceBonus : 0;
    }

    private float GetArmorMoveSpeedBonus(ArmorItem armor)
    {
        return armor != null ? armor.MoveSpeedBonus : 0f;
    }

    public ArmorItem GetRandomArmor(ArmorSlot slot)
    {
        ArmorItem[] armors = null;

        switch (slot)
        {
            case ArmorSlot.Head:
                armors = headArmors;
                break;

            case ArmorSlot.Chest:
                armors = chestArmors;
                break;

            case ArmorSlot.Legs:
                armors = legsArmors;
                break;

            case ArmorSlot.Feet:
                armors = feetArmors;
                break;
        }

        if (armors == null || armors.Length == 0)
        {
            Debug.LogWarning($"Aucune armure disponible pour le slot {slot}.");
            return null;
        }

        int randomIndex = Random.Range(0, armors.Length);

        return armors[randomIndex];
    }


    public void EquipArmor(ArmorItem armor)
    {
        if (armor == null)
            return;

        switch (armor.ArmorSlot)
        {
            case ArmorSlot.Head:
                headArmor = armor;
                break;

            case ArmorSlot.Chest:
                chestArmor = armor;
                break;

            case ArmorSlot.Legs:
                legsArmor = armor;
                break;

            case ArmorSlot.Feet:
                feetArmor = armor;
                break;
        }
    }

    public void UnequipArmor(ArmorSlot slot)
    {
        switch (slot)
        {
            case ArmorSlot.Head:
                headArmor = null;
                break;

            case ArmorSlot.Chest:
                chestArmor = null;
                break;

            case ArmorSlot.Legs:
                legsArmor = null;
                break;

            case ArmorSlot.Feet:
                feetArmor = null;
                break;
        }
    }

    public void TestRandomArmor()
    {
        ArmorItem randomHead = GetRandomArmor(ArmorSlot.Head);
        ArmorItem randomChest = GetRandomArmor(ArmorSlot.Chest);
        ArmorItem randomLegs = GetRandomArmor(ArmorSlot.Legs);
        ArmorItem randomFeet = GetRandomArmor(ArmorSlot.Feet);

        Debug.Log("=== TEST ARMURE ALÉATOIRE ===");

        if (randomHead != null)
            Debug.Log("Head : " + randomHead.ArmorName);

        if (randomChest != null)
            Debug.Log("Chest : " + randomChest.ArmorName);

        if (randomLegs != null)
            Debug.Log("Legs : " + randomLegs.ArmorName);

        if (randomFeet != null)
            Debug.Log("Feet : " + randomFeet.ArmorName);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TestRandomArmor();
        }
    }
}