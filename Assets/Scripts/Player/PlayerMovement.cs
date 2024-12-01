using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public IPlayerState standState, crouchState;
    private IPlayerState currentState;

    public CameraController cameraController;

    private PlayerController playerController;

    internal PlayerInputActions input;

    private CharacterController characterController;
    internal Animator animator;

    internal Vector2 moveInput;
    internal Vector2 lookInput;
    private Vector3 moveDirection;

    [Header("--- Player Stats ---")]
    public float playerSpeed;
    internal float rollSP = 40f;
    internal float sprintSP = 15f;
    internal float normalizedValue;

    public bool isSprinting;
    public bool isCrouching;
    public bool isRolling;

    internal int animStandV, animStandH;
    internal int animCrouchV, animCrouchH;
    internal int animCrouchTrans;
    internal int animRoll;

    internal float soundTime = 0f;
    private float countTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        input = InputManager.Instance.input;

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();

        SetInput();

        AssignAnimationIDs();

        standState = new StandState(this);
        crouchState = new CrouchState(this);

        currentState = standState;
        currentState.EnterState();

    }

    //private void OnEnable()
    //{
        

    //}

    private void SetInput()
    {
        input.Movement.Enable();

        input.Movement.Move.performed += OnMove;
        input.Movement.Move.canceled += _ => { moveInput = Vector2.zero; SetPlayerSpeed(0f); };

        input.Movement.Look.performed += OnLook;
        input.Movement.Look.canceled += _ => lookInput = Vector2.zero;

        input.Movement.Roll.performed += _ => OnRoll();

        input.Movement.Crouch.performed += _ =>
        {
            if (currentState != crouchState)
                ChangeState(crouchState);
            else
                ChangeState(standState);
        };
    }

    private void OnDisable()
    {
        input.Movement.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext ctx)
    {
        lookInput = ctx.ReadValue<Vector2>();
    }

    private async void OnRoll()
    {
        if (playerController.SPSlider.value >= rollSP && !isRolling)
        {
            PlayerSoundManager.Instance.PlayRollSound();
            isRolling = true;
            animator.SetTrigger("roll");
            playerController.SP -= rollSP;
            playerController.SPSlider.value = playerController.SP;

            await UniTask.Delay((int)(1f * 3000));

            isRolling = false;
        }
    }

    private void AssignAnimationIDs()
    {
        animStandH = Animator.StringToHash("stand_horizontal");
        animStandV = Animator.StringToHash("stand_vertical");
        animCrouchH = Animator.StringToHash("crouch_horizontal");
        animCrouchV = Animator.StringToHash("crouch_vertical");
        animCrouchTrans = Animator.StringToHash("crouch_transition");
        animRoll = Animator.StringToHash("roll");
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.enabled == true)    Move();
        currentState.UpdateState();

        AdjustSP();
        if (moveInput != Vector2.zero)
            PlaySound();

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    UIManager.Instance.BloodScreenEffect();
        //}
    }

    private void Move()
    {
        moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        normalizedValue = (moveInput.x != 0 && moveInput.y != 0) ? 1f / 0.71f : 1f;
        characterController.Move((transform.right * moveInput.x + transform.forward * moveInput.y) * playerSpeed * Time.deltaTime);
    }

    private void PlaySound()
    {
        countTime += Time.deltaTime;
        if (countTime >= soundTime)
        {
            PlayerSoundManager.Instance.PlayWalkSound();
            countTime = 0f;
        }
    }

    public void SetPlayerSpeed(float speed)
    {
        playerSpeed = speed;
    }

    public void SetSoundTime(float time)
    {
        soundTime = time;
    }

    private void AdjustSP()
    {
        if (isSprinting && playerController.SP > 0)
        {
            playerController.SP -= sprintSP * Time.deltaTime;
            playerController.SPSlider.value = playerController.SP;
        }
        else if (playerController.SPSlider.value < playerController.maxSP)
        {
            playerController.SP += playerController.reviveSPSpeed * Time.deltaTime;
            playerController.SPSlider.value = playerController.SP;
        }
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
