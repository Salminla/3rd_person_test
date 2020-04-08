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

    Vector2 lookDirection;
    Vector3 inputs;

    [SerializeField]
    private float horizontalSpeed = 6;
    [SerializeField]
    private float verticalSpeed = 6;
    [SerializeField]
    private float airSpeed = 7000;
    [SerializeField]
    private float jumpForce = 900f;

    [SerializeField]
    private float groundDistance = 5;
    private float gCapsuleExtremesX = 0.4f;
    private float gCapsulePosY = 0.15f;

    private float horizontalAxis;
    private float verticalAxis;

    private float camVerticalOffset = 2;
    private float camHorizontalOffset = 4;
    private float camLerpVal = 5;

    private bool isJumping = false;
    private bool colliding = false;
    private bool isGrounded;
    private bool delayFinished = true;
    private bool delayOngoing = false;

    Vector3 cameraOffset = new Vector3(4, 3, 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerModel = GameObject.Find("PlayerModel");
        playerCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main;

        playerCollider.contactOffset = 0.02f;
    }
    //All the Input capturing done in Update
    void Update()
    {
        //Player movement axis
        inputs = Vector3.zero;
        inputs.x = Input.GetAxis("Horizontal");
        inputs.y = Input.GetAxis("Vertical");
        inputs = Vector3.ClampMagnitude(inputs, 1f);
        //horizontalAxis = Input.GetAxis("Horizontal");
        //verticalAxis = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }
        if (IsGrounded())
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    // All the rigidbody interactions done in FixedUpdate
    void FixedUpdate()
    {
        float sumOfVelocityXZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        //float c_sumOfVelocity = Mathf.Clamp(sumOfVelocity, 0, 6);

        //Movement, when player is on the ground
        Vector3 GroundMovement = (transform.forward * horizontalSpeed * inputs.x * Time.deltaTime) + (transform.right * horizontalSpeed * -inputs.y * Time.deltaTime) + new Vector3(0, rb.velocity.y);
        //new Vector3(verticalAxis * -verticalSpeed * Time.deltaTime * 50, rb.velocity.y, horizontalAxis * horizontalSpeed * Time.deltaTime * 50);

        //Movement when the player is in the air
        Vector3 AirMovement = (transform.forward * airSpeed * inputs.x * 100 * Time.deltaTime) + (transform.right * airSpeed * -inputs.y * 100 * Time.deltaTime);
        //new Vector3(verticalAxis * -verticalSpeed * Time.deltaTime * airSpeed, rb.velocity.y, horizontalAxis * horizontalSpeed * Time.deltaTime * airSpeed);

        //Rotation
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);

        if (isGrounded && !delayOngoing && !delayFinished)
        {
            delayOngoing = true;
            StartCoroutine(movementDelay());
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
        if (rb.velocity.magnitude > 6.5f)
        {
            //rb.velocity = new Vector3(rb.velocity.x - 0.1f, rb.velocity.y, rb.velocity.z-0.1f);
        }

        //DEBUG STUFF
        //Debug.Log(Quaternion.LookRotation(transform.forward, Vector3.up).eulerAngles);
        //Debug.Log(GroundMovement);
        //Debug.Log(sumOfVelocityXZ);

        //Jumping
        if (isJumping && isGrounded)
        {
            rb.AddForce(new Vector3(0, rb.mass * 5), ForceMode.Impulse);
            isJumping = false;
        }

        //Camera follow, lerping
        //Vector3 cameraPosition = mainCamera.transform.position + cameraOffset;
        //cameraPosition.y = Mathf.Lerp(mainCamera.transform.position.y, transform.position.y + camVerticalOffset, camLerpVal * Time.deltaTime);
        //cameraPosition.x = Mathf.Lerp(mainCamera.transform.position.x, transform.position.x + camHorizontalOffset, camLerpVal * Time.deltaTime);
        //cameraPosition.z = Mathf.Lerp(mainCamera.transform.position.z, transform.position.z, camLerpVal * Time.deltaTime);
        //mainCamera.transform.position = cameraPosition;

        //IsGrounded function debugging
        if (isGrounded)
        {
            playerModel.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
            //Debug.Log("Am grounded");
        }
        else
        {
            playerModel.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }
        //Debug.Log(colliding);
    }
    //Grounding check done with CheckCapsule
    bool IsGrounded()
    {
        // Bit shift the index of the layer(9) to get a bit mask
        int layerMask = 1 << 9;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        //get the radius of the players capsule collider, and make it a tiny bit smaller than that
        float radius = playerCollider.radius * 0.75f;
        float radiusY = playerCollider.radius * 0.60f;
        //get the position (assuming its right at the bottom) and move it up by almost the whole radius
        /*
        Vector3 pos = transform.position + Vector3.down * 0.06f;
        Vector3 posFront = transform.position + Vector3.down * 0.06f;
        Vector3 posBack = transform.position + Vector3.down * 0.06f;
        */
        //returns true if the capsule touches something on that layer
        bool isGroundedL = Physics.CheckCapsule(new Vector3(transform.position.x - gCapsuleExtremesX, transform.position.y - gCapsulePosY, transform.position.z), 
                                                new Vector3(transform.position.x + gCapsuleExtremesX, transform.position.y - gCapsulePosY, transform.position.z), radius, layerMask);
        //Grounding check for 
        bool isGroundedY = Physics.CheckCapsule(new Vector3(transform.position.x, transform.position.y + 0.30f, transform.position.z),
                                                new Vector3(transform.position.x, transform.position.y - 0.30f, transform.position.z), radiusY, layerMask);
        //bool isGrounded = Physics.CheckSphere(pos, radius, layerMask);
        //bool isGroundedFront = Physics.CheckSphere(pos, radius, layerMask);
        //bool isGroundedBack = Physics.CheckSphere(pos, radius, layerMask);
        //Physics.Che

        if (isGroundedY)
            return true;
        else
            return false;
    }
    IEnumerator movementDelay()
    {
        delayFinished = false;
        yield return new WaitForSeconds(.1f);
        delayFinished = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            colliding = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            colliding = false;
        }
    }
}