using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class EntityStateDebugOverlayManager : Pixelplacement.Singleton<EntityStateDebugOverlayManager>
{
    [Tooltip("Attach overlays to all current entities when enabling.")]
    public bool attachToExistingOnEnable = true;

    private bool _enabled;
    private EntitiesSpawnManager _boundSpawnMgr;

    public static void Toggle() => SetOverlaysEnabled(!Instance._enabled);

    public static void SetOverlaysEnabled(bool enabled)
    {
        var mgr = Instance;
        if (mgr._enabled == enabled) return;
        mgr._enabled = enabled;

        if (enabled)
        {
            mgr.AttachToSpawnManager();
            if (mgr.attachToExistingOnEnable) mgr.AttachToAllExisting();
            Debug.Log("[EntityStateDebugOverlay] ENABLED");
        }
        else
        {
            mgr.DetachFromSpawnManager();
            mgr.RemoveFromAll();
            Debug.Log("[EntityStateDebugOverlay] DISABLED");
        }
    }

    protected override void OnRegistration()
    {
        base.OnRegistration();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        DetachFromSpawnManager();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_enabled)
        {
            AttachToSpawnManager();
            AttachToAllExisting();
        }
    }

    private void AttachToSpawnManager()
    {
        var mgr = EntitiesSpawnManager.Instance;
        if (mgr == null || _boundSpawnMgr == mgr) return;

        if (_boundSpawnMgr != null)
            _boundSpawnMgr.OnEntitySpawned -= OnEntitySpawned;

        _boundSpawnMgr = mgr;
        _boundSpawnMgr.OnEntitySpawned += OnEntitySpawned;
    }

    private void DetachFromSpawnManager()
    {
        if (_boundSpawnMgr != null)
        {
            _boundSpawnMgr.OnEntitySpawned -= OnEntitySpawned;
            _boundSpawnMgr = null;
        }
    }

    private void OnEntitySpawned(Entity entity)
    {
        if (!_enabled || entity == null) return;
        if (entity.GetComponent<EntityStateDebugOverlay>() == null)
            entity.gameObject.AddComponent<EntityStateDebugOverlay>();
    }

    private void AttachToAllExisting()
    {
        var entities = FindObjectsByType<Entity>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i].GetComponent<EntityStateDebugOverlay>() == null)
                entities[i].gameObject.AddComponent<EntityStateDebugOverlay>();
        }
    }

    private void RemoveFromAll()
    {
        var overlays = FindObjectsByType<EntityStateDebugOverlay>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < overlays.Length; i++)
        {
            if (overlays[i] != null)
                Destroy(overlays[i]);
        }
    }
}