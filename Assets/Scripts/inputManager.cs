using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class inputManager : MonoBehaviour
{
[Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool crouch;
    public bool invisible;
    public bool phase;
    public bool pause;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if(cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnCrouchHold(InputValue value)
    {
        CrouchInput(value.isPressed);
    }

    public void OnCrouchToggle(InputValue value)
    {
        CrouchToggleInput(value.isPressed);
    }

    public void OnInvisible(InputValue value)
    {
        InvisibleInput(value.isPressed);
    }

    public void OnPhase(InputValue value)
    {
        PhaseInput(value.isPressed);
    }

    public void OnPause(InputValue value)
    {
        PauseInput(value.isPressed);
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    } 

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }
    
    public void CrouchInput (bool newCrouchState)
    {
        crouch = newCrouchState;
    }

    public void CrouchToggleInput (bool newCrouchToggleState)
    {
        crouch = !crouch;
    }

    public void InvisibleInput (bool newInvisibleState)
    {
        invisible = newInvisibleState;
    }

    public void PhaseInput (bool newPhaseState)
    {
        phase = newPhaseState;
    }

    public void PauseInput (bool newPauseState)
    {
        pause = !pause;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

