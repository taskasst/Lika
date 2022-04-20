using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script used to indicate when the player has completed a level
 */
public class Win2 : MonoBehaviour {

    // has the win state already been activated?
    private bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        // if the win state hasn't already been activated and we've collided with the player, start the story manager
        if (!activated && (other.tag == "Player" || other.tag == "LeafSwarm"))
        {
            Debug.Log("WIN 2");

            StoryManager_V2.Instance.StartPlaying();

            // set a flag to indicate we've started the story (so the story doesn't get activated more than once)
            activated = true;
        }
    }
}
