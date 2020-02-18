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

    Vector3 cameraOffset = new Vector3(4, 3, 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerModel = GameObject.Find("PlayerModel");
        playerCollider = playerModel.GetComponent<BoxCollider>();
        mainCamera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        horizontalAxis = Input.GetAxis("Horizontal");
        verticalAxis = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * Time.deltaTime * horizontalAxis * horizontalSpeed);
        transform.Translate(Vector3.left * Time.deltaTime * verticalAxis * verticalSpeed);

        //mainCamera.transform.position = transform.position + cameraOffset;
        Vector3 cameraPosition = mainCamera.transform.position + cameraOffset;
        cameraPosition.y = Mathf.Lerp(mainCamera.transform.position.y, transform.position.y + 3, 4.0f * Time.deltaTime);
        cameraPosition.x = Mathf.Lerp(mainCamera.transform.position.x, transform.position.x + 4, 4.0f * Time.deltaTime);
        cameraPosition.z = Mathf.Lerp(mainCamera.transform.position.z, transform.position.z, 4.0f * Time.deltaTime);
        mainCamera.transform.position = cameraPosition;
    }
}
