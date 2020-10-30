using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text debugText;
    public Text debugText2;

    public Text interactPrompt;

    public void SetInteractPrompt(bool state)
    {
        interactPrompt.gameObject.SetActive(state);
    }

    public void SetDebugUI(int num, string content)
    {
        if (num == 1)
        {
            debugText.text = content;
        }
        else if (num == 2)
        {
            debugText2.text = content;
        }
        else
        {
            Debug.LogError("Invalid debug UI num!");
        }
    }
}
