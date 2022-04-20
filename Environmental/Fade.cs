using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour {

	private Color fromAlpha;
	private Color targetAlpha;
	private Color lerpedAlpha;

	// Time in seconds it takes to fade in or out
	public float fadeDuration = 1.0f;
	private float timer = 0;
	private bool fading = false;

	public bool fadeInOnStart = false;
	public bool fadeOutOnStart = false;

    public GameObject skipButton;

	void Start()
	{
        if (skipButton != null)
        {
            skipButton.SetActive(false);
        }
		if (fadeInOnStart) {
			FadeIn ("a");
		}
		else if (fadeOutOnStart) {
			if (this.GetComponent<Image>() != null)
				this.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
			else
				this.GetComponent<Text> ().color = new Color (1, 1, 1, 1);
			
			FadeOut ();
		}
	}

	void Update()
	{
		// Is object currently fading?
		if (fading) 
		{
			if (timer >= 0) 
			{
				timer -= Time.deltaTime;

				lerpedAlpha = Color.Lerp (targetAlpha, fromAlpha, timer / fadeDuration);
				if (this.GetComponent<Image> () != null)
					this.GetComponent<Image> ().color = lerpedAlpha;
				else
					this.GetComponent<Text> ().color = lerpedAlpha;
			}
			// Finished color transition
			else 
			{
				fading = false;
			}
		}
	}


	/* Starts fading the object in */
	public void FadeIn(string level)
	{
        if (skipButton != null && level != "Credits")
        {
            skipButton.SetActive(true);
        }
        FadeMechanics ();
		targetAlpha = new Color (fromAlpha.r, fromAlpha.g, fromAlpha.b, 1);
	}

	/* Starts fading the object out */
	public void FadeOut()
	{
		FadeMechanics();
		targetAlpha = new Color (fromAlpha.r, fromAlpha.g, fromAlpha.b, 0);
	}

	/* Shared fading code */
	private void FadeMechanics()
	{
		fading = true;
		timer = fadeDuration;

		if (this.GetComponent<Image> () != null)
			fromAlpha = this.GetComponent<Image> ().color;
		else
			fromAlpha = this.GetComponent<Text> ().color;
	}
}
