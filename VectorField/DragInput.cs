using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragInput : MonoBehaviour
{
    // How far does the swipe have to go before we collect a new point?
    public float distanceBetweenPoints = 0.5f;
    // How many fingers can we read in?
    public int numFingers = 5;
    // Changes how many cells should be affected by swipe
    public int sizeAffect = 1;

    public float dragIncrease = 0.1f;

    // does the input have boundaries enabled?
    public bool boundaries;

    // multiplier for boundaries
    public float boundaryMultiplier;

    // The vector field to affect
    private ExNinja.XnVectorField2D vectorField;
    // Last position touched in the swipe
    private Vector3[] lastJetStreamTouch;

    public GameObject streamFXPrefab;
    private GameObject[] streamFXInstances;
    private GameObject streamHolder;

    private AudioManager audioManager;
    
    private void Start()
    {
        if (ExNinja.XnVectorField2D.instance != null)
        {
            // Get the instance of the vector field
            vectorField = ExNinja.XnVectorField2D.instance;
        }

        if (!vectorField)
        {
            Debug.LogError("Drag Input doesn't have a vector field specified!");
        }

        audioManager = AudioManager.Instance;

        // Up to 5 touches will be read
        lastJetStreamTouch = new Vector3[numFingers];

        // Prep for wind VFX
        streamFXInstances = new GameObject[numFingers];
        streamHolder = new GameObject("StreamFX");
    }

    private void Update()
    {
        // Get touch input if the game is playing
        if (Input.touchCount > 0 && Time.timeScale == 1)
        {
            Touch[] touches = Input.touches;
            audioManager.PlayWind();

            // Only allow up to the number of fingers you want
            int maxTouch = Input.touchCount;
            if(Input.touchCount > numFingers)
            {
                maxTouch = numFingers;
            }

            // Handles multi-touch
            for (int i = 0; i < maxTouch; i++)
            {
                Touch touch = touches[i];

                // Get the position of the touch right now
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, 10);
                Vector3 nextPos = Camera.main.ScreenToWorldPoint(touchPos);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // Start of touch, get initial touch
                        lastJetStreamTouch[i] = nextPos;

                        // Create a new wind stream FX instance
                        GameObject streamFXInstance = Instantiate(streamFXPrefab, streamHolder.transform);
                        streamFXInstance.transform.position = nextPos;
                        streamFXInstances[i] = streamFXInstance;
                        break;

                    case TouchPhase.Moved:
                        // Middle of touch, affect the field
                        if (Vector3.Distance(lastJetStreamTouch[i], nextPos) >= distanceBetweenPoints)
                        {
                            AffectField(lastJetStreamTouch[i], nextPos);
                            lastJetStreamTouch[i] = nextPos;

                            // Update FX
                            streamFXInstances[i].transform.position = nextPos;
                        }
                        break;
                }
            }
        }

        // Also handle click input for testing
        else if (Input.GetMouseButton(0) && Time.timeScale == 1)
        {
            audioManager.PlayWind();

            /* This whole section is to handle Index Out of Bounds errors from clicking outside the vector field in editor */
            // Find edges of the screen in world coordinates
            Vector3 lowerLeftWorld = Camera.main.ScreenToWorldPoint(Vector3.zero);
            Vector3 upperRightWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

            // Find distance from edges of vector field to edges of view space in world coordinates
            float xPaddingWorld = ((upperRightWorld.x - lowerLeftWorld.x) - (vectorField.width * vectorField.cellSize)) / 2;
            float yPaddingWorld = ((upperRightWorld.y - lowerLeftWorld.y) - (vectorField.height * vectorField.cellSize)) / 2;

            // Find edges of vector field in world space
            lowerLeftWorld = new Vector3(lowerLeftWorld.x + xPaddingWorld, lowerLeftWorld.y + yPaddingWorld, 0);
            upperRightWorld = new Vector3(upperRightWorld.x - xPaddingWorld, upperRightWorld.y - yPaddingWorld, 0);

            // Find edges of vector field in screen space
            Vector3 lowerLeftScreen = Camera.main.WorldToScreenPoint(lowerLeftWorld);
            Vector3 upperRightScreen = Camera.main.WorldToScreenPoint(upperRightWorld);
            /* */


            // Get the position of the "touch" right now, clamping mouse position to be within the vector field's screen space
            Vector3 touchPos = new Vector3(Mathf.Clamp(Input.mousePosition.x, lowerLeftScreen.x, upperRightScreen.x),
                                           Mathf.Clamp(Input.mousePosition.y, lowerLeftScreen.y, upperRightScreen.y), 10);
            Vector3 nextPos = Camera.main.ScreenToWorldPoint(touchPos);

            if (Input.GetMouseButtonDown(0))
            {
                // Start of "touch", get initial "touch"
                lastJetStreamTouch[0] = nextPos;

                // Create a new wind stream FX instance
                GameObject streamFXInstance = Instantiate(streamFXPrefab, streamHolder.transform);
                streamFXInstance.transform.position = nextPos;
                streamFXInstances[0] = streamFXInstance;
            }
            else
            {
                // Middle of "touch", affect the field
                if (Vector3.Distance(lastJetStreamTouch[0], nextPos) >= distanceBetweenPoints)
                {
                    AffectField(lastJetStreamTouch[0], nextPos);
                    lastJetStreamTouch[0] = nextPos;
                }

                // Update FX
                streamFXInstances[0].transform.position = nextPos;
            }
        }

        else
        {
            audioManager.StopWind();
        }
    }
    
    private void AffectField(Vector2 lastTouch, Vector2 currTouch)
    {
        // Get the direction
        Vector2 touchDir = currTouch - lastTouch;
        // How much to increase the velocity of the cell
        Vector2 velVect = touchDir * dragIncrease;

        // Get the center point between those two points
        Vector2 centerPos = Vector2.Lerp(lastTouch, currTouch, 0.5f);
        Vector2Int tCell = vectorField.GetCell(centerPos);

        // Change cells at and around the touched position
        for (int x = tCell.x - sizeAffect; x <= tCell.x + sizeAffect; x++)
        {
            if (x > 0 && x < vectorField.width)
            {
                for (int y = tCell.y - sizeAffect; y <= tCell.y + sizeAffect; y++)
                {
                    if (y > 0 && y < vectorField.height)
                    {
                        // if boundaries are turned on, the last cells affected should point in towards the leaves and be larger than the other vectors
                        if (boundaries && (y == tCell.y - sizeAffect || y == tCell.y + sizeAffect))
                        {
                            Vector2 boundaryVelVect = ((currTouch - (Vector2)vectorField.GetLoc(x, y)).normalized).normalized * dragIncrease * boundaryMultiplier;
                            vectorField.cells[x][y].ApplyForce(boundaryVelVect);
                        }
                        // otherwise, just use the normal velocity vector
                        else
                        {
                            vectorField.cells[x][y].ApplyForce(velVect);
                        }
                    }
                }
            }
        }
    }
}