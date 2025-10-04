using System.Collections;
using UnityEngine;
[CreateAssetMenu(menuName = "Game/ProjectilesPattern")]
public class ProjectilesPattern : ScriptableObject
{
    public ProjectileWave[] projectileWaves;
}

[System.Serializable]
public class ProjectileWave
{
    public float delayAfterWave = 0.5f;
    public ProjectileConfig projectileConfig;
}