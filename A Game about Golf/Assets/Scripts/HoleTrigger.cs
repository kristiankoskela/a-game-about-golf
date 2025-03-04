using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    public bool stageComplete;

    private void Awake()
    {
        stageComplete = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            //Debug.Log("holeTrigger script detected stage completion.");
            stageComplete = true;
        }
    }
}
