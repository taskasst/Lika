using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafManager : MonoBehaviour
{
    // Leaf meshes in the swarm (First element is the particle system)
    public List<GameObject> leaves;

    // Pickup to spawn if a leaf is lost
    public GameObject pickupPrefab;
    // Used to make sure leaf doesn't spawn in something else
    public LayerMask layerMask;
    public float radius;

    // Used to make sure player doesn't drop multiple leaves at once
    public float invTime = 1f;
    private float currTime;
    private bool invincible = false;

    // Used to make sure player doesn't pick up multiple leaves if there's only one
    private float pickupTime = 0.1f;
    private float currPickupTime;
    private bool pickupPrev = false;

    // Don't spawn leaves above and below
    public GameObject ceiling;
    public GameObject floor;
    private float roofValue;
    private float floorValue;

    // Current number of pickups the player has
    public int numPickups = 1;
    // Max num pickups in the scene
    public int maxPickups = 0;
    // Number of active leaf meshes in the swarm
    //private int activeLeaves = 0;

    // Color Manager that changes the scene colors
    private ColorManager colorManager;
    // The audio manager for all sounds/music
    private AudioManager audioManager;

    private ScoreKeeper scoreKeeper;
    
    /* Sets the number of pickups the player has collected */
    public void SetPickups(int pickups) { numPickups = pickups; }

    /* Gets the number of pickups the player has collected */
    public int GetPickups() { return numPickups; }

    void Start()
    {
        audioManager = AudioManager.Instance;
        colorManager = ColorManager.cm;
        scoreKeeper = ScoreKeeper.skInstance;

        //set roof and floor value
        roofValue = ceiling.transform.position.y;
        floorValue = floor.transform.position.y;
        
        // Initialize list of child leaf meshes
        Transform[] leafTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform tran in leafTransforms)
        {
            if (tran.parent == transform)
            {
                tran.gameObject.SetActive(false);
                leaves.Add(tran.gameObject);
            }
        }

        // initialize the starting leaves
        for (int i = 0; i <= numPickups; i++)
        {
            leaves[i].SetActive(true);
        }

        currTime = invTime;
        currPickupTime = pickupTime;
        ScoreKeeper.skInstance.SetSeasonMax(maxPickups);
    }

    void Update()
    {
        if(invincible)
        {
            currTime -= Time.deltaTime;
            if(currTime <= 0)
            {
                invincible = false;
                currTime = invTime;
            }
        }

        if(pickupPrev)
        {
            currPickupTime -= Time.deltaTime;
            if(currPickupTime <= 0)
            {
                pickupPrev = false;
                currPickupTime = pickupTime;
            }
        }

        // Have leaves in swarm follow main collider, which is affected by vector field
        foreach (GameObject leaf in leaves)
        {
            leaf.transform.position = Vector3.MoveTowards(gameObject.GetComponent<Collider>().transform.position, transform.position, 0.3f);
        }
    }

    /* Sets number of active leaf meshes to match number of collected pickups */
    public void UpdateSwarmSize(bool add)
    {
        if(add)
        {
            // add a leaf
            numPickups += 1;
            leaves[numPickups].SetActive(true);

            audioManager.PlayGainLeaf();
            audioManager.PlayGainLeafMusic();
            audioManager.AddTrack(numPickups);
            scoreKeeper.UpdateScore(numPickups);
        }
        else
        {
            // remove a leaf
            audioManager.PlayLoseLeaf();
            audioManager.PlayLoseLeafMusic();
            audioManager.RemoveTrack(numPickups);

            leaves[numPickups].SetActive(false);
            numPickups -= 1;
            scoreKeeper.UpdateScore(numPickups);
        }

        // Update the number of leaves we have
        //activeLeaves = numPickups;
        colorManager.UpdateColors();
    }

    /* Stop swarm movement and have leaves settle on the ground */
    public void SettleOnGround()
    {
        for (int i = 1; i < leaves.Count; i++)
        {
            leaves[i].GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    //leaf manager now has collider that determines leaf location, so it will also deal with the swarm
    void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "Spike" || other.tag == "FallingSpike") && !invincible)
        {
            HitSpike();
        }
        else if (other.tag == "PickUp" && !pickupPrev)
        {
            // Collided with a leaf pick up, pick it up
            Destroy(other.gameObject);
            pickupPrev = true;
            UpdateSwarmSize(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Spike" || collision.gameObject.tag == "FallingSpike") && !invincible)
        {
            HitSpike();
        }
    }

    private void HitSpike()
    {
        if (numPickups > 1)
        {
            // Hit a spike, drop a leaf if carrying more than 1
            AudioManager.Instance.PlaySpikeHit();
            invincible = true;
            pickupPrev = true;
            UpdateSwarmSize(false);

            bool startLoop = true;
            Vector3 leafpos = new Vector3(transform.position.x, transform.position.y, 0f);
            while (startLoop)
            {
                leafpos = Random.insideUnitCircle * 5 + new Vector2(transform.position.x, transform.position.y);
                leafpos = new Vector3(leafpos.x, leafpos.y, 0f);
                if (leafpos.y < roofValue && leafpos.y > floorValue)
                {
                    // This ensures that the leaf is within the bounds of the game.
                    Collider[] colliders = Physics.OverlapSphere(leafpos, radius, layerMask);
                    if (colliders.Length == 0)
                    {
                        // Only finish loop if pickup won't spawn in something else
                        startLoop = false;
                    }
                }
            }
            Instantiate(pickupPrefab, leafpos, Quaternion.identity);
        }
    }
}