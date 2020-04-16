using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    Transform endPoint;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.position = endPoint.position + endPoint.right / 1.5f;
    }
}
