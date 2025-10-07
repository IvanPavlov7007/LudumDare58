using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/EntityConfig")]
public class EntityConfig : ScriptableObject
{
    public GameObject entityPrefab;
    public string entityName;
    public int maxHealth = 5;
    public float moveSpeed = 5f;
    public GameObject deathFX;
    public AudioClip deathSound;
    public int scoreValue = 1; // maybe food value
    [Header("Invincibility")]
    public bool invincibleAfterHit = false;
    public float invincibilityDuration = 1f;
}