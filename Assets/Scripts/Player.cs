using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    GameObject playerModel;
    BoxCollider playerCollider;
    Camera mainCamera;

    private float horizontalSpeed = 10;
    private float verticalSpeed = 10;

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
        mainCamera = Camera.main;

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
        rb.velocity = new Vector3(verticalAxis * -verticalSpeed, 0, horizontalAxis * horizontalSpeed) * Time.deltaTime * 10;
        //rb.velocity = Vector3.right * Time.deltaTime * verticalAxis * horizontalSpeed * 10;
        //rb.MovePosition(transform.position + Vector3.left * Time.deltaTime * verticalAxis * verticalSpeed);

        //    //Vector3 position = rb.transform.position;
        //    //position.x = position.x+3*Time.deltaTime * horizontalAxis * horizontalSpeed;
        //    //position.y = position.y+3*Time.deltaTime * verticalAxis * verticalSpeed;
        //}
        //Vector3 tempVect = new Vector3(0, 0, 1);
        //tempVect = tempVect.normalized * horizontalAxis * horizontalSpeed * Time.deltaTime;
        //rb.MovePosition(transform.position + tempVect);

        //Vector3 tempVect = new Vector3(0, 0, 1);
        //tempVect = tempVect.normalized * horizontalAxis * horizontalSpeed * Time.deltaTime;
        //rb.AddForce(Vector3.forward * horizontalAxis * horizontalSpeed * Time.deltaTime * 10000);

        //if (colliding)
        //{
        //    //transform.position = new Vector3(transform.position.x-1, transform.position.y - 1);
        //    transform.Translate(Vector3.forward * Time.deltaTime * -horizontalAxis * horizontalSpeed);
        //    transform.Translate(Vector3.left * Time.deltaTime * -verticalAxis * verticalSpeed);
        //}
        //Camera follow, lerping
        Vector3 cameraPosition = mainCamera.transform.position + cameraOffset;
        cameraPosition.y = Mathf.Lerp(mainCamera.transform.position.y, transform.position.y + camVerticalOffset, camLerpVal * Time.deltaTime);
        cameraPosition.x = Mathf.Lerp(mainCamera.transform.position.x, transform.position.x + camHorizontalOffset, camLerpVal * Time.deltaTime);
        cameraPosition.z = Mathf.Lerp(mainCamera.transform.position.z, transform.position.z, camLerpVal * Time.deltaTime);
        mainCamera.transform.position = cameraPosition;

        Debug.Log(colliding);
    }

    bool CollisionDetected()
    {
        if (true)
        {

        }

        return true;
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
