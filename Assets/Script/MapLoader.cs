// Import des cartes "Map Maker 2D" dans Unity (format v1 et v2 : modes Tuiles, Vectoriel et Hybride).
// 1. Copiez MapLoader.cs, MapObjectInfo.cs et MapShapeInfo.cs dans Assets/Scripts/,
//    et le .json de la carte dans Assets/ (il devient un TextAsset).
// 2. Ajoutez le composant MapLoader à un GameObject vide, glissez le .json dans "Map Json".
// 3. Associez un prefab à chaque type d'objet (Chest, Door, EnemySpawner...) dans "Prefabs".
// 4. Lancez la scène (ou clic droit sur le composant > "Construire la carte").
//
// Ce qui est construit :
//  - Grille (Tuiles / Hybride) : Tilemap de sol (optionnelle) + un collider par case de mur, à la forme exacte.
//  - Formes libres (Vectoriel / Hybride) : un mesh par zone de sol (+ trigger optionnel) et des colliders
//    exacts pour chaque mur (trait épais ou bloc plein).
//  - Objets : le prefab associé à chaque type, avec MapObjectInfo pour lire ses propriétés.
//  - Visuels provisoires (option "Draw Placeholders") : sol, murs et objets colorés comme dans l'éditeur,
//    tant que les vrais graphismes ne sont pas prêts.
//  - FloorTypeAt(position) : le type de sol à une position du monde (zones libres puis grille).
//
// La carte est construite dans Awake, avant les autres scripts (DefaultExecutionOrder) : les gestionnaires
// qui cherchent des objets dans la scène au démarrage (ex. SpawnManager) les trouvent déjà.
//
// Repère : dans l'éditeur, y va vers le BAS ; ici y va vers le HAUT.
//          position Unity = (x * cellSize, (height - y) * cellSize).

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

[Serializable]
public class MapProperty
{
    public string key;
    public string value;
}

[Serializable]
public class MapObjectData
{
    public int id;
    public string type;
    public float x, y, w, h, rotation;
    public MapProperty[] props;

    public string Get(string key, string fallback = "")
    {
        if (props != null)
            foreach (var p in props)
                if (p.key == key) return p.value;
        return fallback;
    }

    public float GetFloat(string key, float fallback = 0f)
    {
        return float.TryParse(Get(key).Replace(',', '.'), System.Globalization.NumberStyles.Float,
                              System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : fallback;
    }

    public int GetInt(string key, int fallback = 0) => Mathf.RoundToInt(GetFloat(key, fallback));

    public bool GetBool(string key)
    {
        string v = Get(key).ToLowerInvariant();
        return v == "true" || v == "1" || v == "oui" || v == "yes";
    }
}

// Zone de sol libre. points = [x0, y0, x1, y1, ...] en cases de l'éditeur.
[Serializable]
public class MapAreaData
{
    public int id;
    public string type;
    public float[] points;
}

// Mur libre : ligne brisée d'épaisseur "thickness". closed = fait le tour ; closed + filled = bloc plein.
[Serializable]
public class MapWallData
{
    public int id;
    public string type;
    public float thickness = 0.5f;
    public bool closed;
    public bool filled;
    public float[] points;

    public bool IsSolid => closed && filled && points != null && points.Length >= 6;
}

[Serializable]
public class MapFile
{
    public string format;
    public int version;
    public string mode; // "Tiles", "Vector", "Hybrid" (absent en v1 = Tiles)
    public string name;
    public int width, height;
    public float cellSize = 1f;
    public int pixelsPerUnit = 32;
    public int nextId;
    public string[] floorTypes, wallTypes, shapes;
    public int[] floor, floorShape, wall, wallShape;
    public MapAreaData[] areas;
    public MapWallData[] walls;
    public MapObjectData[] objects;

    public bool HasGrid => floor != null && floor.Length == width * height && wall != null && wall.Length == width * height;

