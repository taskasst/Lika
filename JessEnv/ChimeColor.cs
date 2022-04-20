using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimeColor : MonoBehaviour
{
    public ChimeOrder chime;
    public Material colorWrong;
    public Material colorRight;
    private ParticleSystemRenderer render;
    private bool changeRing;

    private void Start ()
    {
        render = GetComponent<ParticleSystemRenderer>();
        render.material = colorWrong;
    }
	
	
	private void Update ()
    {
        if (chime.GetRung())
        {
            render.material = colorRight;
        }
        else
        {
            render.material = colorWrong;
        }
        changeRing = chime.GetRung();
        

    }
}
