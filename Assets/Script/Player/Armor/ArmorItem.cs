using UnityEngine;

public class ArmorItem : MonoBehaviour
{
    [Header("Armor")]
    [SerializeField] private string armorName;
    [SerializeField] private ArmorSlot armorSlot;
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject armorVisual;

    [Header("Stat Modifiers")]
    [SerializeField] private int healthBonus;
    [SerializeField] private int attackBonus;
    [SerializeField] private int defenseBonus;
    [SerializeField] private int resistanceBonus;
    [SerializeField] private float moveSpeedBonus;

    public string ArmorName => armorName;
    public ArmorSlot ArmorSlot => armorSlot;
    public Sprite Icon => icon;
    public GameObject ArmorVisual => armorVisual;

    public int HealthBonus => healthBonus;
    public int AttackBonus => attackBonus;
    public int DefenseBonus => defenseBonus;
    public int ResistanceBonus => resistanceBonus;
    public float MoveSpeedBonus => moveSpeedBonus;
}