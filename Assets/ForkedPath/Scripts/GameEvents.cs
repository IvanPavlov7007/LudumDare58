using System.Collections;
using UnityEngine;
using System;
using Pixelplacement;

public class GameEvents : Singleton<GameEvents>
{
    public Action<FXEventData> OnFX;
    public Action<IDamageable, int, Vector2, Vector2> OnDamage;
    public Action<IDamageable, Vector2, Vector2> OnDeath;
    public Action<PlayerEnterTrigger> OnPlayerEnterTrigger;
}