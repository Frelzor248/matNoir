using System;
using System.Collections.Generic;
using UnityEngine;

// Branche une carte du Map Maker sur les systèmes du jeu :
//  - PlayerSpawn   -> le joueur (tag "Player") est placé sur le spawn d'ordre le plus petit
//  - EnemySpawner  -> un SpawnPoint configuré (utilisé par SpawnManager / Mission001)
//  - Chest         -> coffre ouvrable avec la touche d'interaction (loot via LootManager)
//  - Key / Door    -> clés ramassables, portes qui bloquent et s'ouvrent (avec la clé si verrouillées)
//  - Teleporter    -> téléportation vers le téléporteur lié (propriété targetId)
//  - solid = true  -> collider sur les éléments de décor (arbres, rochers, caisses...)
// S'exécute juste après MapLoader et avant les autres scripts de la scène.
[DefaultExecutionOrder(-900)]
[RequireComponent(typeof(MapLoader))]
public class MatNoirMapIntegration : MonoBehaviour
{
    [Serializable]
    public class EnemyTypeEntry
    {
        [Tooltip("Valeur de la propriété enemyType dans le Map Maker (ex. Grunt, Sniper)")]
        public string enemyType;
        public Enemy prefab;
    }

    [Header("Joueur")]
    public bool placePlayerOnSpawn = true;
    public string playerTag = "Player";

    [Header("Ennemis")]
    [Tooltip("Ennemi utilisé quand enemyType n'est pas dans la liste")]
    public Enemy defaultEnemy;
    public List<EnemyTypeEntry> enemyTypes = new List<EnemyTypeEntry>();
    [Tooltip("SpawnId donné aux spawners de la carte qui n'ont pas de propriété spawnId " +
             "(Mission001 fait apparaître les spawners \"Spawn_01\" au démarrage)")]
    public string defaultSpawnId = "Spawn_01";
    [Tooltip("Délai entre deux ennemis d'un même spawner, si la propriété spawnDelay est absente")]
    public float defaultSpawnDelay = 0.5f;

    [Header("Interactions")]
    public KeyCode interactKey = KeyCode.E;
    public float interactRange = 1.5f;

    [Header("Décor")]
    [Tooltip("Ajoute un collider aux objets dont la propriété solid vaut true (arbres, rochers, caisses...)")]
    public bool solidDecorColliders = true;

    MapLoader loader;

    void Awake()
    {
        loader = GetComponent<MapLoader>();
        if (!loader.IsBuilt) loader.Build();
        if (!loader.IsBuilt) return;

        MapKeyRing.Clear();
        float cell = loader.Map.cellSize;

        foreach (var pair in loader.objectsById)
        {
            GameObject go = pair.Value;
            MapObjectData d = go.GetComponent<MapObjectInfo>().data;
            switch (d.type)
            {
                case "EnemySpawner": SetupSpawner(go, d); break;
                case "Chest": GetOrAdd<MapChest>(go).Setup(d, interactKey, interactRange); break;
                case "Key": GetOrAdd<MapKeyPickup>(go).Setup(d, 0.6f * cell); break;
                case "Door": GetOrAdd<MapDoor>(go).Setup(d, cell, interactKey, interactRange); break;
                case "Teleporter": GetOrAdd<MapTeleporter>(go).Setup(d, loader, 0.45f * Mathf.Max(d.w, d.h) * cell); break;
            }
            if (solidDecorColliders && d.GetBool("solid") && go.GetComponent<Collider2D>() == null)
                go.AddComponent<BoxCollider2D>().size = new Vector2(d.w, d.h) * cell * 0.86f;
        }

        if (placePlayerOnSpawn) PlacePlayer();
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : go.AddComponent<T>();
    }

    void SetupSpawner(GameObject go, MapObjectData d)
    {
        Enemy prefab = defaultEnemy;
        string wanted = d.Get("enemyType");
        foreach (var e in enemyTypes)
            if (e != null && e.prefab != null && e.enemyType == wanted) { prefab = e.prefab; break; }
        if (prefab == null)
        {
            Debug.LogWarning($"Carte : aucun prefab d'ennemi pour le spawner #{d.id} (enemyType \"{wanted}\"). " +
                             "Renseignez Default Enemy dans MatNoirMapIntegration.");
            return;
        }

        string spawnId = d.Get("spawnId");
        if (string.IsNullOrEmpty(spawnId)) spawnId = defaultSpawnId;

        GetOrAdd<SpawnPoint>(go).Configure(
            prefab,
            Mathf.Max(1, d.GetInt("count", 1)),
            d.GetFloat("radius", 1f) * loader.Map.cellSize,
            spawnId,
            d.GetFloat("startDelay", 0f),
            d.GetFloat("spawnDelay", defaultSpawnDelay));
    }

    void PlacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        GameObject best = null;
        float bestOrder = float.MaxValue;
        foreach (var pair in loader.objectsById)
        {
            MapObjectData d = pair.Value.GetComponent<MapObjectInfo>().data;
            if (d.type != "PlayerSpawn") continue;
            float order = d.GetFloat("order", 0f);
            if (order < bestOrder) { bestOrder = order; best = pair.Value; }
        }
        if (best == null)
        {
            Debug.LogWarning("Carte : aucun PlayerSpawn, le joueur reste à sa position.");
            return;
        }

        Vector3 pos = best.transform.position;
        pos.z = player.transform.position.z;
        player.transform.position = pos;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.position = pos;
    }
}
