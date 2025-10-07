using Entities.Experimental;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
public class BlobController : MonoBehaviour
{
    private Entity entity;
    private EntitySpawnData spawnData;

    private float jumpCycleDuration = 0.5f;

    private void Awake()
    {
        entity = GetComponent<Entity>();
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
        if(spawnData.moveDirection != Vector2.zero)
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
        float res = SplineUtility.GetNearestPoint(spline, currentPosition, out _, out splineT) ;
        Vector3 direction = spline.EvaluateTangent(splineT);
        return direction;
    }

    private void FixedUpdate()
    {
        // Hopping movement: velocity only along currentDirection, modulated by a jump cycle
        if (entity == null || entity.Rb == null || entity.Config == null) return;

        Vector2 dir = currentDirection();
        if (dir == Vector2.zero) {
            entity.Rb.linearVelocity = Vector2.zero;
            return;
        }

        // Calculate hopping phase (0 to 1)
        float phase = (Time.fixedTime % jumpCycleDuration) / jumpCycleDuration;
        // Use a sine wave for smooth hop, clamp to positive only (no negative velocity)
        float hopFactor = Mathf.Max(0f, Mathf.Sin(phase * Mathf.PI));

        // Project velocity along direction, scaled by moveSpeed and hopFactor
        entity.Rb.linearVelocity = dir.normalized * entity.Config.moveSpeed * hopFactor;
    }


}
