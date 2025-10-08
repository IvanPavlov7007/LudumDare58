using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/EntityConfig")]
public class EntityConfig : ScriptableObject
{
    public GameObject entityPrefab;
    public string entityName;
    public int maxHealth = 5;
    public float moveSpeed = 5f;
    public int collisionDamage = 1;
    public GameObject deathFX;
    public AudioClip deathSound;
    public int scoreValue = 1; // maybe food value

    [Header("Invincibility")]
    public bool invincibleAfterHit = false;
    public float invincibilityDuration = 1f;

    [Header("Hit Reaction")]
    public float hitStunDuration = 0f; // 0 = immediate return to Alive

    [Header("Interaction")]
    public LayerMask interactWithAliveLayers;
}