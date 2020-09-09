using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumScript : MonoBehaviour
{
    public BoxCollider interactArea;
    public Rigidbody objectToActivate;
    public GameObject attachPoint;
    public HingeJoint attachHinge;

    private GameObject player;

    [SerializeField]
    private bool interactable = false;
    // Start is called before the first frame update
    void Start()
    {
        objectToActivate.isKinematic = true;
        player = GameObject.FindGameObjectWithTag("Player");
        attachHinge = attachPoint.GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E))
        {
            objectToActivate.isKinematic = false;
            attachHinge.connectedBody = player.GetComponent<Rigidbody>();
        }
        if (attachHinge.connectedBody != null && Input.GetKeyDown(KeyCode.Q))
        {
            attachHinge.connectedBody = null;
            player.GetComponent<Rigidbody>().velocity = objectToActivate.velocity * 2;
            //player.GetComponent<Rigidbody>().velocity = objectToActivate.GetPointVelocity(transform.TransformPoint(attachPoint.transform.position));
            // player.GetComponent<Rigidbody>().velocity = player.GetComponent<Rigidbody>().GetPointVelocity(transform.TransformPoint(player.transform.position));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = false;
        }
    }
}
