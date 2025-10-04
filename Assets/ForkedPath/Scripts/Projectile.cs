using System.Collections;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    public Vector2 velocity;
    public event Action<Projectile,Collider2D> onTriggerEnter;
    public int damage;
    public float lifetime = 10f;

    [SerializeField]
    protected CustomTrigger2D trigger;

    protected Rigidbody2D rb;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trigger.onEnter.AddListener(triggerEntered);
    }

    protected virtual void FixedUpdate()
    {
        transform.right = velocity.normalized;
        rb.linearVelocity = velocity;
    }

    protected virtual void triggerEntered(Collider2D col)
    {
        onTriggerEnter?.Invoke(this, col);
    }

    public virtual void Initialize(Vector2 velocity, LayerMask hitLayers, int damage, float lifeTime)
    {
        this.velocity = velocity;
        setLayerMask(hitLayers.value);
        this.damage = damage;
        this.lifetime = lifeTime;
        gameObject.AddComponent<LimitedLifetime>().lifetime = lifeTime;
    }

    public virtual void setLayerMask(LayerMask hitLayers)
    {
        int layer = Mathf.RoundToInt(Mathf.Log(hitLayers.value, 2));
        SetLayerRecursively(transform, layer);
    }

    private void SetLayerRecursively(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, layer);
        }
    }
}