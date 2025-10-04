using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AutomaticShooter : MonoBehaviour
{
    public float currentTime { get; private set; } = 0f;
    
    ProjectilesPattern projectilesPattern;
    Vector2 direction;
    Vector2 offsetPosition;
    bool isShooting = false;
    float currentTimer = 0;
    float totalDelay = 0f;


    public static AutomaticShooter ReloadAutomaticShooter(GameObject go, ProjectilesPattern pattern)
    {
        Debug.Assert(pattern != null, $"{go.name}'s pattern is null");
        Debug.Assert(pattern.projectileWaves.Length > 0, $"{go.name}'s pattern has no waves");

        AutomaticShooter shooter;
        if(go.TryGetComponent<AutomaticShooter>(out shooter))
        {
            Destroy(shooter);
        }
        shooter = go.AddComponent<AutomaticShooter>();
        shooter.projectilesPattern = pattern;
        shooter.resetShooting();
        return shooter;
    }

    private void OnDestroy()
    {
        isShooting = false;
    }

    private void OnDisable()
    {
        isShooting = false;
    }

    private void FixedUpdate()
    {
        if (isShooting)
        {
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= currentTimer)
            {
                int i;
                updateAccumulatedTime(out i);

                if (i == projectilesPattern.projectileWaves.Length - 1)
                {
                    // Shoot the last projectile
                    shootProjectile(i);

                    if (currentTime >= totalDelay)
                        resetShooting();
                }
                else
                {
                    shootProjectile(i);
                }
            }
        }
    }
    public void Shoot(Vector2 direction, Vector2 offsetPosition)
    {
        isShooting = true;
        this.direction = direction;
        this.offsetPosition = offsetPosition;
    }

    private void resetShooting()
    {
        isShooting = false;
        currentTime = 0f;
        currentTimer = 0f;
        totalDelay = 0f;
        foreach (var wave in projectilesPattern.projectileWaves)
            totalDelay += wave.delayAfterWave;
    }

    public void StopShooting()
    {
        resetShooting();
    }

    void shootProjectile(int i)
    {
        ProjectileManager.Instance.Shoot((Vector2)transform.position + offsetPosition, direction, projectilesPattern.projectileWaves[i].projectileConfig);
    }

    void updateAccumulatedTime(out int currentTimerIndex)
    {
        float sum = 0f;
        for (currentTimerIndex = 0; currentTimerIndex < projectilesPattern.projectileWaves.Length; currentTimerIndex++)
        {
            sum += projectilesPattern.projectileWaves[currentTimerIndex].delayAfterWave;
            if (sum > currentTime)
            {
                currentTimer = sum;
                break;
            }
        }
    }
}