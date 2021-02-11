using _project.Scripts.etc;
using _project.Scripts.Player;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    // References
    [SerializeField] private GameManager gameManager;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private Material playerMaterial;

    private UIManager uiManager;

    private Transform playerTransform;
    private CapsuleCollider playerCollider;

    private void Start()
    {
        // uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    //All the Input capturing done in Update
    private void Update()
    {
        if (gameManager.PlayerMovement)
            inputHandler.PlayerInput();
        else
            inputHandler.ZeroMovement();

        //DebugUIUpdate();
    }

    // All the rigidbody interactions done in FixedUpdate
    private void FixedUpdate()
    {
        movementHandler.PlayerMovement();
    }
    
    // NOT IN USE
    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable == null) return;
        interactable.Interact();
    }
}
/*
    private void DebugUIUpdate()
    {
        uiManager.SetDebugUI(1, "Vel: " + sumOfVelocityXZ.ToString("F2") + " AirDir: " + inputHandler.airMovement.normalized +
                                " VelDir:" + movementHandler.rb.velocity.normalized
                                + "\nDrag: " + movementHandler.rb.drag.ToString("F2") + " ASpeed: " + 0);
        var position = playerTransform.position;
        //Debug.DrawRay(position, inputHandler.airMovement.normalized, Color.yellow);
        //Debug.DrawRay(position, movementHandler.rb.velocity.normalized, Color.red);

        //IsGrounded function debugging
        // playerMaterial.color = movementHandler.isGrounded ? Color.green : Color.red;
    }
}
*/