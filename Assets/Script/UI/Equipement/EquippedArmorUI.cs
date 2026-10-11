
using UnityEngine;
using UnityEngine.UI;

public class EquippedArmorUI : MonoBehaviour
{
    [Header("Armor System")]
    [SerializeField] private ArmorSystem armorSystem;

    [Header("Equipment Icons")]
    [SerializeField] private Image headIcon;
    [SerializeField] private Image chestIcon;
    [SerializeField] private Image legsIcon;
    [SerializeField] private Image feetIcon;

    private void Start()
    {
        RefreshEquipment();
    }


    public void RefreshEquipment()
    {
        if (armorSystem == null)
        {
            armorSystem = FindFirstObjectByType<ArmorSystem>();
        }

        if (armorSystem == null)
        {
            Debug.LogWarning(
                "ArmorSystem introuvable dans les scènes chargées !"
            );
            return;
        }

        UpdateIcon(headIcon, armorSystem.HeadArmor);
        UpdateIcon(chestIcon, armorSystem.ChestArmor);
        UpdateIcon(legsIcon, armorSystem.LegsArmor);
        UpdateIcon(feetIcon, armorSystem.FeetArmor);
    }


    private void UpdateIcon(Image image, ArmorItem armor)
    {
        if (image == null)
            return;

        if (armor != null && armor.Icon != null)
        {
            image.sprite = armor.Icon;
            image.enabled = true;
        }
        else
        {
            image.sprite = null;
            image.enabled = false;
        }
    }
}
