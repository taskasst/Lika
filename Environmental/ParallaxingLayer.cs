using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxingLayer : MonoBehaviour {

    // The width of one segment of the background to repeat
    public float segmentWidth = 30;

    // If we want to add a constant speed to the layer
    [Header("Constant Movement")]
    public bool constantlyMoving = false;
    public float speed = 0.5f;

    // How much the layer movement will oppose camera movement (0 doesn't oppose at all. 1 sticks with the camera)
    [Header("Parallaxing Relative to the Camera")]
    [Range(0.0f, 1.0f)]
    public float resistance = 0;

    private float distanceTraveledCamera = 0;
    private float cameraStartPosition = 0;
    private float distanceTraveledLayer = 0;
    public List<Transform> segments;

	void Start ()
    {
        // Store all children of this object as segments in the layer
		foreach (Transform segment in transform)
        {
            segments.Add(segment);
        }

        cameraStartPosition = Camera.main.transform.position.x;
	}
	
	void Update ()
    {
        float distanceThisFrame = resistance * (Camera.main.transform.position.x - cameraStartPosition);
        cameraStartPosition = Camera.main.transform.position.x;

        // Move at constant speed
        if (constantlyMoving)
        {
            distanceThisFrame += Time.deltaTime * speed;
        }

        distanceTraveledLayer += distanceThisFrame;

        // Move all the segments
        foreach (Transform segment in segments)
        {
            segment.Translate(distanceThisFrame, 0, 0);
        }       

        // Wrap segments around when they've moved too far to the left
        if (distanceTraveledLayer < -segmentWidth)
        {
            Transform firstSegment = segments[0];
            firstSegment.Translate(segmentWidth * segments.Count, 0, 0);

            segments.Remove(firstSegment);
            segments.Add(firstSegment);

            distanceTraveledLayer += segmentWidth;
        }
        // Or to the right
        else if (distanceTraveledLayer > segmentWidth)
        {
            Transform lastSegment = segments[segments.Count-1];
            lastSegment.Translate(-segmentWidth * segments.Count, 0, 0);

            segments.Remove(lastSegment);
            segments.Insert(0, lastSegment);

            distanceTraveledLayer -= segmentWidth;
        }     
    }
}
