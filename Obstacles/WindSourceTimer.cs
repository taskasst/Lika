using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WindSource))]
public class WindSourceTimer : MonoBehaviour {

    public bool activeOnStart;

    public float onTime;
    public float offTime;

    private WindSource windSource;

    private bool active;
    private bool destroyWindSource;

    public void StartTimer()
    {
        active = true;
        StartCoroutine(Timer());
    }

    public void StopTimer(bool destroyWindSource)
    {
        this.destroyWindSource = destroyWindSource;
    }

    private void Start()
    {
        windSource = GetComponent<WindSource>();

        if (activeOnStart)
        {
            StartTimer();
        }
    }

    private IEnumerator Timer()
    {
        while (active)
        {
            windSource.Activate();

            yield return new WaitForSeconds(onTime);

            windSource.Deactivate();

            yield return new WaitForSeconds(offTime);
        }

        if (destroyWindSource)
        {
            Destroy(gameObject);
        }
    }
}
