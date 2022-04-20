using UnityEngine;

public class FogFade : MonoBehaviour
{
    [Tooltip("The max opacity the fog should be at rest")]
    [Range(0f,1f)]
    public float maxOpacity = 1f;

    // The vector field
    private ExNinja.XnVectorField2D vectorField;
    // Used for transparency on this fog square
    private Color color;
    private MeshRenderer rend;

	private void Start()
    {
        // Setting the vector field to the instance
        vectorField = ExNinja.XnVectorField2D.instance;

        // Get the color on this object
        rend = GetComponent<MeshRenderer>();
        color = rend.material.color;
	}
	
	private void FixedUpdate()
    {
        // Getting the average force around this fog square
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector2 force = vectorField.GetAverageForceAt(pos);
        float opacityVal = 1 - force.magnitude * 1.5f;
        SetFogOpacity(opacityVal);
    }

    private void SetFogOpacity(float val)
    {
        // Make the opacity depend on the strength of the surrounding wind
        color.a = Mathf.Clamp(val, 0, maxOpacity);
        rend.material.color = color;
    }
}
