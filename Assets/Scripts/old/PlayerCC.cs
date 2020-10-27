using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCC : MonoBehaviour
{
    Rigidbody rb;
    GameObject playerModel;
    BoxCollider playerCollider;
    //Camera mainCamera;
    CharacterController characterController;

    private float horizontalSpeed = 100;
    private float verticalSpeed = 100;

    [SerializeField]
    private float groundDistance = 5;

    private float horizontalAxis;
    private float verticalAxis;

    private float camVerticalOffset = 3;
    private float camHorizontalOffset = 4;
    private float camLerpVal = 5;

    private bool colliding = false;

    Vector3 cameraOffset = new Vector3(4, 3, 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerModel = GameObject.Find("PlayerModel");
        playerCollider = GetComponent<BoxCollider>();
        //mainCamera = Camera.main;
        characterController = GetComponent<CharacterController>();

        playerCollider.contactOffset = 0.02f;
    }

    // Update is called once per frame
    void Update()
    {
        //Player movement
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");
        //if (!colliding)
        //{
        //Vector3 velocity = rb.velocity;
        //rb.velocity =  Vector3.forward * Time.deltaTime * horizontalAxis * horizontalSpeed * 10;

        var movement = new Vector3(-verticalAxis, 0, horizontalAxis);

        characterController.SimpleMove(movement * Time.deltaTime * horizontalSpeed);

        if (movement.magnitude > 0)
        {
            Quaternion newDirection = Quaternion.LookRotation(movement);

            transform.rotation = Quaternion.Slerp(transform.rotation, newDirection, Time.deltaTime * 10);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0, 3000), ForceMode.Impulse);
        }

        //Camera follow, lerping
        //Vector3 cameraPosition = mainCamera.transform.position + cameraOffset;
        //cameraPosition.y = Mathf.Lerp(mainCamera.transform.position.y, transform.position.y + camVerticalOffset, camLerpVal * Time.deltaTime);
        //cameraPosition.x = Mathf.Lerp(mainCamera.transform.position.x, transform.position.x + camHorizontalOffset, camLerpVal * Time.deltaTime);
        //cameraPosition.z = Mathf.Lerp(mainCamera.transform.position.z, transform.position.z, camLerpVal * Time.deltaTime);
        //mainCamera.transform.position = cameraPosition;

        if (IsGrounded())
        {
            Debug.Log("Am grounded");
        }
        //Debug.Log(colliding);
    }
    bool IsGrounded()
    {
        // Bit shift the index of the layer(8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        Vector3 rayStartPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        
        if (Physics.Raycast(rayStartPos, transform.TransformDirection(Vector3.down), out hit, groundDistance, layerMask))
        {
            Debug.DrawRay(rayStartPos, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);

            return true;
        }
        else
        {
            return false;
        }
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