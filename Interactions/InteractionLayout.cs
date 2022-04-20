using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionLayout : MonoBehaviour {
    public bool playInteract;
    private Animation anim;
    public string animationString;
    public GameObject spawnLeaf;
    public GameObject pickupPrefab;
    public string interaction;
    public float delayLeafSpawn = 0; //set to animation time basically
    private float time = 0;

    private bool squirrelIsPlaying = false;

    // Audio manager
    private AudioManager audioManager;

    // Use this for initialization
    void Start()
    {
        anim = gameObject.GetComponent<Animation>();
        playInteract = false;
        if (this.GetComponent<ParticleSystem>())
        {
            this.GetComponent<ParticleSystem>().Stop();
        }
        audioManager = AudioManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (playInteract)
        {
            time += Time.deltaTime;
            if (this.GetComponent<ParticleSystem>())
            {
                this.GetComponent<ParticleSystem>().Play();
            }
            if (interaction == "squirrel" && !squirrelIsPlaying)
            {
                audioManager.PlaySnowBall();
                squirrelIsPlaying = true;
            }
            anim.Play(animationString);
            if (time >= delayLeafSpawn)
            {
                Instantiate(pickupPrefab, new Vector3(spawnLeaf.transform.position.x, spawnLeaf.transform.position.y, 0), Quaternion.identity);

                if (interaction == "bird")
                {
                    audioManager.PlayBirdTweet();
                }
                else if (interaction == "fish")
                {
                    audioManager.PlayWaterSplash();
                }
                else if (interaction == "bee")
                {
                    //audioManager.PlayBees();
                }
                
                else if (interaction == "pumpkin")
                {
                    Debug.Log("Kill me");

                    audioManager.PlayPlantGrow();
                }
                else if (interaction == "dandelion")
                {
                    audioManager.PlayPlantRustle();
                }
                else if (interaction == "snowman")
                {
                    audioManager.PlayPop();
                    audioManager.StopSnowballRoll();
                }
                audioManager.PlayHelpAnimal();
                pickupPrefab.GetComponent<Animation>().Play();
                Destroy(spawnLeaf);
                playInteract = false;
                Destroy(this);
            }
        }

    }

    public void SetPlayInteract(bool Boolean)
    {
        playInteract = Boolean;
    }
}