    int Index(int x, int y) => y * width + x;
    public string FloorAt(int x, int y) => floorTypes[floor[Index(x, y)]];
    public string FloorShapeAt(int x, int y) => shapes[floorShape[Index(x, y)]];
    public string WallAt(int x, int y) => wallTypes[wall[Index(x, y)]];
    public string WallShapeAt(int x, int y) => shapes[wallShape[Index(x, y)]];

    // Position (en cases de l'éditeur) -> position monde Unity, et inverse
    public Vector2 ToWorld(float cx, float cy) => new Vector2(cx * cellSize, (height - cy) * cellSize);
    public Vector2 ToEditor(Vector2 world) => new Vector2(world.x / cellSize, height - world.y / cellSize);
    // Case de l'éditeur -> case de Tilemap
    public Vector3Int ToTile(int cx, int cy) => new Vector3Int(cx, height - 1 - cy, 0);

    public Vector2[] WorldPoints(float[] flat)
    {
        if (flat == null) return new Vector2[0];
        var pts = new Vector2[flat.Length / 2];
        for (int i = 0; i < pts.Length; i++) pts[i] = ToWorld(flat[2 * i], flat[2 * i + 1]);
        return pts;
    }
}

[DefaultExecutionOrder(-1000)]
public class MapLoader : MonoBehaviour
{
    [Serializable]
    public class PrefabEntry
    {
        public string type;
        public GameObject prefab;
    }

    [Header("Données")]
    public TextAsset mapJson;
    public List<PrefabEntry> prefabs = new List<PrefabEntry>();
    [Tooltip("Construit la carte au lancement de la scène (dans Awake, avant les autres scripts)")]
    public bool buildOnStart = true;

    [Header("Sol en cases (optionnel)")]
    public Tilemap floorTilemap;
    public TileBase interiorTile, exteriorTile, waterTile, pitTile;

    [Header("Sol en formes libres")]
    public bool buildAreaMeshes = true;
    [Tooltip("Matériau des zones (sinon : Placeholder Material). La couleur dépend du type.")]
    public Material areaMaterial;
    public Color interiorColor = new Color(0.70f, 0.60f, 0.46f);
    public Color exteriorColor = new Color(0.32f, 0.54f, 0.28f);
    public Color waterColor = new Color(0.22f, 0.42f, 0.74f);
    public Color pitColor = new Color(0.04f, 0.04f, 0.05f);
    public Color emptyColor = new Color(0f, 0f, 0f, 0f);
    [Tooltip("Ajoute un PolygonCollider2D en trigger à chaque zone (détection d'entrée dans l'eau, un bâtiment...)")]
    public bool areaTriggers = false;
    [Tooltip("Ordre d'affichage de la première zone ; les suivantes sont au-dessus (+1 chacune)")]
    public int areaSortingOrder = -100;

    [Header("Murs")]
    public bool buildWallColliders = true;
    public bool includeBoundaries = true;

    [Header("Visuels provisoires")]
    [Tooltip("Dessine le sol (si aucune Tilemap), les murs et les objets sans prefab avec des formes colorées")]
    public bool drawPlaceholders = true;
    [Tooltip("Matériau non éclairé utilisant la couleur des sommets (ex. Sprite-Unlit-Default)")]
    public Material placeholderMaterial;
    public Color wallColor = new Color(0.44f, 0.45f, 0.52f);
    public Color destructibleWallColor = new Color(0.71f, 0.44f, 0.24f);
    public Color lowCoverColor = new Color(0.59f, 0.55f, 0.38f);
    public Color glassColor = new Color(0.59f, 0.82f, 0.94f, 0.6f);
    [Tooltip("Ordre d'affichage : sol en cases, murs, objets (le joueur et les ennemis doivent être au-dessus)")]
    public int gridFloorSortingOrder = -101, wallSortingOrder = -3, objectSortingOrder = -1;

    public MapFile Map { get; private set; }
    public bool IsBuilt { get; private set; }
    public event Action<MapLoader> OnBuilt;
    public readonly Dictionary<int, GameObject> objectsById = new Dictionary<int, GameObject>();

