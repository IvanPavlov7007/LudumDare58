using Entities.Experimental;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class BlobController : MonoBehaviour
{
    private Entity entity;
    private EntitySpawnData spawnData;

    private readonly float jumpCycleDuration = 0.5f;
    private readonly float jumpHeight = 0.3f;
    private readonly float bodyReturnSpeed = 8f;

    [SerializeField]
    private Transform bodyTransform; // Assign in inspector or find in Awake

    private Vector3 bodyInitialLocalPos;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        if (bodyTransform == null)
        {
            // Try to find the first child as body
            if (transform.childCount > 0)
                bodyTransform = transform.GetChild(0);
        }
        if (bodyTransform != null)
            bodyInitialLocalPos = bodyTransform.localPosition;
    }

    private void Start()
    {
        spawnData = GetComponent<EntitySpawnData>();
    }

    private Vector2 currentDirection()
    {
        if (spawnData == null)
            return default;

        if (spawnData.splineContainer != null)
        {
            return closestSplineDirection();
        }
        if (spawnData.moveDirection != Vector2.zero)
        {
            return spawnData.moveDirection.normalized;
        }
        return Vector2.zero;
    }

    private Vector2 closestSplineDirection()
    {
        if (spawnData == null || spawnData.splineContainer == null) return Vector2.zero;
        var spline = spawnData.splineContainer.Spline;
        Vector3 currentPosition = spawnData.splineContainer.transform.InverseTransformPoint(transform.position);
        float splineT;
        float res = SplineUtility.GetNearestPoint(spline, currentPosition, out _, out splineT);
        Vector3 direction = spline.EvaluateTangent(splineT);
        return direction;
    }

    private void FixedUpdate()
    {
        if (entity == null || entity.Rb == null || entity.Config == null || bodyTransform == null) return;

        // Only animate/move if not dead or falling
        bool canJump = entity.CurrentState == EntityState.Alive
            || entity.CurrentState == EntityState.Hit
            || entity.CurrentState == EntityState.Invincible;

        Vector2 dir = currentDirection();

        // Calculate hopping phase (0 to 1)
        float phase = (Time.fixedTime % jumpCycleDuration) / jumpCycleDuration;
        // Use a sine wave for smooth hop, clamp to positive only (no negative velocity)
        float hopFactor = Mathf.Max(0f, Mathf.Sin(phase * Mathf.PI));

        if (canJump && dir != Vector2.zero)
        {
            // Project velocity along direction, scaled by moveSpeed and hopFactor
            entity.Rb.linearVelocity = dir.normalized * entity.Config.moveSpeed * hopFactor;

            // Animate body jump
            Vector3 jumpOffset = new Vector3(0, hopFactor * jumpHeight, 0);
            bodyTransform.localPosition = bodyInitialLocalPos + jumpOffset;
        }
        else
        {
            // Stop movement
            entity.Rb.linearVelocity = Vector2.zero;

            // Smoothly return body to initial position
            bodyTransform.localPosition = Vector3.Lerp(
                bodyTransform.localPosition,
                bodyInitialLocalPos,
                Time.fixedDeltaTime * bodyReturnSpeed
            );
        }
    }
}
