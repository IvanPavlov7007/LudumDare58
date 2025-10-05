using System.Collections;
using UnityEngine;
using System;
using Pixelplacement;

public class GameEvents : Singleton<GameEvents>
{
    public Action<Vector2, ProjectileConfig, string, Transform> onFX;
}