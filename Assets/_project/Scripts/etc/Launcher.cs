using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public float launchSpeed = 10;
    public float launchTime = 1;

    public bool launchActive = true;

    Rigidbody rb;
    Vector3 origPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        origPos = gameObject.transform.position;
        StartCoroutine(LaunchTimer());
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.Translate(Vector3.back * launchSpeed * Time.deltaTime);
        rb.MovePosition(transform.position + -transform.forward * launchSpeed * Time.deltaTime);
    }

    IEnumerator LaunchTimer()
    {
        while (launchActive)
        {
            yield return new WaitForSeconds(launchTime);
            gameObject.transform.position = origPos;
        }
    }
}
