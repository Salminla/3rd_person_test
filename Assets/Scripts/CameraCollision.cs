using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    Vector3 dollyDir;
    float distance;

    // Start is called before the first frame update
    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCamPos = transform.parent.TransformPoint(dollyDir * 4.0f);
        RaycastHit hit;

        if (Physics.Linecast (transform.parent.position, desiredCamPos, out hit))
        {
            distance = Mathf.Clamp(hit.distance * 0.8f, 1.0f, 4.0f);
        }
        else
        {
            distance = 4.0f;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * 10.0f);
    }
}
