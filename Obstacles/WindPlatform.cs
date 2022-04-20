using UnityEngine;

public class WindPlatform : MonoBehaviour
{
    [Tooltip("The windmill controlling this platform")]
    public WindmillRotate windmill;

    [Tooltip("The first position to move towards")]
    public GameObject posOne;

    [Tooltip("The second position to move towards")]
    public GameObject posTwo;

    [Tooltip("How fast the platform should move")]
    public float speed = 0.5f;

    [Tooltip("How long the platform should wait before moving back")]
    public float timeWait = 2f;


    private enum Stage { Stationary, Moving, Waiting, Returning };
    private Stage stage = Stage.Stationary;

    // The audio manager
    private AudioManager audioManager;

    // How long until the platform starts moving back
    private float waitTime = 0f;

    // The point determined to be the right or lower point
    private Vector3 rightDownPos;

    // The point determined to be the left or higher point
    private Vector3 leftUpPos;


    private void Start()
    {
        audioManager = AudioManager.Instance;

        // check which point is left and which is right
        if (posOne.transform.position.x > posTwo.transform.position.x)
        {
            // posOne is to the right of posTwo
            rightDownPos = posOne.transform.position;
            leftUpPos = posTwo.transform.position;
        }
        else if (posOne.transform.position.x < posTwo.transform.position.x)
        {
            // posOne is to the left of posTwo
            leftUpPos = posOne.transform.position;
            rightDownPos = posTwo.transform.position;
        }
        else
        {
            // same x position for the points
            // check which is up and which is down
            if (posOne.transform.position.y > posTwo.transform.position.y)
            {
                // posOne is above posTwo
                leftUpPos = posOne.transform.position;
                rightDownPos = posTwo.transform.position;
            }
            else if (posOne.transform.position.y < posTwo.transform.position.y)
            {
                // posOne is to the below posTwo
                rightDownPos = posOne.transform.position;
                leftUpPos = posTwo.transform.position;
            }
        }
    }


    private void Update()
    {
        if (stage == Stage.Moving)
        {
            // currently moving right
            Movement(rightDownPos);
        }
        else if (stage == Stage.Waiting)
        {
            // platform waiting at the right side
            // countdown until the platform starts moving back
            waitTime -= Time.deltaTime;
            windmill.SetCurrPower(waitTime * 2f);
            if(waitTime <= 0)
            {
                // platform should start moving to the left
                stage = Stage.Returning;
                windmill.PlatformDone();
            }
        }
        else if (stage == Stage.Returning)
        {
            // platform moving back left
            Movement(leftUpPos);
        }
    }


    public void MovePlatform()
    {
        // Start the stages of platform movement
        waitTime = timeWait;

        if(stage == Stage.Stationary || stage == Stage.Returning)
        {
            // Platform can start moving to the right if it's moving back to the left
            // or not moving
            stage = Stage.Moving;
        }
    }


    private void Movement(Vector3 pos)
    {
        if (Vector3.Distance(transform.position, pos) > 0.01)
        {
            // Move the platform towards pos
            transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
            audioManager.PlayPlatform();
        }
        else
        {
            // Once it reaches pos, switch the stage to the next portion
            if (stage == Stage.Moving)
            {
                stage = Stage.Waiting;
            }
            else if (stage == Stage.Returning)
            {
                stage = Stage.Stationary;
            }
        }
    }
}
