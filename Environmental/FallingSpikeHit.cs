using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpikeHit : MonoBehaviour
{
    public ParticleSystem particles;
    public float particleOffset = 0.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Floor" || other.tag == "LeafSwarm")
        {
            GetComponent<FadeDisappear>().StartFadingObj();
            AudioManager.Instance.PlaySpikeHit();
            if(other.tag == "Floor")
            {
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<CapsuleCollider>().enabled = false;
            }

            // Play particle effect on hit
            if (particles != null)
            {
                Vector3 particleSpawnPosition = new Vector3(transform.position.x, transform.position.y + particleOffset, transform.position.z);
                Instantiate(particles, particleSpawnPosition, Quaternion.identity);
            }
        }       
    }
}