    Material runtimeMaterial;

    void Awake()
    {
        if (buildOnStart && !IsBuilt) Build();
    }

    [ContextMenu("Construire la carte")]
    public void Build()
    {
        if (mapJson == null) { Debug.LogError("MapLoader : aucun fichier de carte"); return; }
        Map = JsonUtility.FromJson<MapFile>(mapJson.text);
        Clear();

        if (Map.HasGrid && floorTilemap != null) BuildGridFloor();
        if (Map.HasGrid && floorTilemap == null && drawPlaceholders) BuildGridFloorPlaceholder();
        if (buildAreaMeshes) BuildAreas();
        if (Map.HasGrid) BuildGridWalls();
        BuildFreeWalls();
        BuildObjects();

        IsBuilt = true;
        Debug.Log($"Carte \"{Map.name}\" ({(string.IsNullOrEmpty(Map.mode) ? "Tiles" : Map.mode)}) chargée : " +
                  $"{Map.width}x{Map.height}, {Map.areas?.Length ?? 0} zones, {Map.walls?.Length ?? 0} murs libres, " +
                  $"{Map.objects?.Length ?? 0} objets");
        OnBuilt?.Invoke(this);
    }

    [ContextMenu("Vider")]
    public void Clear()
    {
        objectsById.Clear();
        IsBuilt = false;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            if (Application.isPlaying)
            {
                // Désactivé tout de suite : Destroy n'agit qu'en fin d'image, et les autres scripts
                // ne doivent pas trouver les anciens objets (ex. une carte construite dans l'éditeur).
                child.SetActive(false);
                Destroy(child);
            }
            else DestroyImmediate(child);
        }
        if (floorTilemap != null) floorTilemap.ClearAllTiles();
    }

    // ------------------------------------------------------------------------------------------
    // Requêtes de jeu
    // ------------------------------------------------------------------------------------------

    // Type de sol à une position du monde : "Interior", "Exterior", "Water", "Pit" ou "Empty".
    public string FloorTypeAt(Vector2 worldPos)
    {
        if (Map == null) return "Empty";
        Vector2 p = Map.ToEditor(worldPos - (Vector2)transform.position);
        if (Map.areas != null)
            for (int i = Map.areas.Length - 1; i >= 0; i--) // la zone la plus haute d'abord
            {
                var a = Map.areas[i];
                if (a.points != null && a.points.Length >= 6 && PointInPolygon(p, a.points))
                    return a.type;
            }
        if (Map.HasGrid)
        {
            int cx = Mathf.FloorToInt(p.x), cy = Mathf.FloorToInt(p.y);
            if (cx >= 0 && cy >= 0 && cx < Map.width && cy < Map.height)
            {
                string t = Map.FloorAt(cx, cy);
                if (t != "Empty" && PointInList(new Vector2(p.x - cx, p.y - cy), UnitShape(Map.FloorShapeAt(cx, cy))))
                    return t;
            }
        }
        return "Empty";
    }

    public bool IsInterior(Vector2 worldPos) => FloorTypeAt(worldPos) == "Interior";

    // Centre de la carte, en coordonnées monde
    public Vector2 WorldCenter => (Vector2)transform.position +
                                  (Map == null ? Vector2.zero : new Vector2(Map.width, Map.height) * Map.cellSize * 0.5f);

    // ------------------------------------------------------------------------------------------
    // Grille
    // ------------------------------------------------------------------------------------------
    void BuildGridFloor()
    {
        for (int y = 0; y < Map.height; y++)
            for (int x = 0; x < Map.width; x++)
            {
                TileBase tile = null;
                switch (Map.FloorAt(x, y))
                {
                    case "Interior": tile = interiorTile; break;
                    case "Exterior": tile = exteriorTile; break;
                    case "Water":    tile = waterTile; break;
                    case "Pit":      tile = pitTile; break;
                }
                // Note : les formes de sol (arrondis, triangles...) sont dans Map.FloorShapeAt(x, y)
                // si vous voulez choisir une tuile différente selon la forme.
                if (tile != null) floorTilemap.SetTile(Map.ToTile(x, y), tile);
            }
    }

