using UnityEngine;

public class RockAudio : MonoBehaviour
{
    [Tooltip("The velocity the rock should be going in order to make a sound")]
    public float velocityForSound = 4f;

    private AudioManager audioManager;


    private void Start()
    {
        audioManager = AudioManager.Instance;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Rock" || collision.gameObject.tag == "Floor")
        {
            if (collision.relativeVelocity.magnitude > velocityForSound)
            {
                audioManager.PlayRock();
            }
        }
    }
}
