using GunSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StandState : IPlayerState
{
    private PlayerMovement playerMovement;
    private PlayerController playerController;
    private GunController gunController;
    private Animator animator;

    private InputAction sprintInput;

    private Vector2 moveInput;

    internal float walkFWSpeed = 2f, walkFWSoundTime = 0.5f;
    internal float walkBWSpeed = 1.5f, walkBWSoundTime = 0.6f;
    internal float sprintSpeed = 5f, sprintSoundTime = 0.3f;
    private float sprintValue = 0f;
    private float playerSpeed;


    public StandState(PlayerMovement playerMovement)
    {
        this.playerMovement = playerMovement;
        gunController = GunManager.Instance.GetCurrentGun().GetComponent<GunController>();
        sprintInput = playerMovement.input.Movement.Sprint;
        this.playerController = playerMovement.GetComponent<PlayerController>();
    }

    public void EnterState()
    {
        animator = playerMovement.animator;

        sprintInput.Enable();
        sprintInput.performed += OnSprintPress;
        sprintInput.canceled += OnSprintRelease;
    }

    private void OnSprintPress(InputAction.CallbackContext ctx)
    {
        if (playerMovement.moveInput.y > 0 && playerController.SP > 0)
        {
            playerMovement.isSprinting = true;
            sprintValue = 0.5f;
            playerMovement.SetPlayerSpeed(sprintSpeed);
            playerMovement.SetSoundTime(sprintSoundTime);
        }
    }

    private void OnSprintRelease(InputAction.CallbackContext ctx)
    {
        StopSprint();
    }

    private void StopSprint()
    {
        playerMovement.isSprinting = false;
        sprintValue = 0f;
    }

    public void UpdateState()
    {
        moveInput = playerMovement.moveInput;
        playerSpeed = playerMovement.playerSpeed;

        if (moveInput.y >= 0 && playerSpeed != walkFWSpeed && !playerMovement.isSprinting)
        {
            playerMovement.SetPlayerSpeed(walkFWSpeed);
            playerMovement.SetSoundTime(walkFWSoundTime);
        }
        else if (moveInput.y < 0 && playerSpeed != walkBWSpeed)
        {
            playerMovement.SetPlayerSpeed(walkBWSpeed);
            playerMovement.SetSoundTime(walkBWSoundTime);
        }

        if (playerController.SP <= 0)
        {
            StopSprint();
        }

        animator.SetFloat(playerMovement.animStandH, moveInput.y >= 0 ? moveInput.x * playerMovement.normalizedValue : moveInput.x * playerMovement.normalizedValue * -1, 0.1f, Time.deltaTime);
        animator.SetFloat(playerMovement.animStandV, moveInput.y * playerMovement.normalizedValue + sprintValue, 0.1f, Time.deltaTime);
    }

    public void ExitState()
    {
        animator = null;
        sprintInput.Disable();

        sprintInput.performed -= OnSprintPress;
        sprintInput.canceled -= OnSprintRelease;

    }
}
