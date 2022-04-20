using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindSource : MonoBehaviour {

    // force of the wind
    public Vector2 windForce;

    public bool showParticles = true;

    public bool debugDraw = true;

    // collider for the wind source
    private Collider windCollider;

    // wind particle effect
    private ParticleSystem windParticleSystem;

    // is this wind source currently active?
    private bool active;

    // AudioManager
    private AudioManager audioManager;

    /**
     * Initial setup of wind source
     */
    private void Start()
    {
        windCollider = GetComponent<Collider>();
        windParticleSystem = GetComponentInChildren<ParticleSystem>();

        if (!windCollider)
        {
            Debug.LogError("No collider on a wind source object!");
        }
        if (!windParticleSystem)
        {
            Debug.LogError("No particle system in children of wind source object!");
        }

        if (!showParticles)
        {
            windParticleSystem.gameObject.SetActive(false);
        }
        audioManager = AudioManager.Instance;
        Activate();
    }

    /**
     * Is there anything blocking the wind from affecting a given object?
     */
    public bool IsBlocked(Vector3 objPos)
    {
        if (!active)
        {
            return true;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, objPos - transform.position, (objPos - transform.position).magnitude);

        return hits.Any(hit => hit.collider.gameObject.tag == "WindSourceStop");
    }

    /**
     * Activate the wind source
     */
    public void Activate()
    {
        active = true;

        if (showParticles)
        {
            windParticleSystem.gameObject.SetActive(true);
            audioManager.PlayWind();
        }
    }

    /**
     * Deactivate the wind source
     */
    public void Deactivate()
    {
        active = false;
        windParticleSystem.gameObject.SetActive(false);
    }

    /**
     * Handle drawing of gizmos
     */
    private void OnDrawGizmos()
    {
        if (debugDraw)
        {
            DrawArrow.ForGizmo(transform.position, windForce);
        }
    }
}
