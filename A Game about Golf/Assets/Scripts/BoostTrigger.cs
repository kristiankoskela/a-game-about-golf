using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BoostTrigger : MonoBehaviour
{
    public Vector3 boostDirection = Vector3.forward;
    public float boostPower = 10f;
    private BallController ballControllerScript;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            ballControllerScript = other.GetComponent<BallController>();
            if (rb != null && ballControllerScript != null)
            {
                ApplyBoost(rb);
                ballControllerScript.isIdle = false;
            }
        }
    }

    private void ApplyBoost(Rigidbody rb)
    {
        Quaternion parentRotation = transform.parent.rotation;
        Vector3 rotatedBoostDirection = parentRotation * boostDirection;

        rb.AddForce(rotatedBoostDirection * boostPower * Time.fixedDeltaTime);

        Debug.DrawRay(rb.transform.position, rotatedBoostDirection * 2, Color.red, 2f);
    }
}