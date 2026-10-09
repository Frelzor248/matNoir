using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float healthRegen = 0f;
    [SerializeField] private float healingEfficiency = 1f;

    [Header("Combat")]
    [SerializeField] private int attack = 10;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float reloadSpeed = 2f;
    [SerializeField] private int magazineSize = 20;
    [SerializeField] private float criticalChance = 5f;
    [SerializeField] private float criticalMultiplier = 1.5f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float projectileLifetime = 2f;
    [SerializeField] private int projectilePenetration = 0;

    [Header("Defense")]
    [SerializeField] private int defense = 5;
    [SerializeField] private int resistance = 0;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashDistance = 4f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashInvulnerability = 0.2f;
    [SerializeField] private int dashCharges = 1;

    [Header("Progression")]
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;
    [SerializeField] private int money = 0;
    [SerializeField] private float experienceMultiplier = 1f;
    [SerializeField] private float moneyMultiplier = 1f;
    [SerializeField] private float lootMultiplier = 1f;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        armorSystem = GetComponent<ArmorSystem>();
    }

    #region Gameplay

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - defense);

        currentHealth -= finalDamage;

        Debug.Log($"PV : {currentHealth}/{maxHealth}");

        if(IsDead)
        {
            return;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void Die()
    {
        isDead = true;

        currentHealth = 0;

        Debug.Log("Le joueur est mort");
    }

    public void AddMoney(int amount)
    {
        money += amount;
    }

    public void SpendMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
    }

    public void AddExperience(int amount)
    {
        experience += amount;
    }

    public void AddLevel()
    {
        level++;
    }

    #endregion

    #region Getters

    public bool IsDead => isDead;
    private ArmorSystem armorSystem;

    //==========================Stats Health=======================//
    public int CurrentHealth => currentHealth;
    public float HealthRegen => healthRegen;
    public float HealingEfficiency => healingEfficiency;

    //==========================Stats Weapons=======================//
    public float FireRate => fireRate;
    public float ReloadSpeed => reloadSpeed;
    public int MagazineSize => magazineSize;
    public float CriticalChance => criticalChance;
    public float CriticalMultiplier => criticalMultiplier;
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    public int ProjectilePenetration => projectilePenetration;

    //===========================Stats Armure======================//
    public int MaxHealth =>
    maxHealth + (armorSystem != null ? armorSystem.TotalHealthBonus : 0);
    public int Attack =>
    attack + (armorSystem != null ? armorSystem.TotalAttackBonus : 0);
    public int Defense =>
    defense + (armorSystem != null ? armorSystem.TotalDefenseBonus : 0);
    public int Resistance =>
    resistance + (armorSystem != null ? armorSystem.TotalResistanceBonus : 0);
    public float MoveSpeed =>
    moveSpeed + (armorSystem != null ? armorSystem.TotalMoveSpeedBonus : 0f);

    //===========================Stats Competences======================//
    public float DashDistance => dashDistance;
    public float DashCooldown => dashCooldown;
    public float DashInvulnerability => dashInvulnerability;
    public int DashCharges => dashCharges;

    //===========================Stats Progression======================//
    public int Level => level;
    public int Experience => experience;
    public int Money => money;
    public float ExperienceMultiplier => experienceMultiplier;
    public float MoneyMultiplier => moneyMultiplier;
    public float LootMultiplier => lootMultiplier;


    #endregion
}