﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRope : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject pointerObject;
    public GameObject player;

    LineRenderer line;

    Vector3 ropePoint;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }
    private void Update()
    {
        PointerFunction();
        SpringRope();
        DrawRope();
    }
    void PointerFunction()
    {
        // Bit shift the index of the layer(8) and layer(2) to get a bit mask
        int layerMask1 = 1 << 8;
        int layerMask2 = 1 << 2;
        int finalMask = layerMask1 | layerMask2;

        // We want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        finalMask = ~finalMask;

        RaycastHit hit;
        // For pointer stuff... Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100, finalMask))
        {
            if (!pointerObject.activeSelf)
            {
                Debug.Log("Pointer on");
                pointerObject.SetActive(true);
                //hit.normal
            }
            //Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // pointerObject.transform.position = Vector3.Lerp(pointerObject.transform.position, hit.point + (hit.normal / 16), Time.deltaTime * 80);
            pointerObject.transform.position = hit.point + (hit.normal / 16);
        }
        else
        {
            if (pointerObject.activeSelf)
            {
                Debug.Log("Pointer off");
                pointerObject.SetActive(false);
            }
        }
    }
    private void SpringRope()
    {
        int layerMask1 = 1 << 8;
        int layerMask2 = 1 << 2;
        int finalMask = layerMask1 | layerMask2;

        finalMask = ~finalMask;

        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100, finalMask) && Input.GetButtonDown("Fire1"))
        {
            Debug.Log(hit.transform.gameObject.name);
            ropePoint = hit.point;
            SpringJoint spring = player.AddComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedAnchor = hit.point;
            spring.spring = 200;

            float distanceFromPoint = Vector3.Distance(player.transform.position, hit.point);

            spring.maxDistance = distanceFromPoint;
            spring.minDistance = distanceFromPoint * 0.15f;

            spring.damper = 7f;

            
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Destroy(player.GetComponent<SpringJoint>());
        }
    }
    private void DrawRope()
    {
        if (player.GetComponent<SpringJoint>() != null)
        {
            line.enabled = true;
            line.SetPosition(0, player.transform.position);
            line.SetPosition(1, ropePoint);
        }
        else
            line.enabled = false;
    }
}
