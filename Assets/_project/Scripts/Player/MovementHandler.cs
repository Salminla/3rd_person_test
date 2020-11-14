using System.Collections;
using _project.Scripts;
using _project.Scripts.Player;
using UnityEngine;

[RequireComponent(typeof(InputHandler))]
public class MovementHandler : MonoBehaviour
{
    private Rigidbody rb;
    
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float jumpForce = 5f;

    private bool delayFinished = true;
    private bool delayOngoing;

    private Transform playerTransform;
    private CapsuleCollider playerCollider;
    private float sumOfVelocityXZ;
    public float SumOfVelocityXYZ { get; private set; }
    public bool isGrounded { get; private set; }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = transform;
        playerCollider = GetComponent<CapsuleCollider>();

        playerCollider.contactOffset = 0.02f;

        rb.drag = 0.05f;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        
        // Movement calculations in Update
        sumOfVelocityXZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z);
        SumOfVelocityXYZ = Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.y);
    }
    
    public void PlayerMovement()
    {
        // Rotation   
        rb.transform.eulerAngles = inputHandler.playerRotation;
        
        // Adds slight delay after landing from a jump before switching to GroundMovement
        if (isGrounded && !delayOngoing && !delayFinished)
        {
            delayOngoing = true;
            StartCoroutine(MovementDelay());
        }
        if (isGrounded && delayFinished)
        {
            var gettingInput = inputHandler.groundMovement.x != 0 || inputHandler.groundMovement.z != 0;
            if (gettingInput)
                rb.velocity = inputHandler.groundMovement * Time.deltaTime + new Vector3(0, rb.velocity.y) ;
        }
        else if (!isGrounded)
        {
            delayFinished = false;
            delayOngoing = false;
            rb.AddForce(inputHandler.airMovement * Time.deltaTime, ForceMode.Force);
        }

        // Jumping
        if (inputHandler.isJumping && isGrounded)
        {
            rb.AddForce(new Vector3(0, (rb.mass * jumpForce * Time.deltaTime)-rb.velocity.y*2, 0) , ForceMode.Impulse);
            inputHandler.isJumping = false;
        }
        // Nudge if stuck (If stuck in place while the game thinks you are not grounded) Buggy, allows wall climbing in corners.
        if (!IsGrounded() && SumOfVelocityXYZ < 0.01f && inputHandler.isJumping)
            rb.AddForce(new Vector3(0, rb.mass * 1.5f) * Time.deltaTime, ForceMode.Impulse);
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
    private IEnumerator MovementDelay()
    {
        delayFinished = false;
        yield return new WaitForSeconds(.1f);
        delayFinished = true;
    }
}