    void BuildGridFloorPlaceholder()
    {
        var mb = new MeshBuilder();
        for (int y = 0; y < Map.height; y++)
            for (int x = 0; x < Map.width; x++)
            {
                string t = Map.FloorAt(x, y);
                if (t == "Empty") continue;
                mb.AddPolygon(CellPolygon(x, y, Map.FloorShapeAt(x, y)), ColorFor(t));
            }
        if (!mb.IsEmpty) CreateMeshObject("Sol (cases)", transform, mb, gridFloorSortingOrder);
    }

    void BuildGridWalls()
    {
        var parent = new GameObject("Murs (cases)").transform;
        parent.SetParent(transform, false);
        float s = Map.cellSize;
        var visual = new MeshBuilder();

        for (int y = 0; y < Map.height; y++)
            for (int x = 0; x < Map.width; x++)
            {
                string type = Map.WallAt(x, y);
                if (type == "None") continue;
                if (type == "Boundary" && !includeBoundaries) continue;
                string shape = Map.WallShapeAt(x, y);

                if (drawPlaceholders && type != "Boundary")
                    visual.AddPolygon(CellPolygon(x, y, shape), WallColorFor(type));
                if (!buildWallColliders) continue;

                var go = new GameObject($"{type}_{x}_{y}");
                go.transform.SetParent(parent, false);
                go.transform.localPosition = Map.ToWorld(x + 0.5f, y + 0.5f);
                if (shape == "Full")
                    go.AddComponent<BoxCollider2D>().size = new Vector2(s, s);
                else
                    go.AddComponent<PolygonCollider2D>().points = ShapePoints(shape, s);
                // Astuce : pour beaucoup de murs, ajoutez un Rigidbody2D (Static) + CompositeCollider2D
                // sur le parent et activez "Used By Composite" sur les colliders.
            }
        if (!visual.IsEmpty) CreateMeshObject("Visuel", parent, visual, wallSortingOrder);
    }

    // Polygone d'une case (forme comprise), en coordonnées locales de la carte
    Vector2[] CellPolygon(int x, int y, string shape)
    {
        var unit = UnitShape(shape);
        var pts = new Vector2[unit.Count];
        for (int i = 0; i < unit.Count; i++) pts[i] = Map.ToWorld(x + unit[i].x, y + unit[i].y);
        return pts;
    }

    // ------------------------------------------------------------------------------------------
    // Formes libres
    // ------------------------------------------------------------------------------------------
    Color ColorFor(string floorType)
    {
        switch (floorType)
        {
            case "Interior": return interiorColor;
            case "Exterior": return exteriorColor;
            case "Water":    return waterColor;
            case "Pit":      return pitColor;
            default:         return emptyColor;
        }
    }

    Color WallColorFor(string wallType)
    {
        switch (wallType)
        {
            case "Destructible": return destructibleWallColor;
            case "LowCover":     return lowCoverColor;
            case "Glass":        return glassColor;
            default:             return wallColor;
        }
    }

    Material PlaceholderMaterial()
    {
        if (placeholderMaterial != null) return placeholderMaterial;
        if (runtimeMaterial == null)
        {
            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            runtimeMaterial = new Material(shader) { name = "MapPlaceholder (auto)" };
        }
        return runtimeMaterial;
    }

    GameObject CreateMeshObject(string name, Transform parent, MeshBuilder mb, int sortingOrder, Material mat = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<MeshFilter>().sharedMesh = mb.ToMesh(name);
        var mr = go.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat != null ? mat : PlaceholderMaterial();
        mr.sortingOrder = sortingOrder;
        return go;
    }

