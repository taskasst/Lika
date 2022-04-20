using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    // All the rigidbodies of the pieces
    private Rigidbody[] rbs;
    private GameObject[] objs;
    private float time;
    private bool byebyeBlock;
    
    private void Start()
    {
        // Get all the rigidbodies of the pieces
        rbs = gameObject.GetComponentsInChildren<Rigidbody>();
        // Make them all freeze for now
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
            rb.GetComponent<ParticleSystem>().Stop();
        }
        byebyeBlock = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Once a push block hits the wall, unfreeze the pieces
        if(other.tag == "PushBlock")
        {
            foreach(Rigidbody rb in rbs)
            {
                rb.isKinematic = false;
                byebyeBlock = true;
                rb.GetComponent<ParticleSystem>().Play();
                AudioManager.Instance.PlayWallExplode();
                FadeDisappear[] dis = GetComponentsInChildren<FadeDisappear>();
                foreach(FadeDisappear fd in dis)
                {
                    fd.StartFadingObj();
                }
            }
        }
    }
    private void Update()
    {
        if (byebyeBlock)
        {
            time += Time.deltaTime;
        }

        if(time >= 1)
        {
            Collider[] cols = GetComponentsInChildren<Collider>();
            foreach (Collider col in cols)
            {
                col.enabled = false;
            }
        }

        if (time >= 2)
        {
            //Debug.Log("Bye block");
            Destroy(this.gameObject);
        }
    }
}
