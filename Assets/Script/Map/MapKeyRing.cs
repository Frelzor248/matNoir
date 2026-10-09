using System.Collections.Generic;
using UnityEngine;

// Clés ramassées sur la carte (propriété keyId des clés, portes et coffres du Map Maker).
public static class MapKeyRing
{
    static readonly HashSet<string> keys = new HashSet<string>();

    public static void Clear() => keys.Clear();
    public static void Add(string keyId) { if (!string.IsNullOrEmpty(keyId)) keys.Add(keyId); }
    public static bool Has(string keyId) => !string.IsNullOrEmpty(keyId) && keys.Contains(keyId);

    // Verrou ouvert ? (pas verrouillé, ou la bonne clé a été ramassée)
    public static bool CanOpen(bool locked, string keyId, string what, int id)
    {
        if (!locked) return true;
        if (string.IsNullOrEmpty(keyId))
        {
            Debug.LogWarning($"{what} #{id} verrouillé sans keyId dans le Map Maker : ouverture sans clé.");
            return true;
        }
        return Has(keyId);
    }
}

// Petits outils partagés par les objets interactifs de la carte.
public static class MapInteraction
{
    static Transform cachedPlayer;

    public static Transform Player(string tag = "Player")
    {
        if (cachedPlayer == null)
        {
            var go = GameObject.FindGameObjectWithTag(tag);
            cachedPlayer = go != null ? go.transform : null;
        }
        return cachedPlayer;
    }

    public static bool PlayerWithin(Vector3 position, float range)
    {
        Transform p = Player();
        return p != null && ((Vector2)(p.position - position)).sqrMagnitude <= range * range;
    }

    // Assombrit les visuels d'un objet (coffre ouvert, porte ouverte...)
    public static void Dim(GameObject go, float factor)
    {
        foreach (var sr in go.GetComponentsInChildren<SpriteRenderer>())
            sr.color = new Color(sr.color.r * factor, sr.color.g * factor, sr.color.b * factor, sr.color.a);
        foreach (var mf in go.GetComponentsInChildren<MeshFilter>())
        {
            Mesh m = mf.mesh; // copie propre à cet objet
            var colors = m.colors;
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(colors[i].r * factor, colors[i].g * factor, colors[i].b * factor, colors[i].a);
            m.colors = colors;
        }
    }
}
