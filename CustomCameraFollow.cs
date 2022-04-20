using UnityEngine;

public class CustomCameraFollow : MonoBehaviour {

    [Tooltip("Target object to follow")]
    public GameObject target;

    [Tooltip("If true, camera will be biased towards the direction of the last significant movement (left or right)")]
    public bool biasActive;

    [Tooltip("If true, bias will not snap back until another signifcant movement is noticed")]
    public bool biasSnapBack;

    [Tooltip("If bias is active, how large (as a fraction of screen size) the bias will be")]
    public float maxBiasSize;

    [Tooltip("If bias is active, how fast the target has to be moving in a new direction to switch biases")]
    public float minBiasVelocity;

    [Tooltip("If bias is active, velocity at which no more bias will be applied")]
    public float maxBiasVelocity;

    [Tooltip("Effect of target's velocity on the camera movement is multiplied by this value")]
    public float velocityOffsetMultiplier;

    [Tooltip("How much should the velocity offset effect be dampened by when the target's speed is decreasing")]
    public float decreasingSpeedOffsetNerf;

    [Tooltip("Target time for damping action taken by Vector3.SmoothDamp")]
    public float dampingTime;

    [Tooltip("multiplier applied to the maximum distance that the leaves can be from the center of the screen")]
    public float maxOffsetMultiplier;

    // initial camera position
    private Vector3 initialCameraPosition;

    // camera velocity (passed into SmoothDamp as ref)
    private Vector3 cameraVelocity;

    // width of viewport in world coordinates
    private Vector3 viewportWidthWorld;

    // maximum distance from the center of the screen that the leaves should be
    // generated at runtime
    private float maxOffset;

    // velocity of the target at the previous update
    private Vector3 lastTargetVelocity;

    // current x offset between the camera and the target
    private float currentXOffset;

    // current x bias applied to camera (used in calculated of x offset)
    private float currentXBias;

	// Use this for initialization
	void Start ()
    {
        // check for null refs
        if (!target)
        {
            Debug.LogError("No target for camera!");
        }

        // intialize values
        initialCameraPosition = transform.position;
        cameraVelocity = Vector3.zero;
        lastTargetVelocity = Vector3.zero;
        currentXBias = 0;

        // calculate how big the viewport is in world coordinates and use that to determine the max offset
        viewportWidthWorld = Camera.main.ViewportToWorldPoint(Vector3.right) - Camera.main.ViewportToWorldPoint(Vector3.zero);
        maxOffset = ( viewportWidthWorld * maxOffsetMultiplier).x;

	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        // only update the camera if the target is specified
        if (target)
        {
            // get current target position
            Vector3 newTargetPos = target.transform.position;

            // for purposes of calculation, pretend camera and target are on same y and z planes
            newTargetPos.y = initialCameraPosition.y;
            newTargetPos.z = initialCameraPosition.z;

            // get current velocity of the target
            Vector3 currentTargetVelocity = target.GetComponent<Rigidbody>().velocity;

            // if bias is active, we should check to see if we should apply a new bias
            if (biasActive && Mathf.Abs(currentTargetVelocity.x) > minBiasVelocity && (biasSnapBack || Mathf.Abs(currentTargetVelocity.x) >= Mathf.Abs(lastTargetVelocity.x)))
            {
                currentXBias = maxBiasSize * viewportWidthWorld.x * (Mathf.Clamp(currentTargetVelocity.x, -maxBiasVelocity, maxBiasVelocity) / maxBiasVelocity);

                // if moving fast to the left, set a left bias for the camera offset
                //if (currentTargetVelocity.x < -minBiasVelocity)
                //{
                //    currentXBias = -biasSize * viewportWidthWorld.x;
                //}
                // if moving fast to the right, set a right bias for the camera offset
                //else if (currentTargetVelocity.x > minBiasVelocity)
                //{
                //    currentXBias = biasSize * viewportWidthWorld.x;
                //}
            }

            // if we are speeding up or staying equal x speed, do the offset as normal
            if (Mathf.Abs(currentTargetVelocity.x) >= Mathf.Abs(lastTargetVelocity.x))
            {
                currentXOffset = currentXBias + (currentTargetVelocity.x * velocityOffsetMultiplier);
            }
            // if we are losing x speed, make velocity offset smaller (so that it moves back towards the object in a more gradual way
            else
            {
                currentXOffset = currentXBias + (currentTargetVelocity.x * velocityOffsetMultiplier / decreasingSpeedOffsetNerf);
            }

            // keeping track of each target velocity for later use
            lastTargetVelocity = currentTargetVelocity;

            // apply the offset
            newTargetPos.x += currentXOffset;

            // clamp the offset to make sure we're on screen
            newTargetPos.x = Mathf.Clamp(newTargetPos.x, target.transform.position.x - maxOffset, target.transform.position.x + maxOffset);

            // actually apply a SmoothDamp function using our new target position, etc.
            transform.position = Vector3.SmoothDamp(transform.position, newTargetPos, ref cameraVelocity, dampingTime);
        }
	}
}
