using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Menu "Map Maker" de l'éditeur Unity : ajoute une carte du Map Maker à la scène ouverte, en un clic.
public static class MapMakerMenu
{
    const string DemoMap = "Assets/maps/demo.json";
    const string DefaultEnemyPrefab = "Assets/Prefabs/Enemy/Goblin.prefab";
    const string PlaceholderMaterialPath = "Assets/maps/MapPlaceholder.mat";

    [MenuItem("Map Maker/Ajouter la carte demo à la scène", false, 0)]
    static void AddDemo() => AddMap(DemoMap);

    [MenuItem("Map Maker/Ajouter une carte à la scène...", false, 1)]
    static void AddOther()
    {
        string abs = EditorUtility.OpenFilePanel("Carte du Map Maker (.json)", "Assets/maps", "json");
        if (string.IsNullOrEmpty(abs)) return;
        string root = Path.GetFullPath(Path.Combine(Application.dataPath, "..")).Replace('\\', '/');
        abs = Path.GetFullPath(abs).Replace('\\', '/');
        if (!abs.StartsWith(root + "/Assets/"))
        {
            EditorUtility.DisplayDialog("Map Maker", "Copiez d'abord le fichier .json dans le dossier Assets du projet " +
                                                     "(par exemple Assets/maps), puis recommencez.", "OK");
            return;
        }
        AddMap(abs.Substring(root.Length + 1));
    }

    [MenuItem("Map Maker/Reconstruire les cartes de la scène", false, 20)]
    static void RebuildAll()
    {
        var loaders = Object.FindObjectsByType<MapLoader>(FindObjectsSortMode.None);
        foreach (var loader in loaders)
        {
            loader.Build();
            EditorSceneManager.MarkSceneDirty(loader.gameObject.scene);
        }
        Debug.Log($"Map Maker : {loaders.Length} carte(s) reconstruite(s).");
    }

    static void AddMap(string assetPath)
    {
        AssetDatabase.ImportAsset(assetPath);
        var json = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
        if (json == null)
        {
            EditorUtility.DisplayDialog("Map Maker", $"Carte introuvable : {assetPath}", "OK");
            return;
        }

        // 1) L'objet "Carte" avec le chargeur et l'intégration au jeu
        var go = new GameObject("Carte - " + Path.GetFileNameWithoutExtension(assetPath));
        Undo.RegisterCreatedObjectUndo(go, "Ajouter la carte");
        var loader = go.AddComponent<MapLoader>();
        loader.mapJson = json;
        loader.placeholderMaterial = GetOrCreatePlaceholderMaterial();
        var integration = go.AddComponent<MatNoirMapIntegration>();
        integration.defaultEnemy = AssetDatabase.LoadAssetAtPath<Enemy>(DefaultEnemyPrefab);
        if (integration.defaultEnemy == null)
            Debug.LogWarning($"Map Maker : {DefaultEnemyPrefab} introuvable, renseignez Default Enemy à la main.");

        // 2) L'ancien sol de test (grand sprite "Ground") cacherait la carte : on le désactive
        var ground = GameObject.Find("Ground");
        if (ground != null && ground.GetComponent<SpriteRenderer>() != null)
        {
            Undo.RecordObject(ground, "Désactiver Ground");
            ground.SetActive(false);
            Debug.Log("Map Maker : l'objet \"Ground\" a été désactivé (réactivable dans la Hierarchy).");
        }

        // 3) Les SpawnPoints placés à la main se retrouveraient au milieu de la carte
        var olds = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        if (olds.Length > 0 && EditorUtility.DisplayDialog("Map Maker",
                $"La scène contient déjà {olds.Length} SpawnPoint(s) placé(s) à la main.\n\n" +
                "Les désactiver pour n'utiliser que les spawners de la carte ?\n" +
                "(Ils restent dans la scène, réactivables à tout moment.)",
                "Désactiver", "Garder"))
        {
            foreach (var sp in olds)
            {
                Undo.RecordObject(sp.gameObject, "Désactiver les SpawnPoints");
                sp.gameObject.SetActive(false);
            }
        }

        // 4) Aperçu dans l'éditeur (la carte est de toute façon reconstruite au lancement)
        loader.Build();
        EditorSceneManager.MarkSceneDirty(go.scene);
        Selection.activeGameObject = go;
        if (SceneView.lastActiveSceneView != null && loader.Map != null)
        {
            var size = new Vector3(loader.Map.width, loader.Map.height, 1f) * loader.Map.cellSize;
            SceneView.lastActiveSceneView.Frame(new Bounds(loader.WorldCenter, size), false);
        }
        Debug.Log($"Map Maker : carte \"{json.name}\" ajoutée. Enregistrez la scène (Ctrl+S) puis lancez Play.");
    }

    static Material GetOrCreatePlaceholderMaterial()
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(PlaceholderMaterialPath);
        if (mat != null) return mat;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        if (shader == null) return null; // MapLoader se débrouillera à l'exécution

        mat = new Material(shader) { name = "MapPlaceholder" };
        Directory.CreateDirectory(Path.GetDirectoryName(PlaceholderMaterialPath));
        AssetDatabase.CreateAsset(mat, PlaceholderMaterialPath);
        AssetDatabase.SaveAssets();
        return mat;
    }
}
