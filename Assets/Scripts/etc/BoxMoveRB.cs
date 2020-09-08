using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMoveRB : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField]
    private GameObject ObjectToMove;
    [SerializeField]
    private GameObject WaypointContainer;
    [SerializeField]
    private Transform[] waypoints;
    private int waypointIndex;
    private Vector3 targetPosition;
    [SerializeField]
    [Range(0, 2f)]
    private float moveSpeed = 1f;
    [SerializeField]
    private bool loop = true;
    private bool endReached = false;

    // Start is called before the first frame update
    void Start()
    {
        InitWaypoints();
        rb = ObjectToMove.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!endReached)
        {
            //rb.MovePosition(Vector3.forward);
            //rb.MovePosition(ObjectToMove.transform.position + ObjectToMove.transform.right * -Time.fixedDeltaTime);
            rb.MovePosition(Vector3.MoveTowards(ObjectToMove.transform.position, targetPosition, moveSpeed * Time.deltaTime));
            //ObjectToMove.transform.position = Vector3.MoveTowards(ObjectToMove.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(ObjectToMove.transform.position, targetPosition) < 0.25f)
            {
                if (waypointIndex >= waypoints.Length - 1)
                {
                    waypointIndex = 0;
                    if (!loop)
                        endReached = true;
                }
                else
                {
                    waypointIndex++;
                }
                targetPosition = waypoints[waypointIndex].position;
            }
        }
        //Allows to resume looping during runtime.
        if (endReached && loop)
            endReached = false;
    }

    void InitWaypoints()
    {
        //Dynamically create a array of the waypoints
        int wpCount = WaypointContainer.transform.childCount;
        waypoints = new Transform[wpCount];

        for (int i = 0; i < wpCount; i++)
        {
            waypoints[i] = WaypointContainer.transform.GetChild(i).transform;
        }
        targetPosition = waypoints[0].position;
    }
}
