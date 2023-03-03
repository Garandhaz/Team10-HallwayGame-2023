using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
	public float SprintSpeed;
    public float SpeedChangeRate = 10.0f;
    private float verticalSpeed;
 
    
    void Start()
    {
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
        input = GetComponent<inputManager>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Enable();
    }
 
    void Update()
    {
        //camera movement
        cameraPitch -= input.look.y * cameraSpeed;
        cameraRotation = input.look.x * cameraSpeed;
 
        cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
 
        cam.transform.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);

        transform.Rotate(Vector3.up * cameraRotation);

        // player movement

        float targetSpeed = input.sprint ? SprintSpeed : MoveSpeed;
        if (input.move == Vector2.zero) targetSpeed = 0.0f;
        float currentSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f; //if using analog stick, scale with input. Else, magnitude is 1f

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
        if (input.move != Vector2.zero)
			{
				inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
			}
        characterController.Move(inputDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, verticalSpeed, 0.0f) * Time.deltaTime); //move player
    }
}
