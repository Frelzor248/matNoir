using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class HUDManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerStats playerStats;

    [Header("Health")]
    [SerializeField] private Image healthFill;

    [Header("Weapon")]
    [SerializeField] private PistolWeapon pistolWeapon;

    [Header("Ammo")]
    [SerializeField] private TMP_Text ammoText;

    private void Update()
    {
        UpdateHealth();
        UpdateAmmo();
    }

    private void UpdateHealth()
    {
        healthFill.fillAmount =
            (float)playerStats.CurrentHealth / playerStats.MaxHealth;
    }

    private void UpdateAmmo()
    {
        ammoText.text =
            $"{pistolWeapon.CurrentAmmo} / {pistolWeapon.MaxAmmo}";
    }
}