using System.Collections;
using UnityEngine;

public class FXEventData
{
    public Vector2 position;
    public Quaternion rotation;
    public string context; // e.g. "Impact", "EnemyDeath", "Spawn"
    public GameObject prefab; // optional direct prefab override
    public AudioClip sound;   // optional SFX
    public Color color;       // optional color tint
    public float intensity;   // for shake or scaling
    public Transform parent; // optional parent transform

    public ScriptableObject config; // reference to any data asset (EnemyData, ProjectileData, etc.)

    public FXEventData(Vector2 pos, string ctx, ScriptableObject src = null, GameObject fx = null, AudioClip sfx = null, Transform parent = null)
    {
        position = pos;
        rotation = Quaternion.identity;
        context = ctx;
        prefab = fx;
        sound = sfx;
        color = Color.white;
        intensity = 1f;
        config = src;
        this.parent = parent;
    }
}