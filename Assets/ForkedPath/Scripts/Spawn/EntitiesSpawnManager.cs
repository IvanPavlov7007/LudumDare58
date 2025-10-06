using Pixelplacement;
using System.Collections;
using UnityEngine;

public class EntitiesSpawnManager : Singleton<EntitiesSpawnManager>
{
    public Entity SpawnEntity(EntityConfig config, Vector2 position)
    {
        if (config == null || config.entityPrefab == null) throw new System.ArgumentNullException("Config or entityPrefab is null");
        GameObject entityInstance = Instantiate(config.entityPrefab, position, Quaternion.identity);
        Entity entity = entityInstance.GetComponent<Entity>();
        if (entity != null)
        {
            entity.Initialize(config);
        }
        return entity;
    }
}