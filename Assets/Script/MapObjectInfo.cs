using UnityEngine;

// Composant ajouté par MapLoader à chaque objet instancié :
// donne accès aux propriétés de l'objet (GetComponent<MapObjectInfo>().data.Get("lootTable")).
public class MapObjectInfo : MonoBehaviour
{
    public MapObjectData data;
}
