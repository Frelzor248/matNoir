using UnityEngine;

// Coffre de la carte : s'ouvre avec la touche d'interaction quand le joueur est proche.
// Fait apparaître "tier" objets via LootManager (1 à 5). Verrouillé : il faut la clé keyId.
public class MapChest : MonoBehaviour
{
    int id, drops = 1;
    bool locked, opened;
    string keyId;
    KeyCode key = KeyCode.E;
    float range = 1.5f;
    LootManager lootManager;

    public bool IsOpened => opened;

    public void Setup(MapObjectData d, KeyCode interactKey, float interactRange)
    {
        id = d.id;
        drops = Mathf.Clamp(d.GetInt("tier", 1), 1, 5);
        locked = d.GetBool("locked");
        keyId = d.Get("keyId");
        key = interactKey;
        range = interactRange;
    }

    void Start() => lootManager = FindFirstObjectByType<LootManager>();

    void Update()
    {
        if (opened || !Input.GetKeyDown(key) || !MapInteraction.PlayerWithin(transform.position, range)) return;
        if (!MapKeyRing.CanOpen(locked, keyId, "Coffre", id))
        {
            Debug.Log($"Coffre verrouillé : il faut la clé \"{keyId}\".");
            return;
        }

        opened = true;
        if (lootManager == null)
            Debug.LogWarning("Coffre ouvert mais aucun LootManager dans la scène.");
        else
            for (int i = 0; i < drops; i++)
                lootManager.DropLoot((Vector2)transform.position + Random.insideUnitCircle * 0.6f);
        MapInteraction.Dim(gameObject, 0.45f);
    }
}
