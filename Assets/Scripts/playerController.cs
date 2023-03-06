using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [Header("HUD and Core Mechanics")]
    public GameObject pauseMenu;
    public GameObject winScreen;
    public GameObject loseScreen;
    public bool inStartingArea;
    public bool paused;
    private Rigidbody rigidBody;

    [Header("Status Bars")]
    public Image healthBar;
    public Image staminaBar;
    private float healthSize;
    private float staminaSize;
    public Image invisibilityIcon;
    public GameObject invisActive;
    public Image phaseIcon;
    public GameObject phaseActive;
    private float invisSize;
    private float phaseSize;
    public float healthPoints;
    private float currentHealth;

    CharacterController characterController;
    private inputManager input;
    private PlayerInput playerInput;

    private Camera cam; //camera variables
    [Header("Movement")]
    public float cameraSpeed;
    private float cameraRotation;
    private float cameraPitch;
    private float rotationSpeed;

    private float speed;
    public float MoveSpeed;
    private float TrueMoveSpeed;
	public float SprintSpeed;
    public float SprintMax;
    private float SprintSpent;
    public float SprintRechargeSpeed;
    public float SprintRechargeCooldown;
    private float SprintRechargeTimer;
    private float TrueSprintSpeed;
    public float JumpHeight;
    private float verticalSpeed;
    public float Gravity = -15.0f;
    private bool Crouching;
 
    private float jumpTimer;
    public float jumpCooldown = 0.1f;
    private float terminalVelocity = 53.0f;
    
    [Header("Abilities")]
    public bool isInvisible = false;
    private bool isPhasing;
    public bool insideWall;
    public float invisibleCooldown;
    public float phaseCooldown;
    public float invisibleTime;
    public float phaseTime;
    private float invisibleTimer;
    private float phaseTimer;
    private float invisibleCooldownTimer;
    private float phaseCooldownTimer;
    
    void Start()
    {
        cam = Camera.main;
        characterController = GetComponent<CharacterController>();
        rigidBody = GetComponent<Rigidbody>();
        input = GetComponent<inputManager>();
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Enable();
        TrueSprintSpeed = SprintSpeed;
        TrueMoveSpeed = MoveSpeed;
        healthSize = healthBar.rectTransform.rect.width;
        staminaSize = staminaBar.rectTransform.rect.width;
        invisSize = invisibilityIcon.rectTransform.rect.height;
        phaseSize = phaseIcon.rectTransform.rect.height;
        SprintSpent = SprintMax;
        currentHealth = healthPoints;
        invisibleCooldownTimer = invisibleCooldown;
        phaseCooldownTimer = phaseCooldown;
    }
 
    void Update()
    {
        //pause

        if (input.pause && paused == false)
        {
            Time.timeScale = 0f;
            paused = true;
            pauseMenu.SetActive(true);
        }
        else if(input.pause == false && paused == true)
        {
            Time.timeScale = 1;
            paused = false;
            pauseMenu.SetActive(false);
        }
        if(paused)return;

        //camera movement
        cameraPitch -= input.look.y * cameraSpeed;
        cameraRotation = input.look.x * cameraSpeed;
 
        cameraPitch = Mathf.Clamp(cameraPitch, -90, 90);
 
        cam.transform.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);

        transform.Rotate(Vector3.up * cameraRotation);

        //player movement
        SprintRechargeTimer += Time.deltaTime; //Creates cooldown for sprint recharge
        float targetSpeed = (input.sprint && SprintSpent > 0) ? SprintSpeed : MoveSpeed; //checks for sprint input and if sufficient stamina remaining
        if(input.sprint && SprintSpent > 0) //If sprinting
        {
            SprintSpent = SprintSpent - Time.deltaTime; //Reduces available stamina whilee sprinting
            staminaBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, staminaSize * (SprintSpent / SprintMax)); //scales stamina bar
            SprintRechargeTimer = 0; //Resets recharge cooldown while sprinting
        }
        else if (SprintSpent < SprintMax && SprintRechargeTimer >= SprintRechargeCooldown) //if not sprinting, less than max stamina, and recharge cooldown has passed
        {
            SprintSpent += SprintRechargeSpeed * Time.deltaTime; //recharges stamina
            staminaBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, staminaSize * (SprintSpent / SprintMax)); //scales stamina bar
        }
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

        //abilities
        //invisibility
        invisibleCooldownTimer += Time.deltaTime;
        if (input.invisible && invisibleCooldownTimer >= invisibleCooldown)
        {
            isInvisible = true;
            invisibleCooldownTimer = 0;
            invisibilityIcon.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, invisSize * 0);
            invisActive.SetActive(true);
        }
        if(invisibleCooldownTimer >= invisibleTime)
        {
            isInvisible = false;
            invisActive.SetActive(false);
        }
        if (invisibleCooldownTimer < invisibleCooldown)
        {
            invisibilityIcon.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, invisSize * (invisibleCooldownTimer / invisibleCooldown));
        }
        //phasing
        phaseCooldownTimer += Time.deltaTime;
        if (input.phase && phaseCooldownTimer >= phaseCooldown)
        {
            phaseCooldownTimer = 0;
            phaseIcon.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, phaseSize * 0);
            phaseActive.SetActive(true);
            isPhasing = true;
        }
        if(phaseCooldownTimer >= phaseTime && !insideWall)
        {
            isPhasing = false;
            phaseActive.SetActive(false);
        }
        if (phaseCooldownTimer < phaseCooldown)
        {
            phaseIcon.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, phaseSize * (phaseCooldownTimer / phaseCooldown));
        }
	}   

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if(collision.collider.tag == "Obstacle" && isPhasing)
        {
            collision.collider.isTrigger = true;
            insideWall = true;
        }   
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.GetComponent<Collider>().tag == "Obstacle")
        {
            insideWall = false;
            collision.GetComponent<Collider>().isTrigger = false;
            phaseCooldownTimer = phaseTime;
        }
    }

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthSize * (currentHealth / healthPoints));
        if(currentHealth <= 0)
        {
            loseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void win()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }
}
