using _project.Scripts.etc;
using UnityEngine;

public class PointGate : MonoBehaviour, IInteractable
{
    private void Award()
    {
        Debug.Log("You got points!");
        Destroy(gameObject);
    }

    public void Interact()
    {
        Award();
    }
}

