using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityStateDebugOverlay : MonoBehaviour
{
    [Header("Overlay Settings")]
    public float minimalStateDisplayTime = 0.1f;
    public float overlayAlpha = 0.4f;
    public Sprite overlaySprite; // Assign a simple square or circle sprite in inspector

    private Entity entity;
    private GameObject overlayGO;
    private SpriteRenderer overlayRenderer;
    private Coroutine colorCoroutine;
    private EntityState lastState;
    private float lastStateChangeTime;

    private static readonly Color AliveColor = new Color(0f, 1f, 0f, 0.4f);        // Green
    private static readonly Color HitColor = new Color(1f, 1f, 0f, 0.4f);          // Yellow
    private static readonly Color DeadColor = new Color(1f, 0f, 0f, 0.4f);         // Red
    private static readonly Color FallingColor = new Color(0.5f, 0f, 0.5f, 0.4f);  // Purple
    private static readonly Color InvincibleColor = new Color(0f, 0.5f, 1f, 0.4f); // Blue
    private static readonly Color DeadFallingColor = new Color(1f, 0.4f, 0.7f, 0.4f); // Pink

    void Awake()
    {
        entity = GetComponent<Entity>();
        CreateOverlay();
    }

    void OnEnable()
    {
        entity.StateChanged += OnEntityStateChanged;
        overlayGO.SetActive(true);
        OnEntityStateChanged(entity.CurrentState);
    }

    void OnDisable()
    {
        entity.StateChanged -= OnEntityStateChanged;
        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);
        overlayGO.SetActive(false);
    }

    private void CreateOverlay()
    {
        overlayGO = new GameObject("EntityStateDebugOverlay");
        overlayGO.transform.SetParent(transform, false);
        overlayRenderer = overlayGO.AddComponent<SpriteRenderer>();
        overlayRenderer.sprite = overlaySprite != null ? overlaySprite : GenerateDefaultSprite();
        overlayRenderer.sortingOrder = 1000; // Draw above everything
        overlayRenderer.color = Color.clear;
        overlayGO.SetActive(false);

        // Optionally, scale overlay to fit entity bounds
        var bounds = GetEntityBounds();
        overlayGO.transform.localScale = new Vector3(bounds.size.x, bounds.size.y, 1f);
    }

    private Sprite GenerateDefaultSprite()
    {
        // 16x16 white texture
        Texture2D tex = new Texture2D(16, 16, TextureFormat.ARGB32, false);
        Color[] pixels = new Color[16 * 16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 16f);
    }

    private Bounds GetEntityBounds()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0)
            return new Bounds(transform.position, Vector3.one);
        var bounds = renderers[0].bounds;
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }

    private void OnEntityStateChanged(EntityState state)
    {
        float timeSinceLastChange = Time.time - lastStateChangeTime;
        lastStateChangeTime = Time.time;

        // If minimal display time hasn't passed, delay color change
        if (colorCoroutine != null)
            StopCoroutine(colorCoroutine);

        if (timeSinceLastChange < minimalStateDisplayTime && lastState != state)
        {
            colorCoroutine = StartCoroutine(DelayedColorChange(state, minimalStateDisplayTime - timeSinceLastChange));
        }
        else
        {
            SetOverlayColor(state);
        }

        Debug.Log($"[EntityStateDebugOverlay] {entity.name} state changed to {state}");
        lastState = state;
    }

    private IEnumerator DelayedColorChange(EntityState state, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetOverlayColor(state);
        lastState = state;
    }

    private void SetOverlayColor(EntityState state)
    {
        overlayGO.SetActive(true);
        overlayRenderer.color = GetColorForState(state);
    }

    private Color GetColorForState(EntityState state)
    {
        switch (state)
        {
            case EntityState.Alive: return AliveColor;
            case EntityState.Hit: return HitColor;
            case EntityState.Dead: return DeadColor;
            case EntityState.Falling: return FallingColor;
            case EntityState.Invincible: return InvincibleColor;
            case EntityState.DeadFalling: return DeadFallingColor;
            default: return Color.clear;
        }
    }
}