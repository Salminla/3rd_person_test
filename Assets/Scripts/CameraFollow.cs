using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    GameObject CameraFollowObject;
    [SerializeField]
    GameObject CameraZoomPoint;
    [SerializeField]
    bool invertedVertical = true;

    Vector3 camInitPos;

    float rotX = 0f;
    float rotY = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotX = rot.x;
        rotY = rot.y;

        camInitPos = transform.position;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Get controller input
        float horizontalX = Input.GetAxis("HorizontalRS");
        float verticalY = Input.GetAxis("VerticalRS");
        //Get mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        //Combined input
        float horizontalBoth = horizontalX + mouseX;
        float verticalBoth = verticalY + mouseY;

        if (invertedVertical)
            rotX += -verticalBoth * 150 * Time.deltaTime;
        else
            rotX += verticalBoth * 150 * Time.deltaTime;

        rotY += horizontalBoth * 150 * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -80, 70);

        Quaternion localRotation = Quaternion.Euler(0.0f, rotY, rotX);
        transform.rotation = localRotation;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Camera.main.transform.position = CameraZoomPoint.transform.position;
        }
    }

    void LateUpdate()
    {
        CameraUpdater();
    }
    void CameraUpdater()
    {
        //Set the target
        Transform target = CameraFollowObject.transform;
        //Move towards
        float step = 120 * Time.deltaTime;
        if (!Input.GetKey(KeyCode.Mouse1))
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }    
    }
}
