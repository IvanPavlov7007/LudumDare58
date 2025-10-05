using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomTrigger2D))]
public class PlayerEnterTrigger : MonoBehaviour
{
    CustomTrigger2D trigger2D;

    private void Awake()
    {
        trigger2D = GetComponent<CustomTrigger2D>();
        trigger2D.onEnter.AddListener(OnEnter);
    }

    private void OnEnter(Collider2D collider)
    {
        if(Player.Instance.ColliderIsPlayer(collider))
        {
            GameEvents.Instance.OnPlayerEnterTrigger?.Invoke(this);
        }
    }
}