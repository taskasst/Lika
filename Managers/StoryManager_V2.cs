using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager

public class StoryManager_V2 : MonoBehaviour {

    public static StoryManager_V2 Instance { get; private set; }

    // level to load when done
    public string levelToLoad;

    // Fades in to start the story cutscene
    public Fade background;

    // sound that should play when skip button pressed
    public AudioClip skipAudioClip;

    // Time that a line of text stays visible
    public float holdTime = 2;

    // Amount of time that audio takes to fade out before the level transitions
    public float musicFadeTime;

    // is the story at this point branching?
    public bool branching = true;

    // is this story at the end of the game?
    public bool isEndOfGame;

    public int lastLineHoldTime;

    // leaf manager (used to get score if branching)
    public LeafManager leafManager;

    // Sets of story text based on score
    public Text[] badText;
	public Text[] middleText;
	public Text[] bestText;

    // scores needed to determine which story text the player gets
    public int minimumMiddleScore = 6;
    public int minimumBestScore = 11;

    // story text used if the text is not based on score (e.g. start or end of level)
    public Text[] nonBranchingText;

    // actual text lines that are being used internally
    // set in setup to one of the text arrays specified publicly above
    private Text[] textLines;
	private int currentLine = 0;

	// Indicates to start story animation
	private bool playing = false;

    // internal timer
	private float timer = 0;

    private bool skipped = false;

    // audio source for sound that should play when skip button pressed
    private AudioSource skipAudioSource;

    // enum for current state of the story manager
    private enum StoryManagerState
    {
        FadingIn,
        FadingOut,
        Holding
    }

    // asynchronous operation for loading the next level
    private AsyncOperation loadLevelOperation;

    // story manager's current state
    private StoryManagerState state;

    void Start()
	{
        skipped = false;

        // handle singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one game manager v2!");
            Destroy(this);
        }

        // get components, check for null refs
        if (branching && leafManager == null)
        {
            Debug.LogError("Story set to branching, but leaf manager not specified!");
        }

        skipAudioSource = GetComponent<AudioSource>();
        if (skipAudioSource == null)
        {
            Debug.LogError("No audio source on story manager V2 object!");
        }

        // Gets us ready for the first line to come in
        currentLine = -1;
        state = StoryManagerState.FadingOut;


	}

    /**
     * Occurs every update
     * 
     * Currently, nothing happens in this function unless the story is currently playing
     */
	void Update()
	{
        // Only update if the story has started at the end of the level
        if (playing)
        {
			// Timer expired. Move on to next action
			if (timer <= 0)
            {
                // what we want to do depends on current story manager state
                switch(state)
                {
                    // New text finished fading in. Hold for specified amount of time
                    case StoryManagerState.FadingIn:
                        state = StoryManagerState.Holding;
                        timer = (isEndOfGame && currentLine + 1 >= textLines.Length) ? lastLineHoldTime : holdTime;
                        break;
                    // Old text finished fading out. Start fading in next text or end the story stuff
                    case StoryManagerState.FadingOut:
                        // if there is no more text, fade out fully
                        if (++currentLine >= textLines.Length && !loadLevelOperation.allowSceneActivation)
                        {
                            // Fades out audio and sets cutscene finished bool
                            StartCoroutine(FadeOut(musicFadeTime));
                        }
                        // if there is another text line, fade that line in
                        else
                        {
                            state = StoryManagerState.FadingIn;
                            textLines[currentLine].transform.GetComponent<Fade>().FadeIn("a");
                            
                            // keep track of the text's fade duration so we know when it's done fading in
                            timer = textLines[currentLine].transform.GetComponent<Fade>().fadeDuration;
                        }
                        break;
                    // Finished holding current text. Start to fade out
                    case StoryManagerState.Holding:
                        state = StoryManagerState.FadingOut;
                        textLines[currentLine].transform.GetComponent<Fade>().FadeOut();

                        // keep track of the text's fade duration so we know when it's done fading in
                        timer = textLines[currentLine].transform.GetComponent<Fade>().fadeDuration;
                        break;
                }
		    }
            // if timer has not expired, just do the timer operation
            else
            {
                timer -= Time.deltaTime;
            }
		}
	}

    /**
     * Start playing the story
     */
	public void StartPlaying()
	{
        // if the story is branching, we need to determine which set of story lines to use
        if (branching)
        {
            // if the player picked up few leaves, give them the bad text
            if (leafManager.GetPickups() < minimumMiddleScore)
            {
                Debug.Log("Bad ending : " + leafManager.GetPickups());
                textLines = badText;
            }
            // if the player got a middling number of leaves, give them the normal text
            else if (leafManager.GetPickups() < minimumBestScore)
            {
                Debug.Log("Middle ending");
                textLines = middleText;
            }
            // if the player got a lot of leaves, give them the best text
            else if (leafManager.GetPickups() >= minimumBestScore)
            {
                Debug.Log("Best ending");
                textLines = bestText;
            }
            // if none of these conditions happened, either math is wrong or we fucked up somehow
            else
            {
                Debug.LogError("Max score error in story manager");
            }
        }
        // if the story is not branching, just use the non branching text
        else
        {
            Debug.Log("Non branching text");
            textLines = nonBranchingText;
        }
		
        // start the fade in for the story text
		StartCoroutine(InitialFadeIn());
	}

    public bool IsPlaying()
    {
        return playing;
    }

    /**
     * Initial fade in for the story
     * 
     * Fade the background in all the way before starting the text
     */
    IEnumerator InitialFadeIn()
	{
        // start loading the next level in the background
        loadLevelOperation = SceneManager.LoadSceneAsync(levelToLoad);

        // don't allow the level to actually load yet though
        loadLevelOperation.allowSceneActivation = false;

        // start fading in the background and wait for it to fade in fully
		background.FadeIn("a");
		yield return new WaitForSeconds(background.fadeDuration);

        if (isEndOfGame)
        {
            AudioManager.Instance.FadeOutAllMusic(musicFadeTime);

            yield return new WaitForSeconds(musicFadeTime);

            AudioManager.Instance.PlayFinalMusic();
        }

        // start playing
        playing = true;
	}

    /**
     * Coroutine for final fade out
     * 
     * @param fadeTime float The amount of time the music should take to fade out
     */
	IEnumerator FadeOut(float fadeTime)
	{
        // indicate stop playing
        playing = false;

        // fade out music in levels that have an audio manager
        if(AudioManager.Instance)
        {
            AudioManager.Instance.FadeOutAllMusic(fadeTime);
        }
        
        yield return new WaitForSeconds(fadeTime);

        // allow the level load to actually take place
        loadLevelOperation.allowSceneActivation = true;
    }

    /**
     * Skip the story
     */
     public void Skip()
    {
        if (!skipped && playing)
        {
            if (!(state == StoryManagerState.FadingOut && currentLine +1 == textLines.Length))
            {
                // fade out whatever the current text is
                timer = textLines[currentLine].GetComponent<Fade>().fadeDuration;
                textLines[currentLine].GetComponent<Fade>().FadeOut();

                // when the fade is done, we just want to skip right to the end
                // to do this, manipulate currentLine and state so it appears we got to the end of the story
                currentLine = textLines.Length - 1;
                state = StoryManagerState.FadingOut;

                // play skip sound
                // skipAudioSource.PlayOneShot(skipAudioClip, .7f);
            }
        }

        skipped = true;
    }
}