    void BuildAreas()
    {
        if (Map.areas == null || Map.areas.Length == 0) return;
        var parent = new GameObject("Zones").transform;
        parent.SetParent(transform, false);
        var material = areaMaterial != null ? areaMaterial : PlaceholderMaterial();

        for (int i = 0; i < Map.areas.Length; i++)
        {
            var a = Map.areas[i];
            Vector2[] pts = Map.WorldPoints(a.points);
            if (pts.Length < 3) continue;

            var mb = new MeshBuilder();
            mb.AddPolygon(pts, ColorFor(a.type));
            var go = CreateMeshObject($"Zone_{a.type}_{a.id}", parent, mb, areaSortingOrder + i, material);
            var info = go.AddComponent<MapShapeInfo>();
            info.id = a.id; info.kind = "Area"; info.type = a.type;

            if (areaTriggers)
            {
                var col = go.AddComponent<PolygonCollider2D>();
                col.points = pts;
                col.isTrigger = true;
            }
        }
    }

    void BuildFreeWalls()
    {
        if (Map.walls == null || Map.walls.Length == 0) return;
        var parent = new GameObject("Murs (libres)").transform;
        parent.SetParent(transform, false);
        var visual = new MeshBuilder();

        foreach (var w in Map.walls)
        {
            if (w.type == "None") continue;
            if (w.type == "Boundary" && !includeBoundaries) continue;
            Vector2[] pts = Clean(Map.WorldPoints(w.points), w.closed);
            if (pts.Length < 2) continue;
            bool draw = drawPlaceholders && w.type != "Boundary";

            Vector2[] left = null, right = null;
            if (!w.IsSolid) StrokeOffsets(pts, w.closed, w.thickness * Map.cellSize * 0.5f, out left, out right);
            int segs = w.closed ? pts.Length : pts.Length - 1;

            if (draw)
            {
                if (w.IsSolid) visual.AddPolygon(pts, WallColorFor(w.type));
                else
                    for (int i = 0; i < segs; i++)
                    {
                        int j = (i + 1) % pts.Length;
                        visual.AddPolygon(new[] { left[i], left[j], right[j], right[i] }, WallColorFor(w.type));
                    }
            }
            if (!buildWallColliders) continue;

            var go = new GameObject($"Mur_{w.type}_{w.id}");
            go.transform.SetParent(parent, false);
            var info = go.AddComponent<MapShapeInfo>();
            info.id = w.id; info.kind = "Wall"; info.type = w.type; info.thickness = w.IsSolid ? 0f : w.thickness;

            var col = go.AddComponent<PolygonCollider2D>();
            if (w.IsSolid)
            {
                col.points = pts; // bloc plein : le polygone lui-même
                continue;
            }
            // Trait épais : un quadrilatère par segment, jonctions en onglet (identique à l'éditeur)
            col.pathCount = segs;
            for (int i = 0; i < segs; i++)
            {
                int j = (i + 1) % pts.Length;
                col.SetPath(i, new[] { left[i], left[j], right[j], right[i] });
            }
        }
        if (!visual.IsEmpty) CreateMeshObject("Visuel", parent, visual, wallSortingOrder + 1);
    }

