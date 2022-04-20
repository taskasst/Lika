using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropIce : MonoBehaviour {

    private Vector2 icePos;
    public float spawnOffset;
    public VectorFieldMovement leafswarm;
    public LeafManager leafManager;
    public float fallRestart = 0.3f;
    private float time = 0f;
    private float fallTime = 0f;
    private bool fall = false;
    private bool respawn = false;
    public GameObject iceDropPrefab;
    Camera mainCamera;

    private bool leafstop = false;
    public float stopDone = 3.0f;
    private float stopTime = 0f;
    public GameObject leaf;
    private float dropx;

    private FadeDisappear fadeDis;

    public ParticleSystem destroyParticles;
    public float particleOffset = 0.0f;


    // Use this for initialization
    void Start()
    {
        //Debug.Log(transform.localScale);
        icePos = new Vector2(transform.position.x, transform.position.y);
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        dropx = 0.0f;
        fadeDis = GetComponent<FadeDisappear>();
    }

    // Update is called once per frame
    void Update()
    {
        //if water hits, slow down the leafswarm for a specified time.
        if (leafstop)
        {
            Debug.Log("slow");
            stopTime += Time.deltaTime;
            if (leafManager.numPickups > 1)
            {
                leafManager.UpdateSwarmSize(false);
                Instantiate(iceDropPrefab, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
            leafswarm.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            //leaf.transform.GetComponent<Renderer>().material.color = Color.blue;
            if (stopTime >= stopDone)
            {
                Debug.Log("normal");
                leafswarm.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //leaf.transform.GetComponent<Renderer>().material.color = Color.blue;
                leafstop = false;
                stopTime = 0;
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

        if (respawn)
        {
            if (fadeDis.IsInvisible())
            {
                GetComponent<Rigidbody>().useGravity = false;
                dropx = Random.Range(icePos.x + spawnOffset, icePos.x - spawnOffset);
                transform.position = new Vector2(dropx, icePos.y);
                fall = false;
                fallTime = 0;
                fadeDis.SetVisible();
                GetComponent<CapsuleCollider>().enabled = true;
                respawn = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "LeafSwarm" || collision.collider.tag == "Floor" || collision.collider.tag == "PickUp")
        {
            if (collision.collider.tag == "LeafSwarm")
            {
                leafstop = true;
            }
            else if (collision.collider.tag == "Floor")
            {
                GetComponent<Rigidbody>().useGravity = false;
            }

            fadeDis.StartFadingObj();

            GetComponent<CapsuleCollider>().enabled = false;
            respawn = true;

            Vector3 visTest = mainCamera.WorldToViewportPoint(transform.position);
            //Check if visible
            if ((visTest.x >= 0 && visTest.y >= 0) && (visTest.x <= 1 && visTest.y <= 1) && visTest.z >= 0)
            {
                AudioManager.Instance.PlaySnowflakeHit();

                // Play particle effect on hit
                if (destroyParticles != null)
                {
                    Vector3 particleSpawnPosition = new Vector3(transform.position.x, transform.position.y + particleOffset, transform.position.z);
                    Instantiate(destroyParticles, particleSpawnPosition, Quaternion.identity);
                }
            }
        }
    }
}
