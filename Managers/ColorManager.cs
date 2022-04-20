using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // There is only one color manager, make an instance of it
    public static ColorManager cm = null;

	// So we can access the current number of leaves
	public LeafManager leafManager;

	// Time in seconds it takes to fade from one color to another
	public float fadeDuration = 1.0f;
	private float timer;
	private bool fading = false;

    private float currentSaturation = 0.0f;
    private float fromSaturation;
    private float targetSaturation;
    private ScreenSpaceShaderHandler imageFXHandler;

    private void Awake()
    {
        // Set up the color manager instance
        if (cm == null)
        {
            cm = this;
        }
        else if (cm != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
	{
        // Set initial material saturation
        imageFXHandler = Camera.main.GetComponent<ScreenSpaceShaderHandler>();
        //imageFXHandler.ChangeColor(currentSaturation);
	}


	void Update()
	{
		// Is color currently changing?
		/*if (fading) 
		{
			if (timer >= 0) 
			{
				timer -= Time.deltaTime;

				// Update saturation
				//currentSaturation = Mathf.Lerp(targetSaturation, fromSaturation, timer / fadeDuration);
                //imageFXHandler.ChangeColor(currentSaturation);
			} 

			// Finished color transition
			else 
			{
				fading = false;
			}
		}*/
	}


	/* Updates saturation of all materials designated to change */
	public void UpdateColors(bool fade = true)
	{
        // Calculate percent saturation based on number of pickups collected
        if (fade)
        {
            targetSaturation = (float)(leafManager.GetPickups() - 1) / (leafManager.maxPickups - 1);
            imageFXHandler.ChangeColor(currentSaturation, targetSaturation, leafManager.transform.position);
            currentSaturation = targetSaturation;
        }

        else
        {

        }
    }
}
