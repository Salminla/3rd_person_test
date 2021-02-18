using System;
using UnityEngine;

public class LevelEndPoint : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private bool triggerEntered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!triggerEntered)
        {
            Debug.Log("Enter end trigger");

            triggerEntered = true;
            
            if (gameManager != null)
                gameManager.EndLevel();
            else
                Debug.Log("Game Manager not found!");
        }
    }
}
