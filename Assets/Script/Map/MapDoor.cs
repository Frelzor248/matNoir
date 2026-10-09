using UnityEngine;

// Porte de la carte : bloque le passage tant qu'elle est fermée.
// autoOpen = true : s'ouvre seule à l'approche du joueur ; sinon avec la touche d'interaction.
// Verrouillée : il faut avoir ramassé la clé keyId.
public class MapDoor : MonoBehaviour
{
    int id;
    bool locked, autoOpen = true, opened;
    string keyId;
    KeyCode key = KeyCode.E;
    float range = 1.5f;
    Collider2D blocker;
    bool warnedLocked;

    public bool IsOpened => opened;

    public void Setup(MapObjectData d, float cellSize, KeyCode interactKey, float interactRange)
    {
        id = d.id;
        locked = d.GetBool("locked");
        keyId = d.Get("keyId");
        string auto = d.Get("autoOpen", "true").ToLowerInvariant();
        autoOpen = auto == "true" || auto == "1";
        key = interactKey;
        range = interactRange;

        // Collider à la taille de la porte (la rotation de l'objet s'applique)
        var box = GetComponent<BoxCollider2D>();
        if (box == null) box = gameObject.AddComponent<BoxCollider2D>();
        box.size = new Vector2(d.w, Mathf.Max(d.h, 0.2f)) * cellSize;
        blocker = box;
    }

    void Update()
    {
        if (opened || !MapInteraction.PlayerWithin(transform.position, range)) return;
        if (!autoOpen && !Input.GetKeyDown(key)) return;
        if (!MapKeyRing.CanOpen(locked, keyId, "Porte", id))
        {
            if (!warnedLocked) Debug.Log($"Porte verrouillée : il faut la clé \"{keyId}\".");
            warnedLocked = true;
            return;
        }

        opened = true;
        if (blocker != null) blocker.enabled = false;
        MapInteraction.Dim(gameObject, 0.4f);
    }
}
