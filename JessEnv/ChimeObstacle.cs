using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimeObstacle : MonoBehaviour {
    public ChimeOrder finalChime;
    public float speed;
    //private float obsY;
    public GameObject pickupPrefab;

    // Audio
    AudioSource source;
    public AudioClip correctChime;

	// Use this for initialization
	void Start () {
        //obsY = transform.position.y;
        //source = GameObject.FindGameObjectWithTag("ChimeAudio").GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (finalChime.GetRung())
        {
            //Debug.Log("CORRECT CHIMES");
            //float step = speed * Time.deltaTime;
            //stransform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, obsY+transform.localScale.y), step);
            Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            pickupPrefab.GetComponent<Animation>().Play();
            Destroy(gameObject);
        }	
	}
}
