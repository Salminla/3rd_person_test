using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPlayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Player"))
        {
            Debug.Log("On platform");
            other.transform.parent = transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.parent = null;
        }
    }
}
