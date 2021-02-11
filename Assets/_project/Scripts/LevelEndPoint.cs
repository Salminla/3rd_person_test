using System;
using UnityEngine;

public class LevelEndPoint : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    private void OnTriggerEnter(Collider other)
    {
        if (gameManager != null)
            gameManager.EndLevel();
        else
            Debug.Log("Game Manager not found!");
        
    }
}
