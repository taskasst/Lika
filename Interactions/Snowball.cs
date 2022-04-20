using UnityEngine;

public class Snowball : MonoBehaviour
{
    public GameObject leafPrefab;

    public GameObject particlePrefab;

    [Tooltip("The max size the snowball can become")]
    public float maxSize = 5f;

    [Tooltip("The size the snowball should be in order to break")]
    public float sizeInteract = 3f;

    [Tooltip("The velocity the snowball should be moving in order to grow")]
    public float velToGrow = 2f;

    [Tooltip("The velocity the snowball should be moving in order to break")]
    public float velToBreak = 5f;

    // The rigidbody on this snowball
    private Rigidbody rb;

    private float currSize;

    private bool audioPlaying = false;

    //AudioManager
    private AudioManager audioManager;

	void Start ()
    {
        if(!GetComponent<Rigidbody>())
        {
            Debug.LogError("No rigidbody on this snowball!");
        }
        else
        {
            rb = GetComponent<Rigidbody>();
        }

        currSize = transform.localScale.x;
        audioManager = AudioManager.Instance;
    }
	
    
	void Update ()
    {
        if(rb.velocity.magnitude > 2 && !audioPlaying)
        {
            audioManager.PlaySnowballRoll();
            audioPlaying = true;
        }
        else if (rb.velocity.magnitude == 0 && audioPlaying)
        {
            audioManager.StopSnowballRoll();
            audioPlaying = false;
        }
		// As the snowball rolls, make it grow
        if(rb.velocity.magnitude > velToGrow)
        {
            if(currSize < maxSize)
            {
                // Only grow to a certain size
                currSize += Time.deltaTime;
                transform.localScale = new Vector3(currSize, currSize, currSize);
            }
        }

	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rock" || collision.gameObject.tag == "Floor")
        {
            if (collision.relativeVelocity.magnitude > velToBreak && currSize >= sizeInteract)
            {
                // Break the snowball
                Instantiate(leafPrefab, transform.position, Quaternion.identity);
                leafPrefab.GetComponent<Animation>().Play();
                Instantiate(particlePrefab, transform.position, Quaternion.identity);
                audioManager.PlaySnowBall();
                Destroy(gameObject);
                audioManager.StopSnowballRoll();
            }
        }
    }
}
