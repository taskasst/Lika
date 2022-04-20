using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that expands the vectorField attached to this gameObject to cover the entirety of the area filled by the camera.
/// </summary>
[RequireComponent(typeof(ExNinja.XnVectorField2D))]
public class VectorFieldExpander : MonoBehaviour {

    public int widthPaddingPerSide = 7;

    private ExNinja.XnVectorField2D vectorField;

	// Use this for initialization
	void Start () {

        // get the vector field component
        vectorField = GetComponent<ExNinja.XnVectorField2D>();

        // set the vector field object to be the same height as the camera at first
        transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);

        // get corners of the viewport in world coordinates
        Vector3 viewportStartWorld = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 viewportEndWorld = Camera.main.ViewportToWorldPoint(Vector3.one);

        // get width and height of viewport from coords
        float viewportWidth = viewportEndWorld.x - viewportStartWorld.x;
        float viewportHeight = viewportEndWorld.y - viewportStartWorld.y;

        // make the vector field at or just below width of camera FOV and at or just above height of camera FOV
        vectorField.width = Mathf.FloorToInt(viewportWidth / vectorField.cellSize) + widthPaddingPerSide * 2;
        vectorField.height = Mathf.CeilToInt(viewportHeight / vectorField.cellSize);

        // now that we've sized the vector field, move it so that the bottom cell lines up with the bottom of the camera viewport
        transform.position = new Vector3 (transform.position.x - (vectorField.GetLoc(0,0).x - viewportStartWorld.x), transform.position.y - (vectorField.GetLoc(0, 0).y - viewportStartWorld.y) , transform.position.z);
    }
}
