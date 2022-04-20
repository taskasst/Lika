using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAnimation : MonoBehaviour {
    Animation anim;
    public float startAfter = 0;
    private float time = 0;
    public string startAnimation;
    public bool animationChange = false;
    public string secondAnimation;
    public GameObject reactionObject;
    public bool DestroyAfterSecond = false;
	// Use this for initialization
	void Start () {
        anim = gameObject.GetComponent<Animation>();
    }
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        //animation change should not be set unless there is another animation to play, so we should not run into compiler issues with the gameobject retieval
        if (animationChange && !reactionObject.GetComponent<InteractionLayout>() && !anim.IsPlaying(startAnimation))//if animation change is set to true by user and the reaction objects interactive script has been deleted AND the previous animation is done: change the current animation
        {
            anim.Play(secondAnimation);
            if (DestroyAfterSecond)
            {
                Destroy(this);
            }
        }
        else if (time > startAfter)
        {
            if (startAnimation == "")
            {
                anim.Play();
            }
            else
            {
                anim.Play(startAnimation);
            }
            
        }
        
	}
}
