using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumScript : MonoBehaviour
{
    public BoxCollider interactArea;
    public Rigidbody objectToActivate;

    private GameObject player;
    private UIManager uiManager;

    [SerializeField]
    private bool interactable = false;
    // Start is called before the first frame update
    void Start()
    {
        objectToActivate.isKinematic = true;
        player = GameObject.FindGameObjectWithTag("Player");
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable && Input.GetKeyDown(KeyCode.E) && player.GetComponent<SpringJoint>() == null)
        {
            objectToActivate.isKinematic = false;
            SpringJoint spring = player.AddComponent<SpringJoint>();
            spring.spring = 1500f;
            spring.connectedBody = objectToActivate;
        }
        if (Input.GetKeyDown(KeyCode.Q) && player.GetComponent<SpringJoint>() != null)
        {
            Destroy(player.GetComponent<SpringJoint>());
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
}
