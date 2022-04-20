using UnityEngine;

public class Drawbridge : MonoBehaviour
{
    [Tooltip("The windmill opening/closing this drawbridge")]
    public GameObject windmill;

    [Tooltip("The speed the windmill rotates this drawbridge")]
    public float rotateSpeed = 15f;

    [Tooltip("The speed this drawbridge closes at")]
    public float closeSpeed = 20f;

    [Tooltip("How long the drawbridge should wait before closing")]
    public float timeWait = 1f;


    // The audio manager
    private AudioManager audioManager;

    // How long is left until the drawbridge starts closing
    private float timing = 0f;

    // Is the drawbridge closed right now
    private bool closed = true;

    
    private void Start()
    {
        audioManager = AudioManager.Instance;
    }


    private void Update()
    {
        if(!closed)
        {
            if(timing <= 0)
            {
                // Stop rotating at 0 degrees
                if (transform.rotation.z > 0.005)
                {
                    CloseBridge();
                }
                else
                {
                    closed = true;
                }
            }
            else
            {
                timing -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Rotate the drawbridge open or closed
    /// </summary>
    /// <param name="open">true if opening false if closing</param>
    public void OpenBridge(bool open)
    {
        if (open)
        {
            // Open the drawbridge
            Quaternion rot = new Quaternion(0.0f, 0.0f, 0.7f, 0.7f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, Time.deltaTime * rotateSpeed);
        }
        else
        {
            // Close the drawbridge
            Quaternion rot = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, Time.deltaTime * rotateSpeed);
        }

        audioManager.PlayDrawbridge();

        // Used to start closing the drawbridge after
        timing = timeWait;
        closed = false;
    }

    /// <summary>
    /// Closes the drawbridge automatically
    /// </summary>
    public void CloseBridge()
    {
        // Close the drawbridge
        Quaternion rot = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, Time.deltaTime * closeSpeed);
        //windmill.GetComponent<WindmillRotate>().ReverseWindmill(-1);
        audioManager.PlayDrawbridge();
    }
}
