// Classe de base de toutes les armes du jeu.

using UnityEngine;

[DisallowMultipleComponent]
public abstract class Weapon : MonoBehaviour
{
    public abstract bool TryShoot();

    public abstract void Reload();
}