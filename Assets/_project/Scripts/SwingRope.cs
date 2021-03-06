﻿using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SwingRope : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject pointerObject;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ropeOriginPoint;

    [SerializeField] private float ropeStiffness = 300;
    [SerializeField] private float ropeRetractSpeed = 4f;

    LineRenderer line;

    private Transform targetTransform;
    Vector3 ropePoint;
    
    float lineWidthOrig;

    private bool ropeEnabled;
    private Vector3 ropeEndPoint;
    private float maxDistance;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        
        lineWidthOrig = line.endWidth;
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
                pointerObject.SetActive(true);
            if (uiManager != null && !uiManager.ReticuleTarget)
            {
                uiManager.SwitchReticule();
            }
            pointerObject.transform.position = hit.point + (hit.normal / 16);
        }
        else
        {
            if (pointerObject.activeSelf)
                pointerObject.SetActive(false);
            if (uiManager != null && uiManager.ReticuleTarget)
            {
                uiManager.SwitchReticule();
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
        
        // TODO Make the rope move with the object it hit/disconnect when it disapperars
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, 100, finalMask) && Input.GetButtonDown("Fire1"))
        {
            ropePoint = hit.point;
            SpringJoint spring = player.AddComponent<SpringJoint>();
            spring.anchor = ropeOriginPoint.transform.localPosition;
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedAnchor = hit.point;
            spring.spring = ropeStiffness;

            float distanceFromPoint = Vector3.Distance(player.transform.position, hit.point);
            maxDistance = distanceFromPoint;
            ropeEndPoint = hit.point;
            ropeEnabled = true;
            
            spring.maxDistance = distanceFromPoint;
            spring.minDistance = distanceFromPoint * 0.15f;

            spring.damper = 7f;

            targetTransform = hit.transform;
        }

        if (targetTransform != null)
        {
            
        }
        if (Input.GetButton("Fire2") && player.GetComponent<SpringJoint>() != null)
        {
            RetractRope();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            ropeEnabled = false;
            targetTransform = null;
            StartCoroutine(LineFade());
            Destroy(player.GetComponent<SpringJoint>());
        }
    }

    private void RetractRope()
    {
        if (maxDistance > 1f) maxDistance -= ropeRetractSpeed * Time.deltaTime;
        player.GetComponent<SpringJoint>().maxDistance = maxDistance;
    }
    private void DrawRope()
    {
        if (player.GetComponent<SpringJoint>() != null)
        {
            StopCoroutine(LineFade());
            line.enabled = true;
            line.SetPosition(0, ropeOriginPoint.transform.position);
            UpdateRopeColor();
            StartCoroutine(LineDraw());
        }
    }

    private float GetRopeLength()
    {
        return (player.transform.position - ropeEndPoint).magnitude / maxDistance;
    }

    private void UpdateRopeColor()
    {
        // TODO: This is pretty bad, fix pls
        Color newColor = Color.HSVToRGB(1f, 1f, Mathf.Clamp01(Mathf.Pow(GetRopeLength(), 100)));
        line.material.color = newColor;
        line.material.SetColor("_EmissionColor", newColor * 2);
        //new Color(GetRopeLength() * 20, 20, 20);
    }
    
    IEnumerator LineDraw()
    {
        line.startWidth = lineWidthOrig;
        line.endWidth = lineWidthOrig;
        
        float time = 0.2f;
        Vector3 orig = ropeOriginPoint.transform.position;
        Vector3 orig2 = ropePoint;
        line.SetPosition(1, orig);
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            var newpos = Vector3.Lerp(orig, orig2, t / time);
            line.SetPosition(1, newpos);
            yield return null;
        }
        line.SetPosition(1, orig2);
    }

    IEnumerator LineFade()
    {
        float time = 0.5f;
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            line.endWidth = Mathf.Lerp(lineWidthOrig, 0, t/time);
            line.startWidth = Mathf.Lerp(lineWidthOrig, 0, t/time);
            yield return null;
        }
        line.enabled = false;
    }
}
