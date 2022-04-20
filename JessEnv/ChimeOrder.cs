using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimeOrder : MonoBehaviour
{
    public bool rungCorrectly;
    public int chimeNumber = 1;
    public ChimeOrder previousChime;

    private Animation anim;
    private Behaviour halo;
    // Audio manager
    private AudioManager audioManager;

    private float timeRing = 0f;

    // Use this for initialization
    private void Start()
    {
        halo = (Behaviour)gameObject.GetComponent("Halo");
        halo.enabled = false;
        audioManager = AudioManager.Instance;
        anim = gameObject.GetComponent<Animation>();
    }

    private void Update()
    {
        if(timeRing > 0)
        {
            timeRing -= Time.deltaTime;
        }
    }

    public void SetRung()
    {
        anim.Play("ChimeAnimation");
        if (previousChime == null /*&& rungCorrectly == false*/) //if no previous chime set, it the first chime and set to true
        {
            //Debug.Log(rungCorrectly);
            //Debug.Log("First chime");
            if(rungCorrectly==false)
            {
                /*anim["GlowGrow"].speed = 1;
                anim["GlowGrow"].time = 0;
                anim.Play("GlowGrow");*/
                halo.enabled = true;
                rungCorrectly = true;
                transform.Rotate(Vector3.up * Time.deltaTime, Space.World);
                //Debug.Log("First chime");
                // Ring only once
                audioManager.PlayChime(chimeNumber);
            }
 
        }
        else if (previousChime != null &&  previousChime.GetRung() == true /*&& rungCorrectly == false*/)
        {
            //Debug.Log("Next chime");
            if (rungCorrectly==false)
            {
                /*anim["GlowGrow"].speed = 1;
                anim["GlowGrow"].time = 0;
                anim.Play("GlowGrow");*/
                halo.enabled = true;
                rungCorrectly = true; // Ring only once
                audioManager.PlayChime(chimeNumber);
            }
        }
        else
        {
            Debug.Log("Wrong chime");
            rungCorrectly = false;
            previousChime.RecursiveUnsetChimes();
            if (timeRing <= 0)
            {
                /*anim["GlowGrow"].speed = -1;
                anim["GlowGrow"].time = previousChime.anim["GlowGrow"].length;
                anim.Play("GlowGrow");*/
                halo.enabled = false;
                // to prevent repetative audio
                audioManager.PlayChimeWrong();
                timeRing = 0.5f;
            }
        }
    }

    //if one chime is wrong set all the previous chimes must be false
    private void RecursiveUnsetChimes()
    {
        rungCorrectly = false;
        if (previousChime != null) //if previous chime is set and every is false than this is false
        {
            previousChime.RecursiveUnsetChimes();
            /*previousChime.anim["GlowGrow"].speed = -1;
            previousChime.anim["GlowGrow"].time = previousChime.anim["GlowGrow"].length;
            previousChime.anim.Play("GlowGrow");*/
            previousChime.halo.enabled = false;
        }
    }

    public bool GetRung()
    {
       return rungCorrectly;
    }
    
	
	// Update is called once per frame
	/*void Update () {
        //want to set all previous chimes to false here
        if (previousChime != null) //if previous chime is set and every is false than this is false
        {
            if (previousChime.GetRung() == false)
            {
                rungCorrectly = false;
            }
        }
    }*/
}
