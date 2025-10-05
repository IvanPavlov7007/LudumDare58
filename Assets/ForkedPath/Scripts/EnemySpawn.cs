using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public EntityConfig[] enemyConfigs;
    public float spawnDelay = 1f;

    [SerializeField]
    protected PlayerEnterTrigger playerTrigger;

    protected virtual void OnEnable()
    {
        GameEvents.Instance.OnPlayerEnterTrigger += onPlayerEnterTriggerEnter;
        spawning = null;
    }

    protected virtual void OnDisable()
    {
        GameEvents.Instance.OnPlayerEnterTrigger -= onPlayerEnterTriggerEnter;
    }

    Coroutine spawning = null;

    protected virtual void onPlayerEnterTriggerEnter(PlayerEnterTrigger trigger)
    {
        if(trigger == playerTrigger && spawning == null)
        {
            spawning = StartCoroutine(spawnEnemies());
        }
    }

    protected virtual Entity spawnEnemy(EntityConfig config, Vector2 position)
    {
        return EntitiesSpawnManager.Instance.SpawnEntity(config, transform.position);
    }

    protected virtual IEnumerator spawnEnemies()
    {
        foreach(EntityConfig config in enemyConfigs)
        {
            spawnEnemy(config, transform.position);
            yield return new WaitForSeconds(spawnDelay);
        }
        gameObject.SetActive(false);
    }
}