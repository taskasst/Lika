using UnityEngine;
using UnityEngine.UI;

public class WindmillRotate : MonoBehaviour
{
    // Used to differentiate between what it should effect
    public enum MillType { JustWindmill, Drawbridge, Platform };

    [Tooltip("Used to differentiate between what the windmill should effect")]
    public MillType myType = MillType.JustWindmill;
    
    [Tooltip("The drawbridge for this windmill to effect")]
    public GameObject drawbridge;
    
    [Tooltip("The platform for this windmill to effect")]
    public GameObject platform;
    
    [Tooltip("The speed for the windmill to rotate")]
    public float rotateSpeed = 5f;

    [Tooltip("The amount of power needed to start moving the platform")]
    public float powerNeeded = 4f;

    public Image powerSprite;


    // The audio manager
    private AudioManager audioManager;

    // The rigidbody on this windmill
    private Rigidbody rb;

    // The drawbridge script from the drawbridge gameobject
    private Drawbridge db;

    // The platform script from the platform gameobject
    private WindPlatform wp;

    // Used for the platform
    private float currPower = 0f;

    // If the windmill is powered up for the platform
    private bool poweredUp = false;


    private void Start()
    {
        // Get the rigidbody on this windmill
        rb = GetComponent<Rigidbody>();

        audioManager = AudioManager.Instance;

        if (myType == MillType.Drawbridge)
        {
            // make sure there is a drawbridge game object attached
            if(!drawbridge)
            {
                Debug.LogError("Windmill doesn't have a drawbridge specified!");
            }
            else
            {
                db = drawbridge.GetComponent<Drawbridge>();
            }
        }
        else if (myType == MillType.Platform)
        {
            // make sure there is a platform game object attached
            if (!platform)
            {
                Debug.LogError("Windmill doesn't have a platform specified!");
            }
            else
            {
                wp = platform.GetComponent<WindPlatform>();
            }
        }
        else
        {
            Debug.Log("Windmill has no type.");
        }
    }


    private void Update()
    {
        //Debug.Log(rb.angularVelocity);
        if(myType == MillType.JustWindmill)
        {
            // only the windmill is affected
            // don't do anything
        }
        else if(myType == MillType.Drawbridge)
        {
            // affect the drawbridge gameobject
            if (rb.angularVelocity.z > 0.1f)
            {
                // Left = open
                db.OpenBridge(true);
            }
            else if (rb.angularVelocity.z < -0.1f)
            {
                // Right = close
                db.OpenBridge(false);
            }
        }
        else if(myType == MillType.Platform)
        {
            // affect the platform gameobject
            if (rb.angularVelocity.z < -0.1f)
            {
                // Spinning right, power up the currPower
                if(!poweredUp)
                {
                    currPower += Time.deltaTime;
                }
            }
            float percent = currPower/powerNeeded;
            powerSprite.fillAmount = percent;
        }

        // Has enough power, start moving the platform
        if(currPower >= powerNeeded && !poweredUp)
        {
            poweredUp = true;
            wp.MovePlatform();
        }
    }


    public void SetCurrPower(float power)
    {
        currPower = power;
    }


    public void PlatformDone()
    {
        // Platform has reached the other side
        // It will move back to the left now
        poweredUp = false;
        currPower = 0;
    }


    public void ReverseWindmill(float direction)
    {
        // Should the windmill move when the platform/drawbridge reverses on its own?
        //if(direction > 0)
        //{
            //transform.Rotate(Vector3.forward * (10 * rotateSpeed * Time.deltaTime));
        //}
        //else if(direction < 0)
        //{
            //transform.Rotate(Vector3.forward * (-10 * rotateSpeed * Time.deltaTime));
        //}
        //audioManager.PlayWindmill();
    }


    private void OnMouseOver()
    {
        // Spinning the windmill blades
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && Time.timeScale == 1)
        {
            Debug.Log("Hi");
            // Get movement of the finger since last frame
            Vector3 currPos = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 10);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(currPos);
            Vector3 touchDeltaPos = Input.GetTouch(0).deltaPosition;

            audioManager.PlayWindmill();

            if(touchPos.y >= transform.position.y)
            {
                //Debug.Log("Higher Y");
                rb.AddTorque(Vector3.forward * -touchDeltaPos.x * rotateSpeed);
            }
            if (touchPos.y < transform.position.y)
            {
                //Debug.Log("Lower Y");
                rb.AddTorque(Vector3.forward * touchDeltaPos.x * rotateSpeed);
            }
            if (touchPos.x >= transform.position.x)
            {
                //Debug.Log("Higher X");
                rb.AddTorque(Vector3.forward * touchDeltaPos.y * rotateSpeed);
            }
            if (touchPos.x < transform.position.x)
            {
                //Debug.Log("Lower X");
                rb.AddTorque(Vector3.forward * -touchDeltaPos.y * rotateSpeed);
            }
        }
    }
}
