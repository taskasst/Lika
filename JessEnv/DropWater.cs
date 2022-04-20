using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropWater : MonoBehaviour
{
    public enum DropType { Water, Spike };
    public DropType dropType = DropType.Water;

    private Vector2 rainPos;
    public float spawnOffset;
    public VectorFieldMovement leafswarm;
    public float fallRestart = 0.3f;
    private float time = 0f;
    private float fallTime = 0f;
    private bool fall = false;
    private bool respawn = false;
    public int divisionReduce = 2;
    public ParticleSystem particles;
    public float particleOffset = 0.0f;
    Camera mainCamera;

    private bool leafslowdown = false;
    public float slowdownDone = 3.0f;
    private float slowdownTime = 0f;
    private float leafmultipliernormal = 0;
    private float leafmultiplierslow = 0;
    private float dropx;

    // Audio manager
    private AudioManager audioManager;
    private FadeDisappear fadeDis;


    void Start ()
    {
        rainPos = new Vector2(transform.position.x, transform.position.y);
        leafmultipliernormal = leafswarm.forceMultiplier;
        leafmultiplierslow = leafswarm.forceMultiplier/divisionReduce;
        dropx = 0.0f;
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        audioManager = AudioManager.Instance;
        fadeDis = GetComponent<FadeDisappear>();
    }
    

    void Update()
    {
        //if water hits, slow down the leafswarm for a specified time.
        if (leafslowdown)
        {
            Debug.Log("slow");
            slowdownTime += Time.deltaTime;
            leafswarm.forceMultiplier = leafmultiplierslow;
            if (slowdownTime >= slowdownDone)
            {
                Debug.Log("normal");
                leafswarm.forceMultiplier = leafmultipliernormal;
                leafslowdown = false;
                slowdownTime = 0;
            }
        }

        if (!fall)
        {
            time += Time.deltaTime;
            if (time >= fallRestart)
            {
                GetComponent<Rigidbody>().useGravity = true;
                fall = true;
                time = 0;
            }
        }

        if(respawn)
        {
            if(fadeDis.IsInvisible())
            {
                dropx = Random.Range(rainPos.x + spawnOffset, rainPos.x - spawnOffset);
                transform.position = new Vector2(dropx, rainPos.y);
                fall = false;
                fallTime = 0;
                fadeDis.SetVisible();
                GetComponent<BoxCollider>().enabled = true;
                respawn = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "LeafSwarm" || collision.collider.tag == "Floor" || collision.collider.tag == "PickUp")
        {
            if(collision.collider.tag == "LeafSwarm")
            {
                leafslowdown = true;
            }
            else if (collision.collider.tag == "Floor")
            {
                GetComponent<Rigidbody>().useGravity = false;
            }

            fadeDis.StartFadingObj();
            
            GetComponent<BoxCollider>().enabled = false;
            respawn = true;
        }

        Vector3 visTest = mainCamera.WorldToViewportPoint(transform.position);
        //Check if visible
        if ((visTest.x >= 0 && visTest.y >= 0) && (visTest.x <= 1 && visTest.y <= 1) && visTest.z >= 0)
        {
            if (dropType == DropType.Water)
            {
                audioManager.PlayDropletDry(Random.Range(1, 3));
            }
            else if (dropType == DropType.Spike)
            {
                audioManager.PlaySpikeHit();
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
