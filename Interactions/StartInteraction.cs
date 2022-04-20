using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartInteraction : MonoBehaviour {
    

    public Rigidbody rigidInteractive;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}




    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Snowball") //i tag moveable things as pickup 
        {
            Destroy(other.GetComponent<VectorFieldMovement>());
            Destroy(other.GetComponent<Rigidbody>());
            if (rigidInteractive.gameObject.GetComponent<LoopAnimation>() && !rigidInteractive.gameObject.GetComponent<LoopAnimation>().animationChange)
            {
                Destroy(rigidInteractive.gameObject.GetComponent<LoopAnimation>());
            }
            if (rigidInteractive.gameObject.GetComponent<InteractionLayout>())
            {
                rigidInteractive.gameObject.GetComponent<InteractionLayout>().SetPlayInteract(true);
            }
        }
    }
}
