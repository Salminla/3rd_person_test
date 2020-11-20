using System;
using _project.Scripts;
using _project.Scripts.etc;
using _project.Scripts.Player;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    // References
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private Material playerMaterial;

    private UIManager uiManager;

    private Transform playerTransform;
    private CapsuleCollider playerCollider;
    
    public UnityEngine.UI.Text debugText1;
    public UnityEngine.UI.Text debugText2;
    
    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }
    //All the Input capturing done in Update
    private void Update()
    {
        inputHandler.PlayerInput();
        
        //DebugUIUpdate();
    }

    // All the rigidbody interactions done in FixedUpdate
    private void FixedUpdate()
    {
        movementHandler.PlayerMovement();       
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable == null) return;
        interactable.Interact();
    }

    /*private void DebugUIUpdate()
    {
        uiManager.SetDebugUI(1, "Vel: " + sumOfVelocityXZ.ToString("F2") + " AirDir: " + inputHandler.airMovement.normalized +
                                " VelDir:" + movementHandler.rb.velocity.normalized
                                + "\nDrag: " + movementHandler.rb.drag.ToString("F2") + " ASpeed: " + 0);
        var position = playerTransform.position;
        Debug.DrawRay(position, inputHandler.airMovement.normalized, Color.yellow);
        Debug.DrawRay(position, movementHandler.rb.velocity.normalized, Color.red);

        //IsGrounded function debugging
        if (movementHandler.isGrounded)
            playerMaterial.color = Color.green;
        else
            playerMaterial.color = Color.red;
    }*/
}