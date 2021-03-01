using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TitleGrabber : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponentInChildren<TMP_Text>() != null)
        {
            var textElement = GetComponentInChildren<TMP_Text>();
            textElement.text = gameObject.name;
        }
    }
}
