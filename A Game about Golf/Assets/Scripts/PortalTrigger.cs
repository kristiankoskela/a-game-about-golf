using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public GameObject outPortal;

    public AudioClip[] portalArray;
    public float portalVolume;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            int randomIndex = Random.Range(0, portalArray.Length);
            SoundManager.instance.PlaySoundClip(portalArray[randomIndex], transform, portalVolume);

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                GameObject outPortal = GameObject.FindGameObjectWithTag("OutPortal");
                rb.position = new Vector3(outPortal.transform.position.x, 0.5f, outPortal.transform.position.z);
            }
        }
    }
}
