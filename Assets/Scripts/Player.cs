using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    // References
    private Rigidbody rb;
    private CapsuleCollider playerCollider;
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
    private float airSpeedO;
    [SerializeField]
    private float ropeSpeed = 20f;
    [SerializeField]
    private float jumpForce = 5f;

    private bool isJumping;
    private bool isGrounded;
    private bool delayFinished = true;
    private bool delayOngoing;
    [SerializeField]
    private bool onRope;

    //Movement vars
    private float sumOfVelocityXZ;
    private float sumOfVelocityXYZ;

    private Vector3 GroundMovement;
    private Vector3 AirMovement;
    private Vector3 RopeMovement;

    private Vector3 playerRotation;

    // MovementSmoothingVars
    public float iAcceleration = 1f;
    public float iDeceleration = 2f;
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
        playerCollider = GetComponent<CapsuleCollider>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        //playerHinge = GetComponent<HingeJoint>();

        playerCollider.contactOffset = 0.02f;

        airSpeedO = airSpeed;

        rb.drag = 0.05f;
    }
    //All the Input capturing done in Update
    private void Update()
    {
        InputHandler();

        isGrounded = IsGrounded();
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

        inputs.x = InputSmoothing("Horizontal", ref xSmoothed);
        inputs.z = InputSmoothing("Vertical", ref ySmoothed);
        
        inputs = Vector3.ClampMagnitude(inputs, 1f);

        // Movement calculations in Update
        sumOfVelocityXZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        sumOfVelocityXYZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.y);
        
        // GroundMovement = (transform.forward * horizontalSpeed * inputs.x ) + (transform.right * horizontalSpeed * -inputs.z) + new Vector3(0, rb.velocity.y);
        AirMovement = (transform.forward * (airSpeed * inputs.x * 100)) + (transform.right * (airSpeed * -inputs.z * 100));
        RopeMovement = (transform.forward * (ropeSpeed * inputs.x)) + (transform.right * (ropeSpeed * -inputs.z));

        playerRotation = new Vector3(transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);

        // Jumping
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            StartCoroutine(JumpBuffer());
        }

        #region DEBUG STUFF
        //Debug.Log(Quaternion.LookRotation(transform.forward, Vector3.up).eulerAngles);
        //Debug.Log(GroundMovement);
        uiManager.SetDebugUI(1, "Vel: " + sumOfVelocityXZ.ToString("F2") + " AirDir: " + AirMovement.normalized + " VelDir:" + rb.velocity.normalized
                    + "\nDrag: " + rb.drag.ToString("F2") + " ASpeed: " + airSpeed);
        Debug.DrawRay(transform.position, AirMovement.normalized, Color.yellow);
        Debug.DrawRay(transform.position, rb.velocity.normalized, Color.red);

        //IsGrounded function debugging
        if (isGrounded)
            playerMaterial.color = Color.green;
        else
            playerMaterial.color = Color.red;
        //Debug.Log(colliding);
        #endregion
    }
    // Makes the specified input move smoothly from 0 to 1 and vice-versa. A bit buggy still, esp. controllers...
    private float InputSmoothing(string axis, ref float smoothed)
    {
        var accelerating = Input.GetAxisRaw(axis);

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
    private void PlayerMovement()
    {
        // Rotation   
        rb.transform.eulerAngles = playerRotation;

        GroundMovement = (transform.forward * (groundSpeed * inputs.x * Time.deltaTime)) +
                         (transform.right * (groundSpeed * -inputs.z * Time.deltaTime)) + new Vector3(0, rb.velocity.y);
        // rb.drag = sumOfVelocityXZ / 800;

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
            rb.AddForce(AirMovement * Time.deltaTime);
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

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        finalmask = ~finalmask;

        // get the radius of the players capsule collider, and make it a tiny bit smaller than that
        var groundColliderRad = playerCollider.radius;
        float radiusY = groundColliderRad * 0.60f;

        // returns true if the capsule touches something on that layer
        // Grounding check for 
        bool isGroundedY = Physics.CheckCapsule(new Vector3(transform.position.x, transform.position.y + 0.30f, transform.position.z),
                                                new Vector3(transform.position.x, transform.position.y - 0.30f, transform.position.z), radiusY, finalmask);
        // Grounding check for 
        return isGroundedY;

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
    //DEBUG
    private IEnumerator DebugUpdate()
    {
        yield return 0;
    }
}