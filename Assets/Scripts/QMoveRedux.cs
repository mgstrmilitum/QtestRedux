
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class QMoveRedux : MonoBehaviour, IDamage
{
    //Input related variables
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Qtest inputActions;
    [SerializeField] float maxSpeed, jumpForce, movementSpeed;
    [SerializeField] float strafeSpeed, forwardBackSpeed; //assume 1.0f is default
    [SerializeField] float sprintMultiplier;
    [SerializeField] float crouchMultiplier;
    InputAction move;
    InputAction look;
    InputAction jump;

    //health-related
    [SerializeField] int health;
    int origHealth;

    //movement bools
    bool isGrounded;
    bool isCrouching;
    bool isSprinting;

    //Camera variables
    float rotationX;
    float rotationY;
    [SerializeField] Transform playerLookTransform;

    //Player adjustable settings
    [SerializeField] bool invertLook;
    [SerializeField] float xMouseSensitivity;
    [SerializeField] float yMouseSensitivity;

    //maybe move inside of Update long-term effeciency sake wise (talk to teacher about this)
    Vector2 moveDirection; //variable for reading in WASD input

    [SerializeField] Vector3 forceDirection;
    [SerializeField] Transform centerOfMass;

    private void Awake()
    {
        inputActions = new Qtest();
    }
    private void OnEnable()
    {

        inputActions.Player.Jump.performed += Jump;
        inputActions.Player.Crouch.performed += Crouch;
        inputActions.Player.CrouchFinish.performed += UnCrouch;
        inputActions.Player.Sprint.performed += Sprint;
        inputActions.Player.SprintFinish.performed += UnSprint;
        move = inputActions.Player.Move;
        look = inputActions.Player.Look;
        jump = inputActions.Player.Jump;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        //playerControls.Disable();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    private void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 3f, Color.blue);
        moveDirection = move.ReadValue<Vector2>();
        //if (isGrounded)
        Movement();
        PerformGravity();
        Rotation(look.ReadValue<Vector2>());


        //at end of update, set character to correct vertical orientation
    }

    void Movement()
    {
        if (moveDirection == Vector2.zero)
        {
            Debug.Log("Yatta!");
            forceDirection = new Vector3(0f, forceDirection.y, 0f);
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        forceDirection += moveDirection.x * strafeSpeed * transform.right * Time.deltaTime;
        forceDirection += moveDirection.y * forwardBackSpeed * transform.forward * Time.deltaTime;
        
        if (isSprinting)
        {
            forceDirection *= sprintMultiplier;
        }
        else if (isCrouching)
        {
            forceDirection *= crouchMultiplier;
        }
        rb.AddForce(forceDirection * movementSpeed * Time.deltaTime, ForceMode.Impulse);
        //rb.AddForceAtPosition(forceDirection * movementSpeed * Time.deltaTime, centerOfMass.position, ForceMode.Impulse);
        forceDirection = Vector3.zero;
        Debug.Log(moveDirection.ToString());
        Debug.Log("Doing it right!");
    }

    void AirMovement()
    {

    }
    void Rotation(Vector2 mouseRotation)
    {
        rotationX += mouseRotation.y * xMouseSensitivity * Time.deltaTime;
        rotationY += mouseRotation.x * yMouseSensitivity * Time.deltaTime;

        if (rotationX < -90f)
        {
            rotationX = -90f;
        }
        else if (rotationX > 90f)
        {
            rotationX = 90f;
        }

        this.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        if (invertLook)
        {
            playerLookTransform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
        else
        {
            playerLookTransform.rotation = Quaternion.Euler(-rotationX, rotationY, 0);
        }
    }

    void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Crouch(InputAction.CallbackContext ctx)
    {
        isCrouching = true;
    }

    void Sprint(InputAction.CallbackContext ctx)
    {
        isSprinting = true;
    }

    void UnSprint(InputAction.CallbackContext ctx)
    {
        isSprinting = false;
    }

    void UnCrouch(InputAction.CallbackContext ctx)
    {
        isCrouching = false;
    }

    void PerformGravity()
    {
        if(!isGrounded)
        {

            forceDirection += GameManager.Instance.worldGravity * -Vector3.up * Time.deltaTime;
        }
        else
        {
            //forceDirection = new Vector3(forceDirection.x, 0f, forceDirection.z);
        }
    }

    void PerformFriction()
    {

    }

    public void TakeDamage(int amount)
    {
        throw new System.NotImplementedException();
    }
}
