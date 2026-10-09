using UnityEngine;

// Ajouté par MapLoader sur chaque zone de sol et chaque mur libre.
// Exemple : réagir à l'entrée dans l'eau avec un trigger de zone
//   void OnTriggerEnter2D(Collider2D c) { var z = c.GetComponent<MapShapeInfo>(); if (z && z.type == "Water") ... }
public class MapShapeInfo : MonoBehaviour
{
    public int id;
    public string kind;      // "Area" (zone de sol) ou "Wall" (mur libre)
    public string type;      // Interior, Exterior, Water, Pit, Empty / Standard, Destructible, LowCover, Glass, Boundary
    public float thickness;  // murs uniquement (0 pour un bloc plein)
}
