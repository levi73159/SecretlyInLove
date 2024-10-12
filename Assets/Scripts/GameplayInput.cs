using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(PlayerInput))]
public class GameplayInput : MonoBehaviour
{
    private PlayerInput _playerInput;

    public Vector2 Move { get; set; }
    public Vector2 Look { get; set; }
    public bool Jump { get; set; }
    public bool Sprint { get; set; }
    public bool Crouch { get; set; }
    public bool Attack { get; set; }

    [Header("Movement Settings")]
    public bool analogMovement;

    public string CurrentControlScheme => _playerInput.currentControlScheme;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputValue value)
    {
        Move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        Look = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        Jump = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        Sprint = value.isPressed;
    }

    public void OnCrouch(InputValue value)
    {
        Crouch = value.isPressed;
    }

    public void OnAttack(InputValue value) {
        Attack = value.isPressed;
    }
}
