using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : IPlayerState
{
    private PlayerMovement playerMovement;
    private Animator animator;

    private Vector2 moveInput;

    internal float crouchSpeed = 1f;

    public CrouchState(PlayerMovement playerMovement)
    {
        this.playerMovement = playerMovement;
    }

    public void EnterState()
    {
        animator = playerMovement.animator;
        animator.SetBool(playerMovement.animCrouchTrans, true);

        PlayerSoundManager.Instance.playerAudioSource.volume /= 3f;
    }

    public void UpdateState()
    {
        moveInput = playerMovement.moveInput;

        playerMovement.SetPlayerSpeed(crouchSpeed);

        animator.SetFloat(playerMovement.animCrouchH, playerMovement.moveInput.x * playerMovement.normalizedValue, 0.1f, Time.deltaTime);
        animator.SetFloat(playerMovement.animCrouchV, playerMovement.moveInput.y * playerMovement.normalizedValue, 0.1f, Time.deltaTime);
    }

    public void ExitState()
    {
        animator.SetBool(playerMovement.animCrouchTrans, false);
        animator = null;

        PlayerSoundManager.Instance.playerAudioSource.volume *= 3f;
    }
}
