using _project.Scripts.etc;
using UnityEngine;

public class BoostGate : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject direction;
    [SerializeField] private float boostForce = 100;
    [SerializeField] private bool destroyOnEnter = true;

    private void Boost(GameObject obj)    
    {
        if (obj.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("You got a boost!");
            obj.GetComponent<Rigidbody>().AddForce(GetDirection() * -boostForce, ForceMode.Impulse);
            if (destroyOnEnter)
                Destroy(this.gameObject);
        }
    }

    private Vector3 GetDirection()
    {
        return (transform.position - direction.transform.position).normalized;
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