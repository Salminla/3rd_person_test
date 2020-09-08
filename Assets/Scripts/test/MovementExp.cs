using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementExp : MonoBehaviour
{
    Vector3 currPos;
    Vector3 newPos;
    Vector3 oldPos;

    // Start is called before the first frame update
    void Start()
    {
        currPos = transform.position;
        newPos = transform.position;
        oldPos = new Vector3(transform.position.x - 0.01f, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        newPos = currPos + (currPos - oldPos);
        oldPos = transform.position;
        transform.position = newPos;
        currPos = transform.position;
    }
}
