using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTest : MonoBehaviour
{
    public GameObject ropeSegment;
    public GameObject[] allSegments;
    public int ropeLength = 10;
    public Vector3 offset = Vector3.zero;

    BoxCollider interactionPoint;
    private GameObject player;
    private UIManager uiManager;
    [SerializeField]
    private bool interactable = false;

    void Start()
    {
        allSegments = new GameObject[ropeLength];
        /********************************************************/
        interactionPoint = GetComponent<BoxCollider>();
        player = GameObject.FindGameObjectWithTag("Player");
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        /********************************************************/

        GenerateRope();
    }

    void GenerateRope()
    {
        GameObject prevRopeSegment = ropeSegment;
        for (int i = 0; i < ropeLength; i++)
        {
            allSegments[i] = Instantiate(ropeSegment, this.transform);
            ConfigurableJoint newSpring = allSegments[i].GetComponent<ConfigurableJoint>();
            if (prevRopeSegment != null)
            {
                allSegments[i].transform.position = prevRopeSegment.transform.position + transform.right + offset;
                newSpring.connectedBody = prevRopeSegment.GetComponent<Rigidbody>();    
            }
            else
                Debug.Log("No previous rope segment found!");

            prevRopeSegment = allSegments[i];
        }
        /********************************************************/
        //interactionPoint.center = allSegments[allSegments.Length - 1].transform.position;
        /********************************************************/

    }
    public void ActivateRope()
    {
        for (int i = 0; i < allSegments.Length; i++)
        {
            allSegments[i].GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    //TESTING...
    /********************************************************//********************************************************/
    void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E) && player.GetComponent<ConfigurableJoint>() == null)
        {
            ConfigurableJoint spring = player.AddComponent<ConfigurableJoint>();
            spring.xMotion = ConfigurableJointMotion.Locked;
            spring.yMotion = ConfigurableJointMotion.Locked;
            spring.zMotion = ConfigurableJointMotion.Locked;
            //spring.projectionMode = JointProjectionMode.PositionAndRotation;
            spring.connectedMassScale = 1.5f;
            spring.connectedBody = allSegments[allSegments.Length - 1].GetComponent<Rigidbody>();
            ActivateRope();
        }
        if (Input.GetKeyDown(KeyCode.Q) && player.GetComponent<ConfigurableJoint>() != null)
        {
            Destroy(player.GetComponent<ConfigurableJoint>());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.SetInteractPrompt(true);
            interactable = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiManager.SetInteractPrompt(false);
            interactable = false;
        }
    }
    /********************************************************//********************************************************/
}
