using System.Collections;
using UnityEngine;
using Pixelplacement;

public class Player : Singleton<Player>
{
    public bool ColliderIsPlayer(Collider2D collider)
    {
        return collider.gameObject.layer == LayerMask.NameToLayer("Player");
    }
}