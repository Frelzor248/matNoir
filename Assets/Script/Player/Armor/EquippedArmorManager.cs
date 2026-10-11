
using UnityEngine;

public class EquippedArmorManager : MonoBehaviour
{
    public static EquippedArmorManager Instance { get; private set; }

    public ArmorItem HeadArmor { get; private set; }
    public ArmorItem ChestArmor { get; private set; }
    public ArmorItem LegsArmor { get; private set; }
    public ArmorItem FeetArmor { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetArmor(ArmorItem armor)
    {
        if (armor == null)
            return;

        switch (armor.ArmorSlot)
        {
            case ArmorSlot.Head:
                HeadArmor = armor;
                break;

            case ArmorSlot.Chest:
                ChestArmor = armor;
                break;

            case ArmorSlot.Legs:
                LegsArmor = armor;
                break;

            case ArmorSlot.Feet:
                FeetArmor = armor;
                break;
        }
    }

    public void RemoveArmor(ArmorSlot slot)
    {
        switch (slot)
        {
            case ArmorSlot.Head:
                HeadArmor = null;
                break;

            case ArmorSlot.Chest:
                ChestArmor = null;
                break;

            case ArmorSlot.Legs:
                LegsArmor = null;
                break;

            case ArmorSlot.Feet:
                FeetArmor = null;
                break;
        }
    }
}
