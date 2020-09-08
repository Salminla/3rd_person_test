using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    Vector2 lookDirection;
    Vector3 inputs;

    [SerializeField]
    private float horizontalSpeed = 6;
    [SerializeField]
    private float verticalSpeed = 6;
    [SerializeField]
    private float airSpeed = 7000;
    private float airSpeedO;
    [SerializeField]
    private float jumpForce = 900f;

    [SerializeField]
    private float groundDistance = 5;
    private float gCapsuleExtremesX = 0.4f;
    private float gCapsulePosY = 0.15f;

    private bool isJumping = false;
    private bool colliding = false;
    private bool isGrounded;
    private bool delayFinished = true;
    private bool delayOngoing = false;

    //MovementSmoothingTestVars
    public float iAcceleration = 1f;
    public float iDeceleration = 2f;
    public float xSmoothed = 0;
    public float ySmoothed = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerModel = GameObject.Find("Thing");
        playerCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;

        playerCollider.contactOffset = 0.02f;

        airSpeedO = airSpeed;
    }
    //All the Input capturing done in Update
    void Update()
    {
        InputHandler();

        if (IsGrounded())
            isGrounded = true;
        else
            isGrounded = false;

        PointerFunction();
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

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            StartCoroutine(JumpBuffer());
        }
    }
    // Makes the specified input move smoothly from 0 to 1 and vice-versa.
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

        float sumOfVelocityXZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        float sumOfVelocityXYZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.y);

        // Movement, when player is on the ground
        Vector3 GroundMovement = (transform.forward * horizontalSpeed * inputs.x * Time.deltaTime) + (transform.right * horizontalSpeed * -inputs.y * Time.deltaTime) + new Vector3(0, rb.velocity.y);
        //new Vector3(verticalAxis * -verticalSpeed * Time.deltaTime * 50, rb.velocity.y, horizontalAxis * horizontalSpeed * Time.deltaTime * 50);

        // Movement when the player is in the air
        Vector3 AirMovement = (transform.forward * airSpeed * inputs.x * 100 * Time.deltaTime) + (transform.right * airSpeed * -inputs.y * 100 * Time.deltaTime);
        //new Vector3(verticalAxis * -verticalSpeed * Time.deltaTime * airSpeed, rb.velocity.y, horizontalAxis * horizontalSpeed * Time.deltaTime * airSpeed);

        // Rotation
        Vector3 playerRotation = new Vector3(transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);
        rb.transform.eulerAngles = playerRotation;
        //Quaternion rotation = new Quaternion(playerCamera.transform.rotation.x, playerCamera.transform.rotation.y, playerCamera.transform.rotation.z, 1);
        //rb.MoveRotation(rotation);
        //Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
        //rb.MoveRotation(rb.rotation * deltaRotation);

        //rb.drag = sumOfVelocityXZ / 15;
        rb.drag = 0.2f;

        if (sumOfVelocityXZ > 8)
            airSpeed = airSpeedO - (sumOfVelocityXZ * 100) / 1.5f;
        else
            airSpeed = airSpeedO;

        // Adds slight delay after landing from a jump before switching to GroundMovement
        if (isGrounded && !delayOngoing && !delayFinished)
        {
            delayOngoing = true;
            StartCoroutine(MovementDelay());
        }
        if (isGrounded && delayFinished)
        {
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
        // Nudge if stuck (If stuck in place while the game thinks you are not grounded)
        if (!IsGrounded() && sumOfVelocityXYZ < 0.01f && isJumping)
            rb.AddForce(new Vector3(0, rb.mass * 1.5f), ForceMode.Impulse);

        #region DEBUG STUFF
        //Debug.Log(Quaternion.LookRotation(transform.forward, Vector3.up).eulerAngles);
        //Debug.Log(GroundMovement);
        Debug.Log("Vel: " + sumOfVelocityXZ.ToString("F2") + "AirDir: " + AirMovement.normalized + "VelDir:" + rb.velocity.normalized
                    + "Drag: " + rb.drag.ToString("F2") + "ASpeed: " + airSpeed);
        Debug.DrawRay(transform.position, AirMovement.normalized, Color.yellow);
        Debug.DrawRay(transform.position, rb.velocity.normalized, Color.red);

        //IsGrounded function debugging
        if (isGrounded)
        {
            playerMaterial.color = Color.green;
            //playerModel.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", Color.green);
            //Debug.Log("Am grounded");
        }
        else
        {
            playerMaterial.color = Color.red;
            //playerModel.GetComponent<SkinnedMeshRenderer>().material.SetColor("_Color", Color.red);
        }
        //Debug.Log(colliding);
        #endregion
    }
    //Grounding check done with CheckCapsule
    bool IsGrounded()
    {
        // Bit shift the index of the layer(9) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        // get the radius of the players capsule collider, and make it a tiny bit smaller than that
        float radius = playerCollider.radius * 0.75f;
        float radiusY = playerCollider.radius * 0.60f;
        float radiusA = playerCollider.radius * 0.80f;

        // returns true if the capsule touches something on that layer
        bool isGroundedL = Physics.CheckCapsule(new Vector3(transform.position.x - gCapsuleExtremesX, transform.position.y - gCapsulePosY, transform.position.z), 
                                                new Vector3(transform.position.x + gCapsuleExtremesX, transform.position.y - gCapsulePosY, transform.position.z), radius, layerMask);
        // Grounding check for 
        bool isGroundedY = Physics.CheckCapsule(new Vector3(transform.position.x, transform.position.y + 0.30f, transform.position.z),
                                                new Vector3(transform.position.x, transform.position.y - 0.30f, transform.position.z), radiusY, layerMask);
        // Grounding check for 
        bool isGroundedA = Physics.CheckCapsule(new Vector3(transform.position.x, transform.position.y + 0.18f, transform.position.z),
                                                new Vector3(transform.position.x, transform.position.y - 0.18f, transform.position.z), radiusA, layerMask);
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
    // Function for the player's pointer in the world
    void PointerFunction()
    {
        // Bit shift the index of the layer(8) and layer(2) to get a bit mask
        int layerMask1 = 1 << 8;
        int layerMask2 = 1 << 2;
        int finalMask = layerMask1 | layerMask2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        finalMask = ~finalMask;

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, finalMask))
        {
            if (!pointerObject.activeSelf)
            {
                Debug.Log("Pointer on");
                pointerObject.SetActive(true);
            }
            //Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            pointerObject.transform.position = Vector3.Lerp(pointerObject.transform.position, hit.point, Time.deltaTime * 30);
        }
        else
        {
            if (pointerObject.activeSelf)
            {
                Debug.Log("Pointer off");
                pointerObject.SetActive(false);
            }      
        }
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