    // ------------------------------------------------------------------------------------------
    // Objets
    // ------------------------------------------------------------------------------------------
    void BuildObjects()
    {
        if (Map.objects == null) return;
        var parent = new GameObject("Objets").transform;
        parent.SetParent(transform, false);

        var lookup = new Dictionary<string, GameObject>();
        foreach (var p in prefabs)
            if (p != null && !string.IsNullOrEmpty(p.type) && p.prefab != null) lookup[p.type] = p.prefab;

        foreach (var o in Map.objects)
        {
            bool hasPrefab = lookup.TryGetValue(o.type, out var prefab);
            GameObject go = hasPrefab ? Instantiate(prefab, parent) : new GameObject();
            go.name = $"{o.type}_{o.id}";
            if (go.transform.parent != parent) go.transform.SetParent(parent, false);
            go.transform.localPosition = Map.ToWorld(o.x, o.y);
            go.transform.localRotation = Quaternion.Euler(0f, 0f, -o.rotation); // éditeur : sens horaire
            go.AddComponent<MapObjectInfo>().data = o;
            objectsById[o.id] = go;

            // Pas de prefab : un rectangle de la couleur de l'éditeur, à la taille de l'objet
            if (!hasPrefab && drawPlaceholders && o.type != "TriggerZone" && o.type != "CameraZone" && o.type != "PatrolPoint")
            {
                float s = Map.cellSize;
                Vector2 half = new Vector2(o.w, o.h) * s * 0.43f;
                var mb = new MeshBuilder();
                mb.AddPolygon(new[] { new Vector2(-half.x, -half.y), new Vector2(-half.x, half.y),
                                      new Vector2(half.x, half.y), new Vector2(half.x, -half.y) }, ObjectColor(o.type));
                CreateMeshObject("Visuel", go.transform, mb, objectSortingOrder);
            }
        }
        // Exemple : récupérer la destination d'un téléporteur
        //   var info = teleporter.GetComponent<MapObjectInfo>();
        //   var dest = loader.objectsById[int.Parse(info.data.Get("targetId"))];
    }

    // Couleurs des objets, identiques à l'éditeur
    static readonly Dictionary<string, Color32> objectColors = new Dictionary<string, Color32>
    {
        {"PlayerSpawn", new Color32(80, 210, 120, 255)}, {"EnemySpawner", new Color32(230, 70, 70, 255)},
        {"ItemSpawner", new Color32(235, 190, 60, 255)}, {"BossSpawn", new Color32(160, 30, 50, 255)},
        {"Chest", new Color32(170, 110, 50, 255)}, {"Door", new Color32(210, 170, 110, 255)},
        {"Teleporter", new Color32(70, 220, 230, 255)}, {"Key", new Color32(250, 230, 90, 255)},
        {"Workbench", new Color32(150, 100, 210, 255)}, {"BoostSpeed", new Color32(90, 200, 255, 255)},
        {"BoostHeal", new Color32(110, 230, 110, 255)}, {"BoostDamage", new Color32(255, 110, 60, 255)},
        {"BoostShield", new Color32(140, 150, 255, 255)}, {"BoostAmmo", new Color32(220, 220, 200, 255)},
        {"ResWood", new Color32(140, 95, 55, 255)}, {"ResMetal", new Color32(170, 180, 195, 255)},
        {"ResScrap", new Color32(150, 120, 100, 255)}, {"ResFiber", new Color32(120, 190, 90, 255)},
        {"ResCrystal", new Color32(200, 120, 255, 255)}, {"Crate", new Color32(160, 130, 90, 255)},
        {"Barrel", new Color32(200, 60, 40, 255)}, {"Rock", new Color32(130, 130, 135, 255)},
        {"Tree", new Color32(40, 120, 60, 255)}, {"Bush", new Color32(100, 170, 80, 255)},
        {"Pillar", new Color32(185, 180, 170, 255)}, {"Table", new Color32(150, 110, 80, 255)},
    };

    static Color ObjectColor(string type) => objectColors.TryGetValue(type, out var c) ? (Color)c : Color.white;

    // ------------------------------------------------------------------------------------------
    // Géométrie (mêmes algorithmes que l'éditeur)
    // ------------------------------------------------------------------------------------------
    class MeshBuilder
    {
        readonly List<Vector3> verts = new List<Vector3>();
        readonly List<Color> colors = new List<Color>();
        readonly List<int> tris = new List<int>();
        public bool IsEmpty => tris.Count == 0;

        public void AddPolygon(Vector2[] pts, Color c)
        {
            int start = verts.Count;
            foreach (var p in pts) { verts.Add(p); colors.Add(c); }
            foreach (int i in Triangulate(pts)) tris.Add(start + i);
        }

