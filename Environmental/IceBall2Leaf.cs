using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall2Leaf : MonoBehaviour {
    public int numHits2Break;
    private int currentHits;
    public GameObject pickupPrefab;

    // Use this for initialization
    void Start () {
        currentHits = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (currentHits >= numHits2Break)
        {
            Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentHits++;
        //Debug.Log("Num Hits: "+currentHits);

    }
}
