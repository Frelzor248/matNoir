using UnityEngine;

// Clé de la carte : ramassée automatiquement quand le joueur passe dessus.
public class MapKeyPickup : MonoBehaviour
{
    string keyId;
    float radius = 0.6f;

    public void Setup(MapObjectData d, float pickupRadius)
    {
        keyId = d.Get("keyId");
        radius = pickupRadius;
    }

    void Update()
    {
        if (!MapInteraction.PlayerWithin(transform.position, radius)) return;
        MapKeyRing.Add(keyId);
        Debug.Log($"Clé ramassée : {keyId}");
        Destroy(gameObject);
    }
}
