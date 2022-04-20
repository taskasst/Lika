using UnityEngine;

public class Fall : MonoBehaviour
{
    // The spike game object that will fall
    public GameObject spike;

    // If this spike can respawn or not
    private bool respawn = false;

    // The initial spike position
    private Vector3 position;

    private FadeDisappear fadeDis;

    private void Start()
    {
        // Get the initial position of the spike for respawning
        position = spike.transform.position;
        fadeDis = spike.GetComponent<FadeDisappear>();
    }

    private void Update()
    {
        if (respawn)
        {
            // Wait until the spike has become invisible
            if(fadeDis.IsInvisible())
            {
                spike.GetComponent<Rigidbody>().useGravity = false;
                spike.GetComponent<Rigidbody>().isKinematic = true;
                spike.GetComponent<CapsuleCollider>().enabled = true;
                spike.transform.position = position;
                respawn = false;
                fadeDis.SetVisible();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if ((other.tag == "LeafSwarm" || other.tag == "Player") && !respawn)
        {
            // The swarm hit the trigger, drop the spike
            respawn = true;
            spike.GetComponent<Rigidbody>().useGravity = true;
            spike.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
