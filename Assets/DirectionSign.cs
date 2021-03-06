using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionSign : MonoBehaviour
{
    [SerializeField] private bool animationState = true;
    [SerializeField] private float animationSpeed = 0.7f;
    
    private List<Transform> signs = new List<Transform>();
    
    void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            signs.Add(gameObject.transform.GetChild(i));
        }
        signs.Reverse();
        StartCoroutine(SignAnimation());
    }
    
    IEnumerator SignAnimation()
    {
        while (animationState)
        {
            foreach (var sign in signs)
            {
                //sign.GetComponent<MeshRenderer>().material.color = Color.red;
                sign.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red * 2);
                yield return new WaitForSeconds(animationSpeed);
            }

            foreach (var sign in signs)
            {
                // sign.GetComponent<MeshRenderer>().material.color = Color.white;
                sign.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
            }
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}
