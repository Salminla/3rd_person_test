using System;
using _project.Scripts.etc;
using UnityEngine;

public class BoostGate : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject direction;
    
    private void Boost(GameObject player)
    {
        Debug.Log("You got a boost!");
        player.GetComponent<Rigidbody>().AddForce(player.transform.right * -100, ForceMode.Impulse);
        Destroy(gameObject);
    }

    public void Interact()
    {
        //Boost();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Boost(other.gameObject);
    }
}