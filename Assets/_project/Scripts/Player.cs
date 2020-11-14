using System.Collections;
using _project.Scripts;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    // References
    private Rigidbody rb;
    private Transform playerTransform;
    private CapsuleCollider playerCollider;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private Material playerMaterial;
    private HingeJoint playerHinge;

    private Vector2 lookDirection;
    private Vector3 inputs;

    // Movement
    [SerializeField]
    private float groundSpeed = 100f;
    [SerializeField]
    private float airSpeed = 1000f;
    private float airSpeedOrig;
    [SerializeField]
    private float jumpForce = 5f;

    private bool isJumping;
    private bool isGrounded;
    private bool delayFinished = true;
    private bool delayOngoing;

    //Movement vars
    private float sumOfVelocityXZ;
    private float sumOfVelocityXYZ;

    private Vector3 groundMovement;
    private Vector3 airMovement;

    private Vector3 playerRotation;

    // MovementSmoothingVars
    public float xSmoothed;
    public float ySmoothed;

    // UI
    private UIManager uiManager;

    public UnityEngine.UI.Text debugText1;
    public UnityEngine.UI.Text debugText2;
    

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = transform;
        playerCollider = GetComponent<CapsuleCollider>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        playerCollider.contactOffset = 0.02f;

        airSpeedOrig = airSpeed;

        rb.drag = 0.05f;
    }
    //All the Input capturing done in Update
    private void Update()
    {
        InputHandler();

        isGrounded = IsGrounded();
        
        // Movement calculations in Update
        sumOfVelocityXZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        sumOfVelocityXYZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.y);
        
        DebugUIUpdate();

    }

    // All the rigidbody interactions done in FixedUpdate
    private void FixedUpdate()
    {
        PlayerMovement();       
    }
    /// <summary>
    /// Function that handles all of the player's inputs
    /// </summary>
    private void InputHandler()
    {
        // Player movement axis
        inputs = Vector3.zero;

        inputs.x = inputHandler.InputSmoothing("Horizontal", ref xSmoothed);
        inputs.z = inputHandler.InputSmoothing("Vertical", ref ySmoothed);
        
        inputs = Vector3.ClampMagnitude(inputs, 1f);

        var forward = playerTransform.forward;
        var right = playerTransform.right;
        airMovement = (forward * (airSpeed * inputs.x * 100)) + (right * (airSpeed * -inputs.z * 100));
        
        var eulerAngles = playerTransform.eulerAngles;
        playerRotation = new Vector3(eulerAngles.x, playerCamera.transform.eulerAngles.y, eulerAngles.z);

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            StartCoroutine(JumpBuffer());
        }
    }

    private void DebugUIUpdate()
    {
        uiManager.SetDebugUI(1, "Vel: " + sumOfVelocityXZ.ToString("F2") + " AirDir: " + airMovement.normalized +
                                " VelDir:" + rb.velocity.normalized
                                + "\nDrag: " + rb.drag.ToString("F2") + " ASpeed: " + airSpeed);
        var position = playerTransform.position;
        Debug.DrawRay(position, airMovement.normalized, Color.yellow);
        Debug.DrawRay(position, rb.velocity.normalized, Color.red);

        //IsGrounded function debugging
        if (isGrounded)
            playerMaterial.color = Color.green;
        else
            playerMaterial.color = Color.red;
        //Debug.Log(colliding);
    }
    /// <summary>
    /// Function that handles all of the player's movement, using rigidbody
    /// </summary>
    private void PlayerMovement()
    {
        // Rotation   
        rb.transform.eulerAngles = playerRotation;

        groundMovement = (playerTransform.forward * (groundSpeed * inputs.x * Time.deltaTime)) +
                         (playerTransform.right * (groundSpeed * -inputs.z * Time.deltaTime)) + new Vector3(0, rb.velocity.y);

        // Adds slight delay after landing from a jump before switching to GroundMovement
        if (isGrounded && !delayOngoing && !delayFinished)
        {
            delayOngoing = true;
            StartCoroutine(MovementDelay());
        }
        if (isGrounded && delayFinished)
        {
            if (groundMovement.x != 0 || groundMovement.z != 0)
                rb.velocity = groundMovement;
        }
        else if (!isGrounded)
        {
            delayFinished = false;
            delayOngoing = false;
            rb.AddForce(airMovement * Time.deltaTime);
        }

        // Jumping
        if (isJumping && isGrounded)
        {
            rb.AddForce(new Vector3(0, rb.mass * jumpForce) * Time.deltaTime, ForceMode.Impulse);
            isJumping = false;
        }
        // Nudge if stuck (If stuck in place while the game thinks you are not grounded) Buggy, allows wall climbing in corners.
        if (!IsGrounded() && sumOfVelocityXYZ < 0.01f && isJumping)
            rb.AddForce(new Vector3(0, rb.mass * 1.5f), ForceMode.Impulse);
    }
    //Grounding check done with CheckCapsule
    private bool IsGrounded()
    {
        // Bit shift the index of the layer(8) to get a bit mask
        // 0000 0001 -> 1000 0000
        int layerMask  = 1 << 8;
        int layerMask2 = 1 << 2;
        int finalmask = layerMask | layerMask2;
        
        //inverts a bitmask.
        finalmask = ~finalmask;

        // get the radius of the players capsule collider, and make it a tiny bit smaller than that
        var groundColliderRad = playerCollider.radius;
        float radius = groundColliderRad * 0.60f;

        // returns true if the capsule touches something on that layer
        var position = playerTransform.position;
        bool isGroundedL = Physics.CheckCapsule(new Vector3(position.x, position.y + 0.30f, position.z),
                                                new Vector3(position.x, position.y - 0.30f, position.z), radius, finalmask);
        return isGroundedL;
    }
    // Slight delay before being able to move againg after landing
    private IEnumerator MovementDelay()
    {
        delayFinished = false;
        yield return new WaitForSeconds(.1f);
        delayFinished = true;
    }
    // Stops player from jumping after landing when pressing the jump button while in the air
    private IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(.1f);
        isJumping = false;
    }
}