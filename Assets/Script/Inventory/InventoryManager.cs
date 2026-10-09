using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<ArmorItem> armorItems = new List<ArmorItem>();

    public IReadOnlyList<ArmorItem> ArmorItems => armorItems;

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

    public void AddArmor(ArmorItem armor)
    {
        if (armor == null)
            return;

        armorItems.Add(armor);

        Debug.Log("Équipement ajouté à l'inventaire : " + armor.ArmorName);
        Debug.Log("Nombre d'équipements dans l'inventaire : " + armorItems.Count);
    }

    public void RemoveArmor(ArmorItem armor)
    {
        if (armor == null)
            return;

        if (armorItems.Remove(armor))
        {
            Debug.Log("Équipement retiré de l'inventaire : " + armor.ArmorName);
        }
    }
}