using UnityEngine;

// Téléporteur de la carte : envoie le joueur sur le téléporteur lié (propriété targetId).
// Le téléporteur d'arrivée ne renvoie pas le joueur tant qu'il n'est pas ressorti de sa zone.
public class MapTeleporter : MonoBehaviour
{
    MapLoader loader;
    int targetId = -1;
    float radius = 0.45f, cooldown = 1f;
    float readyAt;
    bool playerInside;

    public void Setup(MapObjectData d, MapLoader mapLoader, float triggerRadius)
    {
        loader = mapLoader;
        if (!int.TryParse(d.Get("targetId"), out targetId)) targetId = -1;
        radius = triggerRadius;
        cooldown = d.GetFloat("cooldown", 1f);
    }

    void Update()
    {
        bool inside = MapInteraction.PlayerWithin(transform.position, radius);
        bool entered = inside && !playerInside;
        playerInside = inside;
        if (!entered || Time.time < readyAt || loader == null) return;

        if (!loader.objectsById.TryGetValue(targetId, out GameObject target) || target == null)
        {
            Debug.LogWarning($"Téléporteur {name} : destination #{targetId} introuvable.");
            return;
        }

        Transform player = MapInteraction.Player();
        Vector3 pos = target.transform.position;
        pos.z = player.position.z;
        player.position = pos;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.position = pos;

        // À l'arrivée, le joueur est déjà "dans" la destination : elle ne se déclenche pas tout de suite
        var dest = target.GetComponent<MapTeleporter>();
        if (dest != null)
        {
            dest.playerInside = true;
            dest.readyAt = Time.time + cooldown;
        }
        readyAt = Time.time + cooldown;
    }
}
