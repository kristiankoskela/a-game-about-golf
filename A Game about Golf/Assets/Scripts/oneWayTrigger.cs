using UnityEngine;

public class oneWayTrigger : MonoBehaviour
{
    private GameObject parent;

    public enum TriggerType
    {
        Block,
        Pass
    }

    public TriggerType triggerType;
    public void Start()
    {
        parent = transform.parent.gameObject;
        DisableCollision();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (triggerType == TriggerType.Block)
            {
                EnableCollision();
            }
            else if (triggerType == TriggerType.Pass)
            {
                DisableCollision();
            }
        }
    }

    public void EnableCollision()
    {
        parent.GetComponent<Collider>().enabled = true;
    }

    public void DisableCollision()
    {
        parent.GetComponent<Collider>().enabled = false;
    }
}