        public Mesh ToMesh(string name)
        {
            var m = new Mesh { name = name };
            if (verts.Count > 65000) m.indexFormat = IndexFormat.UInt32;
            m.SetVertices(verts);
            m.SetColors(colors);
            m.SetTriangles(tris, 0);
            m.RecalculateBounds();
            return m;
        }
    }

    static float Cross(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;

    static float SignedArea(IList<Vector2> p)
    {
        float s = 0f;
        for (int i = 0; i < p.Count; i++) s += Cross(p[i], p[(i + 1) % p.Count]);
        return s * 0.5f;
    }

    static Vector2[] Clean(Vector2[] pts, bool closed)
    {
        var o = new List<Vector2>();
        foreach (var p in pts)
            if (o.Count == 0 || (o[o.Count - 1] - p).sqrMagnitude > 1e-8f) o.Add(p);
        if (closed && o.Count > 1 && (o[0] - o[o.Count - 1]).sqrMagnitude <= 1e-8f) o.RemoveAt(o.Count - 1);
        return o.ToArray();
    }

    static bool PointInPolygon(Vector2 p, float[] flat)
    {
        bool inside = false;
        int n = flat.Length / 2;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            float xi = flat[2 * i], yi = flat[2 * i + 1], xj = flat[2 * j], yj = flat[2 * j + 1];
            if ((yi > p.y) != (yj > p.y) && p.x < (xj - xi) * (p.y - yi) / (yj - yi) + xi) inside = !inside;
        }
        return inside;
    }

    static bool PointInList(Vector2 p, List<Vector2> poly)
    {
        var flat = new float[poly.Count * 2];
        for (int i = 0; i < poly.Count; i++) { flat[2 * i] = poly[i].x; flat[2 * i + 1] = poly[i].y; }
        return PointInPolygon(p, flat);
    }

    // Triangulation par "oreilles" d'un polygone simple, triangles dans le sens horaire (face visible en 2D).
    static int[] Triangulate(Vector2[] poly)
    {
        var tris = new List<int>();
        int n = poly.Length;
        if (n < 3) return tris.ToArray();
        var V = new List<int>();
        bool positive = SignedArea(poly) > 0f;
        for (int i = 0; i < n; i++) V.Add(positive ? i : n - 1 - i);

        int guard = 2 * V.Count, v = V.Count - 1;
        while (V.Count > 2)
        {
            if (guard-- <= 0)
            {
                for (int i = 1; i + 1 < V.Count; i++) { tris.Add(V[0]); tris.Add(V[i + 1]); tris.Add(V[i]); }
                break;
            }
            int u = v; if (u >= V.Count) u = 0;
            v = u + 1; if (v >= V.Count) v = 0;
            int w = v + 1; if (w >= V.Count) w = 0;
            Vector2 A = poly[V[u]], B = poly[V[v]], C = poly[V[w]];
            if (Cross(B - A, C - A) <= 1e-9f) continue;
            bool ear = true;
            for (int k = 0; k < V.Count && ear; k++)
            {
                if (k == u || k == v || k == w) continue;
                Vector2 P = poly[V[k]];
                if (P == A || P == B || P == C) continue;
                if (Cross(B - A, P - A) >= 0f && Cross(C - B, P - B) >= 0f && Cross(A - C, P - C) >= 0f) ear = false;
            }
            if (!ear) continue;
            tris.Add(V[u]); tris.Add(V[w]); tris.Add(V[v]); // inversé : sens horaire pour Unity
            V.RemoveAt(v);
            guard = 2 * V.Count;
        }
        return tris.ToArray();
    }

