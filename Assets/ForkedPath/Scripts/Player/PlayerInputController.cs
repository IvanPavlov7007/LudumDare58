using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Pixelplacement;

public class PlayerInputController : Singleton<PlayerInputController>
{
    public Vector2 moveInput = Vector2.zero;
    public bool attacking = false;

    public void OnAttack(InputValue inputValue)
    {
        attacking = inputValue.isPressed;
    }

    public void OnMove(InputValue inputValue)
    {
        moveInput = inputValue.Get<Vector2>();
    }
}
