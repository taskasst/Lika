using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Script that applies force to an object based on the values of the vector field at/near the object's position.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VectorFieldMovement : MonoBehaviour
{
    // Used to speed up/slow down movement
    public float forceMultiplier;
    // Used for gravity on leaf swarm
    public float gravMultiplier;
    // Max speed the leaves can go
    public int maxSpeed;
    // Used for the downward sine wave
    public float waveMult;
    public float waveFreq;
    public bool sineWave = false;

    // AudioManager
    private AudioManager audioManager;

    // The vector field affecting this object
    private ExNinja.XnVectorField2D vectorField;

    // list containing any wind source objects that may be affecting the object
    private List<WindSource> windSources;

    // This object's rigidbody
    private Rigidbody rigid;
    private Vector2 oldforce = new Vector2(0, 0);
    private bool onFloor = false;

    //private float direction = 0.0f;
    //private float x;

    private void Start()
    {
        windSources = new List<WindSource>();

        if (ExNinja.XnVectorField2D.instance != null)
        {
            // Get the instance of the vector field
            vectorField = ExNinja.XnVectorField2D.instance;
        }

        if (!vectorField)
        {
            Debug.LogError("Leaf movement doesn't have a vector field specified!");
        }

        rigid = GetComponent<Rigidbody>();
        audioManager = AudioManager.Instance;
    }

    private void FixedUpdate()
    {
        // get the average force of the vector directly underneath the object and the 8 vectors surrounding that position
        Vector2 force = vectorField.GetAverageForceAt(transform.position);

        // select movement code based on tag of the gameobject that this is attached to
        switch(gameObject.tag)
        {
            case "LeafSwarm":
                LeafSwarmMovement(force);
                break;
            case "PickUp":
                PickUpMovement(force);
                break;
            case "LeafIceBall":
                IceBallMovement(force);
                break;
            case "Chime":
                ChimeMovement(force);
                break;
            case "Crystal":
                CrystalMovement(force);
                break;
            case "Snowball":
                SnowballMovement(force);
                break;
            case "InteractiveAnimal":
                AnimalInter(force);
                break;
            case "Spike":
                VineMovement(force);
                break;
            case "Dandelion":
                DandelionPuff(force);
                break;
        }
    }


    // move the leaf swarm
    private void LeafSwarmMovement(Vector2 force)
    {
        if (windSources.Any(windSource => !windSource.IsBlocked(transform.position)))
        {
            Vector3 sumVector = new Vector3(
                windSources.Average(windSource => windSource.windForce.x),
                windSources.Average(windSource => windSource.windForce.y),
                0);

            rigid.AddForce(sumVector * forceMultiplier, ForceMode.Acceleration);

        }
        // if there's no force on the leaves and we aren't already on the floor, apply gravity
        else if (force.magnitude <= 0.05f && !onFloor)
        {
            Vector3 gravity = Vector3.zero;

            if (sineWave)
            {
                // Downward sine wave gravity
                gravity = new Vector3(Mathf.Sin(Time.time * waveFreq) * waveMult, -1.0f, 0);
            }
            else
            {
                // Normal downward gravity
                gravity = Vector3.down;
            }

            // apply gravity to the rigidbody
            rigid.AddForce(gravity * gravMultiplier, ForceMode.Acceleration);

            // clamp velocity to max speed
            rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
        }
        // if there is a force on the leaves, apply force from vector field
        else
        {
            // if changing direction of movement, amplify the amount of force being added
            // dot product: -1 if velocity and force are in opposite directions, 1 if they are in the same direction
            // so if the vectors are perpendicular or farther, give the force added a bit of extra oomph
            float dot = Vector3.Dot(rigid.velocity.normalized, force.normalized);
            float dotMultiplier = dot < 0 ? 2 - dot : 1;

            // apply the force to the object's rigidbody
            rigid.AddForce(force * forceMultiplier * dotMultiplier, ForceMode.Acceleration);

            // clamp velocity to max speed
            rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
        }
    }


    // move a ice ball object
    private void IceBallMovement(Vector2 force)
    {
        // actually apply the force to the object's rigidbody
        if (rigid.position.y < -1.5) // hard coded value to not apply force past a certain point so ball dont not bounce off ceiling collider
        {
            //Debug.Log("Below ceiling");
            rigid.AddForce(force * forceMultiplier, ForceMode.Acceleration);
        }
        else
        {
            //Debug.Log("Above ceiling");
        }
    }


    private void CrystalMovement(Vector2 force)
    {

        //float clockwise = Mathf.Atan(force.y / force.x);
        //float clockwise = Mathf.Sin(direction - (Mathf.Atan2(force.y, force.x)));
        //Debug.Log(clockwise);

        //Debug.Log(Mathf.Atan2(force.y, force.x);

        //direction = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        /*if (Mathf.Atan2(force.y, force.x) > atan2)
        {
            Debug.Log("Clockwise");
        }
        else
            Debug.Log("Counter Clockwise");
        atan2 = Mathf.Atan2(force.y, force.x);*/
        //Debug.Log(direction);
        /*if (direction > 1)
        {
            direction = 1;
        }
        else { direction = -1; }
        rigid.AddTorque(0,0, direction*force.magnitude);*/
        rigid.AddTorque(new Vector3(0, 0, force.x) * forceMultiplier);

    }


    // move/interact with wind chimes
    private void ChimeMovement(Vector2 force)
    {
        if (force * forceMultiplier != new Vector2(0, 0))
        {
            if (oldforce.magnitude < force.magnitude)
            {
                rigid.gameObject.GetComponent<ChimeOrder>().SetRung();
            }
            oldforce = force;
        }
    }

    private void AnimalInter(Vector2 force)
    {
        if ((force * forceMultiplier).magnitude > 2)
        {
            if (rigid.gameObject.GetComponent<LoopAnimation>() && !rigid.gameObject.GetComponent<LoopAnimation>().animationChange)
            {
                Destroy(rigid.gameObject.GetComponent<LoopAnimation>());
            }
            if (rigid.gameObject.GetComponent<InteractionLayout>())
            {
                rigid.gameObject.GetComponent<InteractionLayout>().SetPlayInteract(true);
            }          
        }
       
    }

    private void DandelionPuff(Vector2 force)
    {
        //rigid.GetComponent<ParticleSystem>().Stop();
        if ((force * forceMultiplier).magnitude > 1)
        {
            if (rigid.gameObject.GetComponent<ParticleSystem>())
            {
                Debug.Log("HERE");
                rigid.gameObject.GetComponent<ParticleSystem>().Play();
                audioManager.PlayPlantRustle();
               
            }
               
        }
        else
        {
            if (rigid.gameObject.GetComponent<ParticleSystem>())
            {
                //Debug.Log("HERE");
                rigid.gameObject.GetComponent<ParticleSystem>().Stop();
            }
        }
    }


    private void PickUpMovement(Vector2 force)
    {
        if (windSources.Any(windSource => !windSource.IsBlocked(transform.position)))
        {
            Vector3 sumVector = new Vector3(
                windSources.Average(windSource => windSource.windForce.x),
                windSources.Average(windSource => windSource.windForce.y),
                0);

            rigid.AddForce(sumVector * forceMultiplier, ForceMode.Acceleration);

        }
        else
        {
            float dot = Vector3.Dot(rigid.velocity.normalized, force.normalized);
            float dotMultiplier = dot < 0 ? 2 - dot : 1;

            // apply the force to the object's rigidbody
            rigid.AddForce(force * forceMultiplier * dotMultiplier, ForceMode.Acceleration);

            // clamp velocity to max speed
            rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
        }
    }


    private void SnowballMovement(Vector2 force)
    {
        Vector2 forceNoY = new Vector2(force.x, 0f);
        float dot = Vector3.Dot(rigid.velocity.normalized, forceNoY.normalized);
        float dotMultiplier = dot < 0 ? 2 - dot : 1;

        // apply the force to the object's rigidbody
        rigid.AddForce(forceNoY * forceMultiplier * dotMultiplier, ForceMode.Acceleration);

        // clamp velocity to max speed
        rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
    }


    private void VineMovement(Vector2 force)
    {
        // This is the weird scrunching up version
        //var hinge = GetComponent<HingeJoint>();

        // Make the hinge motor rotate with 90 degrees per second and a strong force.
        //var motor = hinge.motor;
        //motor.force = force.magnitude * 500;
        //motor.targetVelocity = 180;
        //motor.freeSpin = false;
        //hinge.motor = motor;
        //hinge.useMotor = true;

        // This is normal swinging
        float dot = Vector3.Dot(rigid.velocity.normalized, force.normalized);
        float dotMultiplier = dot < 0 ? 2 - dot : 1;

        // apply the force to the object's rigidbody
        rigid.AddForce(force * forceMultiplier * dotMultiplier, ForceMode.Acceleration);

        // clamp velocity to max speed
        rigid.velocity = Vector3.ClampMagnitude(rigid.velocity, maxSpeed);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            // stop gravity if on the floor
            onFloor = true;
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            // gravity can start again
            onFloor = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<WindSource>() != null)
        {
            windSources.Add(other.gameObject.GetComponent<WindSource>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<WindSource>() != null)
        {
            windSources.Remove(other.gameObject.GetComponent<WindSource>());
        }
    }
}