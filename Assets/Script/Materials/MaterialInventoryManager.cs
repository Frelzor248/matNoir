using System.Collections.Generic;
using UnityEngine;

public class MaterialInventoryManager : MonoBehaviour
{
    public static MaterialInventoryManager Instance { get; private set; }

    private Dictionary<MaterialItem, int> materials =
        new Dictionary<MaterialItem, int>();

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

    public void AddMaterial(MaterialItem material, int quantity)
    {
        if (material == null || quantity <= 0)
            return;

        if (materials.ContainsKey(material))
        {
            materials[material] += quantity;
        }
        else
        {
            materials.Add(material, quantity);
        }

        Debug.Log(
            "Matériau ajouté : " +
            material.MaterialName +
            " x" +
            quantity +
            " | Total : " +
            materials[material]
        );
    }

    public int GetMaterialQuantity(MaterialItem material)
    {
        if (material == null)
            return 0;

        if (materials.TryGetValue(material, out int quantity))
            return quantity;

        return 0;
    }

    public IEnumerable<MaterialItem> GetMaterials()
    {
        return materials.Keys;
    }

    public bool HasMaterial(MaterialItem material, int quantity)
    {
        return GetMaterialQuantity(material) >= quantity;
    }

    public bool RemoveMaterial(MaterialItem material, int quantity)
    {
        if (material == null || quantity <= 0)
            return false;

        if (!materials.TryGetValue(material, out int currentQuantity))
            return false;

        if (currentQuantity < quantity)
            return false;

        materials[material] -= quantity;

        if (materials[material] <= 0)
            materials.Remove(material);

        return true;
    }
}