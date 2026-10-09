using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private Weapon currentWeapon;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        if (currentWeapon == null)
        {
            Debug.LogError("Aucune arme équipée !");
        }
    }

    private void Update()
    {
        if (playerStats.IsDead)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentWeapon.Reload();
        }
    }
}