using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public GameObject outPortal;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                GameObject outPortal = GameObject.FindGameObjectWithTag("OutPortal");
                rb.position = outPortal.transform.position;
            }
        }
    }
}
