using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // References
    Rigidbody rb;
    GameObject playerModel;
    CapsuleCollider playerCollider;
    Camera mainCamera;
    [SerializeField]
    GameObject playerCamera;
    [SerializeField]
    GameObject pointerObject;
    [SerializeField]
    Material playerMaterial;
    HingeJoint playerHinge;

    Vector2 lookDirection;
    Vector3 inputs;

    // Movement
    [SerializeField]
    private float horizontalSpeed = 10f;
    [SerializeField]
    private float verticalSpeed = 10f;
    [SerializeField]
    private float airSpeed = 1000f;
    private float airSpeedO;
    [SerializeField]
    private float ropeSpeed = 20f;
    [SerializeField]
    private float jumpForce = 1100f;

    [SerializeField]
    private float groundDistance = 5;
    private float gCapsuleExtremesX = 0.4f;
    private float gCapsulePosY = 0.15f;

    private bool isJumping = false;
    private bool colliding = false;
    private bool isGrounded;
    private bool delayFinished = true;
    private bool delayOngoing = false;
    [SerializeField]
    private bool onRope = false;

    //Movement vars
    float sumOfVelocityXZ;
    float sumOfVelocityXYZ;

    Vector3 GroundMovement;
    Vector3 AirMovement;
    Vector3 RopeMovement;

    Vector3 playerRotation;

    // MovementSmoothingVars
    public float iAcceleration = 1f;
    public float iDeceleration = 2f;
    public float xSmoothed = 0;
    public float ySmoothed = 0;

    // UI
    UIManager uiManager;

    public UnityEngine.UI.Text debugText1;
    public UnityEngine.UI.Text debugText2;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerModel = GameObject.Find("Thing");
        playerCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        //playerHinge = GetComponent<HingeJoint>();

        playerCollider.contactOffset = 0.02f;

        airSpeedO = airSpeed;

        rb.drag = 0.1f;
    }
    //All the Input capturing done in Update
    void Update()
    {
        InputHandler();

        if (IsGrounded())
            isGrounded = true;
        else
            isGrounded = false;
    }

    // All the rigidbody interactions done in FixedUpdate
    void FixedUpdate()
    {
        PlayerMovement();       
    }
    /// <summary>
    /// Function that handles all of the player's inputs
    /// </summary>
    void InputHandler()
    {
        // Player movement axis
        inputs = Vector3.zero;

        inputs.x = InputSmoothing("Horizontal", ref xSmoothed);
        inputs.y = InputSmoothing("Vertical", ref ySmoothed);
        
        inputs = Vector3.ClampMagnitude(inputs, 1f);

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            StartCoroutine(JumpBuffer());
        }

        // Movement calculations in Update
        sumOfVelocityXZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        sumOfVelocityXYZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.y);

        GroundMovement = (transform.forward * horizontalSpeed * inputs.x * Time.deltaTime) + (transform.right * horizontalSpeed * -inputs.y * Time.deltaTime) + new Vector3(0, rb.velocity.y);
        AirMovement = (transform.forward * airSpeed * inputs.x * 100 * Time.deltaTime) + (transform.right * airSpeed * -inputs.y * 100 * Time.deltaTime);
        RopeMovement = (transform.forward * ropeSpeed * inputs.x * Time.deltaTime) + (transform.right * ropeSpeed * -inputs.y * Time.deltaTime);

        playerRotation = new Vector3(transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);

        #region DEBUG STUFF
        //Debug.Log(Quaternion.LookRotation(transform.forward, Vector3.up).eulerAngles);
        //Debug.Log(GroundMovement);
        uiManager.SetDebugUI(1, "Vel: " + sumOfVelocityXZ.ToString("F2") + " AirDir: " + AirMovement.normalized + " VelDir:" + rb.velocity.normalized
                    + "\nDrag: " + rb.drag.ToString("F2") + " ASpeed: " + airSpeed);
        Debug.DrawRay(transform.position, AirMovement.normalized, Color.yellow);
        Debug.DrawRay(transform.position, rb.velocity.normalized, Color.red);

        //IsGrounded function debugging
        if (isGrounded)
        {
            playerMaterial.color = Color.green;
        }
        else
        {
            playerMaterial.color = Color.red;
        }
        //Debug.Log(colliding);
        #endregion
    }
    // Makes the specified input move smoothly from 0 to 1 and vice-versa. A bit buggy still, esp. controllers...
    private float InputSmoothing(string axis, ref float smoothed)
    {
        float accelerating = Input.GetAxisRaw(axis);

        if (accelerating > 0)
            smoothed = Mathf.Clamp(smoothed + iAcceleration * Time.deltaTime, -1f, accelerating);
        else if (accelerating < 0)
            smoothed = Mathf.Clamp(smoothed - iAcceleration * Time.deltaTime, accelerating, 1f);
        else
                smoothed = Mathf.Clamp01(Mathf.Abs(smoothed) - iDeceleration * Time.deltaTime) * Mathf.Sign(smoothed);
        return smoothed;
    }
    /// <summary>
    /// Function that handles all of the player's movement, using rigidbody
    /// </summary>
    void PlayerMovement()
    {
        // Movement vector, when player is on the ground

        //new Vector3(verticalAxis * -verticalSpeed * Time.deltaTime * 50, rb.velocity.y, horizontalAxis * horizontalSpeed * Time.deltaTime * 50);

        // Movement vector when the player is in the air

        //Vector3 AirMovementDir = rb.velocity.normalized;
        //Vector3 AirMovement = new Vector3(AirMovementDir.x + 10 * inputs.x * Time.deltaTime, AirMovementDir.z + 10 * -inputs.y * Time.deltaTime);
        //new Vector3(verticalAxis * -verticalSpeed * Time.deltaTime * airSpeed, rb.velocity.y, horizontalAxis * horizontalSpeed * Time.deltaTime * airSpeed);


        // Rotation
        
        rb.transform.eulerAngles = playerRotation;

        //rb.drag = sumOfVelocityXZ / 15;

        if (!onRope)
        {
            if (sumOfVelocityXZ > 8)
                airSpeed = airSpeedO - (sumOfVelocityXZ * 100) / 1.5f;
            else
                airSpeed = airSpeedO;
        }
        else
        {
            airSpeed = ropeSpeed;
        }
        // Adds slight delay after landing from a jump before switching to GroundMovement
        if (isGrounded && !delayOngoing && !delayFinished)
        {
            delayOngoing = true;
            StartCoroutine(MovementDelay());
        }
        if (isGrounded && delayFinished)
        {
            if (GroundMovement.x != 0 || GroundMovement.z != 0)
                rb.velocity = GroundMovement;
        }
        else if (!isGrounded)
        {
            delayFinished = false;
            delayOngoing = false;
            rb.AddForce(AirMovement);
        }

        // Jumping
        if (isJumping && isGrounded)
        {
            rb.AddForce(new Vector3(0, rb.mass * 5), ForceMode.Impulse);
            isJumping = false;
        }
        // Nudge if stuck (If stuck in place while the game thinks you are not grounded) Buggy, allows wall climbing in corners.
        if (!IsGrounded() && sumOfVelocityXYZ < 0.01f && isJumping)
            rb.AddForce(new Vector3(0, rb.mass * 1.5f), ForceMode.Impulse);

        
    }
    //Grounding check done with CheckCapsule
    bool IsGrounded()
    {
        // Bit shift the index of the layer(8) to get a bit mask
        // 0000 0001 -> 1000 0000
        int layerMask  = 1 << 8;
        int layerMask2 = 1 << 2;
        int finalmask = layerMask | layerMask2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        finalmask = ~finalmask;

        // get the radius of the players capsule collider, and make it a tiny bit smaller than that
        float radius = playerCollider.radius * 0.75f;
        float radiusY = playerCollider.radius * 0.60f;
        float radiusA = playerCollider.radius * 0.80f;

        // returns true if the capsule touches something on that layer
        bool isGroundedL = Physics.CheckCapsule(new Vector3(transform.position.x - gCapsuleExtremesX, transform.position.y - gCapsulePosY, transform.position.z), 
                                                new Vector3(transform.position.x + gCapsuleExtremesX, transform.position.y - gCapsulePosY, transform.position.z), radius, finalmask);
        // Grounding check for 
        bool isGroundedY = Physics.CheckCapsule(new Vector3(transform.position.x, transform.position.y + 0.30f, transform.position.z),
                                                new Vector3(transform.position.x, transform.position.y - 0.30f, transform.position.z), radiusY, finalmask);
        // Grounding check for 
        bool isGroundedA = Physics.CheckCapsule(new Vector3(transform.position.x, transform.position.y + 0.18f, transform.position.z),
                                                new Vector3(transform.position.x, transform.position.y - 0.18f, transform.position.z), radiusA, finalmask);
        if (isGroundedY)
            return true;
        else
            return false;

        #region OLD GROUNDING STUFF
        //get the position (assuming its right at the bottom) and move it up by almost the whole radius
        /*
        Vector3 pos = transform.position + Vector3.down * 0.06f;
        Vector3 posFront = transform.position + Vector3.down * 0.06f;
        Vector3 posBack = transform.position + Vector3.down * 0.06f;
        */
        //bool isGrounded = Physics.CheckSphere(pos, radius, layerMask);
        //bool isGroundedFront = Physics.CheckSphere(pos, radius, layerMask);
        //bool isGroundedBack = Physics.CheckSphere(pos, radius, layerMask);
        //Physics.Che
        #endregion
    }
    // Slight delay before being able to move againg after landing
    IEnumerator MovementDelay()
    {
        delayFinished = false;
        yield return new WaitForSeconds(.1f);
        delayFinished = true;
    }
    // Stops player from jumping after landing when pressing the jump button while in the air
    IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(.1f);
        isJumping = false;
    }
    //DEBUG
    IEnumerator DebugUpdate()
    {
        yield return 0;
    }
}