    static void StrokeOffsets(Vector2[] pts, bool closed, float hw, out Vector2[] left, out Vector2[] right)
    {
        int n = pts.Length;
        left = new Vector2[n];
        right = new Vector2[n];
        for (int i = 0; i < n; i++)
        {
            bool hasPrev = closed || i > 0, hasNext = closed || i + 1 < n;
            Vector2 dPrev = hasPrev ? (pts[i] - pts[(i + n - 1) % n]).normalized : Vector2.zero;
            Vector2 dNext = hasNext ? (pts[(i + 1) % n] - pts[i]).normalized : Vector2.zero;
            Vector2 nPrev = new Vector2(-dPrev.y, dPrev.x), nNext = new Vector2(-dNext.y, dNext.x);
            Vector2 off;
            if (!hasPrev) off = nNext * hw;
            else if (!hasNext) off = nPrev * hw;
            else
            {
                Vector2 m = nPrev + nNext;
                if (m.magnitude < 1e-4f) off = nNext * hw;
                else
                {
                    m.Normalize();
                    float c = Mathf.Max(Vector2.Dot(m, nNext), 0.25f); // limite d'onglet (x4)
                    off = m * (hw / c);
                }
            }
            left[i] = pts[i] + off;
            right[i] = pts[i] - off;
        }
    }

    // Polygones des formes de case (identiques à l'éditeur), centrés sur la case, y vers le haut.
    public static Vector2[] ShapePoints(string shape, float size)
    {
        var unit = UnitShape(shape);
        var pts = new Vector2[unit.Count];
        for (int i = 0; i < unit.Count; i++)
            pts[i] = new Vector2((unit[i].x - 0.5f) * size, (0.5f - unit[i].y) * size);
        return pts;
    }

    // Coordonnées unitaires 0..1 avec y vers le BAS (comme dans l'éditeur).
    static List<Vector2> UnitShape(string shape)
    {
        const int arc = 12;
        var p = new List<Vector2>();
        bool mx = shape.EndsWith("TR") || shape.EndsWith("BR");
        bool my = shape.EndsWith("BL") || shape.EndsWith("BR");

        if (shape.StartsWith("Tri"))
        {
            p.Add(new Vector2(0, 0)); p.Add(new Vector2(1, 0)); p.Add(new Vector2(0, 1));
        }
        else if (shape.StartsWith("Round"))
        {
            p.Add(new Vector2(1, 1));
            for (int k = 0; k <= arc; k++)
            {
                float a = Mathf.PI + Mathf.PI * 0.5f * k / arc;
                p.Add(new Vector2(1 + Mathf.Cos(a), 1 + Mathf.Sin(a)));
            }
        }
        else if (shape.StartsWith("Inner"))
        {
            p.Add(new Vector2(1, 0)); p.Add(new Vector2(1, 1)); p.Add(new Vector2(0, 1));
            for (int k = 1; k < arc; k++)
            {
                float a = Mathf.PI * 0.5f * (1f - (float)k / arc);
                p.Add(new Vector2(Mathf.Cos(a), Mathf.Sin(a)));
            }
        }
        else
        {
            mx = my = false;
            switch (shape)
            {
                case "HalfTop":    p.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, .5f), new Vector2(0, .5f) }); break;
                case "HalfBottom": p.AddRange(new[] { new Vector2(0, .5f), new Vector2(1, .5f), new Vector2(1, 1), new Vector2(0, 1) }); break;
                case "HalfLeft":   p.AddRange(new[] { new Vector2(0, 0), new Vector2(.5f, 0), new Vector2(.5f, 1), new Vector2(0, 1) }); break;
                case "HalfRight":  p.AddRange(new[] { new Vector2(.5f, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(.5f, 1) }); break;
                case "Circle":
                    for (int k = 0; k < 32; k++)
                    {
                        float a = 2f * Mathf.PI * k / 32f;
                        p.Add(new Vector2(.5f + .5f * Mathf.Cos(a), .5f + .5f * Mathf.Sin(a)));
                    }
                    break;
                default: p.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) }); break;
            }
        }
        for (int i = 0; i < p.Count; i++)
            p[i] = new Vector2(mx ? 1 - p[i].x : p[i].x, my ? 1 - p[i].y : p[i].y);
        return p;
    }
}
