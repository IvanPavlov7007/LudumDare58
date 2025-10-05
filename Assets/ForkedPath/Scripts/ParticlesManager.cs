using System.Collections;
using UnityEngine;
using Pixelplacement;
public class ParticlesManager : Singleton<ParticlesManager>
{
    void OnEnable() => GameEvents.Instance.onFX += HandleFX;
    void OnDisable() => GameEvents.Instance.onFX -= HandleFX;

    void HandleFX(Vector2 position, ProjectileConfig config, string eventName, Transform parent)
    {
        if (config == null) return;
        if (eventName == "Impact" && config.impactFX != null)
        {
            Destroy(Instantiate(config.impactFX, position, Quaternion.identity, parent).gameObject, 0.5f);
        }
        else if (eventName == "Spawn" && config.spawnFX != null)
        {
            Destroy(Instantiate(config.spawnFX, position, Quaternion.identity, parent).gameObject, 0.5f);
        }
    }
}