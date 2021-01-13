using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TrailRenderer playerTrail;
    
    [SerializeField] private float fovFactor = 180;
    [SerializeField] private float maxFov = 110;

    [SerializeField] private bool velocityLineEnabled = true;

    private float defaultFov;
    
    private MovementHandler movementHandler;
    private LineRenderer velocityLine;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        velocityLine = GetComponent<LineRenderer>();
        movementHandler = GetComponent<MovementHandler>();

        defaultFov = mainCamera.fieldOfView;
    }

    private void Update()
    {
        UpdateFov();
        
        if (playerTrail != null)
            UpdateTrail();
        
        if (velocityLineEnabled && movementHandler.SumOfVelocityXYZ > 8.5f)
            DrawVelocityLine();
        else
            velocityLine.enabled = false;
    }

    private void DrawVelocityLine()
    {
        velocityLine.enabled = true;
        float time = 0.05f;
        
        for (int i = 0; i < velocityLine.positionCount; i++)
        {
            velocityLine.SetPosition(i, GetPosAtTime(time));
            time += 0.1f;
        }
    }

    private void UpdateFov()
    {
        float t = Mathf.InverseLerp( 1, fovFactor, movementHandler.SumOfVelocityXYZ )+1;
        float newFov = defaultFov * t;
        float clampedFov = Mathf.Clamp(newFov, defaultFov, maxFov);

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, clampedFov, .06f);
    }

    private void UpdateTrail()
    {
        float trailTime;
        if (movementHandler.SumOfVelocityXYZ > 15)
            trailTime = movementHandler.SumOfVelocityXYZ / 15;
        else
            trailTime = Mathf.Lerp(playerTrail.time, 0, 0.01f);

        float trailTimeClamped = Mathf.Clamp(trailTime, 0, 3);

        playerTrail.time = trailTimeClamped;
    }
    
    private Vector3 GetPosAtTime(float time)
    {
        return new Vector3(transform.position.x + rb.velocity.x * time,
                           transform.position.y + rb.velocity.y * time + 0.5f * Physics.gravity.y * time * time,
                           transform.position.z + rb.velocity.z * time);
    }
}