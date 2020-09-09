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
        // Bit shift the index of the layer(8) and layer(2) to get a bit mask
        int layerMask1 = 1 << 8;
        int layerMask2 = 1 << 2;
        int finalMask = layerMask1 | layerMask2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        finalMask = ~finalMask;

        Vector3 desiredCamPos = transform.parent.TransformPoint(dollyDir * 4.0f);
        RaycastHit hit;

        if (Physics.Linecast (transform.parent.position, desiredCamPos, out hit, finalMask ))
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
