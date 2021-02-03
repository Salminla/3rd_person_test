using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SwingRope : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject pointerObject;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ropeOriginPoint;

    LineRenderer line;

    Vector3 ropePoint;
    
    float lineWidthOrig;

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
            pointerObject.transform.position = hit.point + (hit.normal / 16);
        }
        else
        {
            if (pointerObject.activeSelf)
                pointerObject.SetActive(false);
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
            spring.spring = 300;

            float distanceFromPoint = Vector3.Distance(player.transform.position, hit.point);

            spring.maxDistance = distanceFromPoint;
            spring.minDistance = distanceFromPoint * 0.15f;

            spring.damper = 7f;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StartCoroutine(LineFade());
            Destroy(player.GetComponent<SpringJoint>());
        }
    }
    private void DrawRope()
    {
        if (player.GetComponent<SpringJoint>() != null)
        {
            StopCoroutine(LineFade());
            line.enabled = true;
            line.SetPosition(0, ropeOriginPoint.transform.position);
            StartCoroutine(LineDraw());
        }
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
