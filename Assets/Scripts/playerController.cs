using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    public float cameraSpeed;
    private float cameraRotation;
    private Camera cam;

    CharacterController characterController;
    private inputManager input;
    private PlayerInput playerInput;
    public float Gravity = -15.0f;
    private float cameraPitch;
    private float rotationSpeed;

    private float speed;
    public float MoveSpeed;
    private float TrueMoveSpeed;
	public float SprintSpeed;
    private float TrueSprintSpeed;
    public float JumpHeight;
    private float verticalSpeed;
    private bool Crouching;
 
    private float jumpTimer;
    public float jumpCooldown = 0.1f;
    private float terminalVelocity = 53.0f;
    
    void Start()
    {
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
        input = GetComponent<inputManager>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Enable();
        TrueSprintSpeed = SprintSpeed;
        TrueMoveSpeed = MoveSpeed;
    }
 
    void Update()
    {
        //camera movement
        cameraPitch -= input.look.y * cameraSpeed;
        cameraRotation = input.look.x * cameraSpeed;
 
        cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
 
        cam.transform.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);

        transform.Rotate(Vector3.up * cameraRotation);

        //player movement

        float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;
        if (input.move == Vector2.zero) targetSpeed = 0.0f;
        float currentSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f; //if using analog stick, scale with input. Else, magnitude is 1f

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
        if (input.move != Vector2.zero)
        {
            inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
        }
        characterController.Move(inputDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, verticalSpeed, 0.0f) * Time.deltaTime); //move player

        //jumping and gravity

        if (characterController.isGrounded) //check if character is on ground
        {
            if (verticalSpeed < 0.0f) //Keeps velocity consistent while grounded
            {
                verticalSpeed = -2f;
            }

            if (input.jump && jumpTimer <= 0.0f) //if input pressed and cooldown is settled
            {
                verticalSpeed = Mathf.Sqrt(JumpHeight * -2f * Gravity); //jump
            }

            if (jumpTimer >= 0.0f)
            {
                jumpTimer -= Time.deltaTime; //Counts cooldown timer down while greater than 0
            }
        }
        else
        {
            jumpTimer = jumpCooldown; //Keeps cooldown consistent while in the air
            input.jump = false; //do not jump if grounded
        }

        if (verticalSpeed < terminalVelocity) //Applies gravity while fall speed is under max fall speed
        {
            verticalSpeed += Gravity * Time.deltaTime;
        }

        //crouching

        if (input.crouch)
        {
            if(Crouching == false)
            {
                transform.localScale = transform.localScale - new Vector3(0, 0.5f, 0);
                Crouching = true;    
                MoveSpeed = MoveSpeed * 0.5f;
                SprintSpeed = SprintSpeed * 0.5f;
            }
        }
        else if (Crouching == true)
        {
            transform.localScale = transform.localScale + new Vector3(0, 0.5f, 0);
            Crouching = false;
            MoveSpeed = TrueMoveSpeed;
            SprintSpeed = TrueSprintSpeed;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
	}
}
