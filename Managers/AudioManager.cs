using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // Instance of the audio manager so many scripts can access it
    public static AudioManager Instance = null;
    
    [Tooltip("The parent game object of all music track audio sources")]
    public GameObject musicGO;

    [Tooltip("The length of time to fade in the tracks")]
    public float fadeDuration = 2f;

    [Tooltip("The length of time to fade in the wind")]
    public float windDuration = 4f;

    // List of music
    private AudioSource[] musicLayers = new AudioSource[0];

    [Tooltip("The audio that plays when you gain a leaf")]
    public AudioSource gainLeaf;

    [Tooltip("The audio that plays when you gain a leaf")]
    public AudioSource gainLeafMusic;

    [Tooltip("The audio that plays when you lose a leaf")]
    public AudioSource loseLeaf;

    [Tooltip("The audio that plays when you lose a leaf")]
    public AudioSource loseLeafMusic;

    [Tooltip("The audio that plays when you rotate the windmill")]
    public AudioSource windmill;

    [Tooltip("The audio that plays when the drawbridge is rotating")]
    public AudioSource drawbridge;

    [Tooltip("The audio that plays when the platform is being moved")]
    public AudioSource platform;

    [Tooltip("The audio that plays when the rock hits the ground")]
    public AudioSource rockThud;

    [Tooltip("The audio that plays when a chime is blown on")]
    public AudioSource chimeOne;

    [Tooltip("The audio that plays when a chime is blown on")]
    public AudioSource chimeTwo;

    [Tooltip("The audio that plays when a chime is blown on")]
    public AudioSource chimeThree;

    [Tooltip("The audio that plays when a chime is blown on")]
    public AudioSource chimeFour;

    [Tooltip("The audio that plays when the wrong chime is blown on")]
    public AudioSource chimeWrong;

    [Tooltip("The audio that plays for a dry droplet")]
    public AudioSource dropletDryOne;

    [Tooltip("The audio that plays for a dry droplet")]
    public AudioSource dropletDryTwo;

    [Tooltip("The audio that plays for a dry droplet")]
    public AudioSource dropletDryThree;

    [Tooltip("The audio that plays for a wet droplet")]
    public AudioSource dropletWetOne;

    [Tooltip("The audio that plays for a wet droplet")]
    public AudioSource dropletWetTwo;

    [Tooltip("The audio that plays for a wet droplet")]
    public AudioSource dropletWetThree;

    [Tooltip("The audio that plays for a snowflake hitting the leaf swarm")]
    public AudioSource snowflakeHit;

    [Tooltip("The audio that plays for a spike hitting the leaf swarm")]
    public AudioSource spikeHit;

    [Tooltip("The audio that plays when the fish hits the water")]
    public AudioSource waterSplash;

    [Tooltip("The audio that plays when the wall explodes")]
    public AudioSource wallExplode;

    [Tooltip("The chime audio that plays when wind is blowing")]
    public AudioSource windChimeAmbiance;

    [Tooltip("The wind audio that plays when wind is blowing")]
    public AudioSource windAmbiance;

    [Tooltip("The audio that plays when you win the game")]
    public AudioSource finalMusic;

    [Tooltip("The audio that plays when you help an animal")]
    public AudioSource helpAnimal;

    [Tooltip("The audio that plays when you help the bird")]
    public AudioSource birdTweet;

    [Tooltip("The audio that plays when you hit vines")]
    public AudioSource vineHit;

    [Tooltip("The audio that plays when you save the beehive")]
    public AudioSource bees;

    [Tooltip("The audio that plays when you save the squirrel")]
    public AudioSource snowBall;

    [Tooltip("The audio that plays when you grow a pumpkin")]
    public AudioSource plantGrow;

    [Tooltip("The audio that plays when you blow plants")]
    public AudioSource plantRustle;

    [Tooltip("The audio that plays when you make a snowman")]
    public AudioSource pop;

    [Tooltip("The audio that plays when you roll a snowball")]
    public AudioSource snowballRoll;


    private void Awake()
    {
        // Set up the audio manager instance
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Make sure that all audio sources are present
        if (musicGO)
        {
            // Get all of the layered tracks
            musicLayers = musicGO.GetComponentsInChildren<AudioSource>();
        }
        else
        {
            Debug.LogError("Music is missing!");
        }
        if(!gainLeaf)
        {
            Debug.LogError("Gain leaf sound is missing!");
        }
        if (!loseLeaf)
        {
            Debug.LogError("Lose leaf sound is missing!");
        }
        if(!windmill)
        {
            Debug.LogError("Windmill sound is missing!");
        }
        if(!drawbridge)
        {
            Debug.LogError("Drawbridge sound is missing!");
        }
        if (!platform)
        {
            Debug.LogError("Platform sound is missing!");
        }
        if (!rockThud)
        {
            Debug.LogError("Rock thud sound is missing!");
        }
        if (!chimeOne)
        {
            Debug.LogError("Chime one sound is missing!");
        }
        if (!chimeTwo)
        {
            Debug.LogError("Chime two sound is missing!");
        }
        if (!chimeThree)
        {
            Debug.LogError("Chime three sound is missing!");
        }
        if (!chimeFour)
        {
            Debug.LogError("Chime four sound is missing!");
        }
        if(!chimeWrong)
        {
            Debug.LogError("Chime wrong sound is missing!");
        }
        if(!dropletDryOne)
        {
            Debug.LogError("Droplet dry one sound is missing!");
        }
        if (!dropletDryTwo)
        {
            Debug.LogError("Droplet dry two sound is missing!");
        }
        if (!dropletDryThree)
        {
            Debug.LogError("Droplet dry three sound is missing!");
        }
        if (!dropletWetOne)
        {
            Debug.LogError("Droplet wet one sound is missing!");
        }
        if (!dropletWetTwo)
        {
            Debug.LogError("Droplet wet two sound is missing!");
        }
        if (!dropletWetThree)
        {
            Debug.LogError("Droplet wet three sound is missing!");
        }
        if(!windChimeAmbiance || !windAmbiance)
        {
            Debug.LogError("Wind sound is missing!");
        }

        foreach (AudioSource aSource in musicLayers)
        {
            // Mute all of the tracks
            aSource.volume = 0f;
        }

        // Mute the wind at the start
        windChimeAmbiance.volume = 0f;
        windAmbiance.volume = 0f;

        // Un-mute the first track
        if (musicLayers.Length >= 1)
        {           
            StartCoroutine(FadeIn(musicLayers[0], fadeDuration));
        }
    }

    public void PlayAllLayers()
    {
        foreach (AudioSource a in musicLayers)
        {
            a.Play();
        }
    }
    public void FadeOutAllMusic(float fadeTime)
    {
        foreach (AudioSource a in musicLayers)
        {
            StartCoroutine(FadeOut(a, fadeTime));
        }

        if (finalMusic.isPlaying)
        {
            StartCoroutine(FadeOut(finalMusic, fadeTime));
        }
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float maxVolume = 1f;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= maxVolume * Time.deltaTime / fadeTime;

            yield return null;
        }
        
        audioSource.volume = 0f;
    }

    public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float maxVolume = 1f;

        while (audioSource.volume < 1)
        {
            audioSource.volume += maxVolume * Time.deltaTime / fadeTime;

            yield return null;
        }
        
        audioSource.volume = 1f;
    }

    public void AddTrack(int numLeaves)
    {
        // Turn on the music layer
        if (musicLayers.Length > numLeaves - 1)
        {
            if (!musicLayers[1].isPlaying)
            {
                PlayAllLayers();
            }
            StartCoroutine(FadeIn(musicLayers[numLeaves - 1], fadeDuration));
        }
    }

    public void RemoveTrack(int numLeaves)
    {
        // Turn off the music layer
        if (musicLayers.Length > numLeaves - 1)
        {
            StartCoroutine(FadeOut(musicLayers[numLeaves - 1], fadeDuration));
        }
    }

    public void PlayGainLeaf()
    {
        gainLeaf.Play();
    }

    public void PlayLoseLeaf()
    {
        loseLeaf.Play();
    }

    public void PlayWindmill()
    {
        if (!windmill.isPlaying)
        {
            windmill.Play();
        }
    }

    public void PlayDrawbridge()
    {
        if (!drawbridge.isPlaying)
        {
            drawbridge.Play();
        }
    }

    public void PlayPlatform()
    {
        if (!platform.isPlaying)
        {
            platform.Play();
        }
    }

    public void PlayRock()
    {
        rockThud.Play();
    }

    public void PlayChime(int num)
    {
        // play a different chime sound based on num
        if(num == 1)
        {
            chimeOne.Play();
        }
        else if (num == 2)
        {
            chimeTwo.Play();
        }
        else if (num == 3)
        {
            chimeThree.Play();
        }
        else if (num == 4)
        {
            chimeFour.Play();
        }
    }

    public void PlayChimeWrong()
    {
        chimeWrong.Play();
    }

    public void PlayDropletDry(int num)
    {
        if(num == 1)
        {
            dropletDryOne.Play();
        }
        else if (num == 2)
        {
            dropletDryTwo.Play();
        }
        else if (num == 3)
        {
            dropletDryThree.Play();
        }
    }

    public void PlayDropletWet(int num)
    {
        if (num == 1)
        {
            dropletWetOne.Play();
        }
        else if (num == 2)
        {
            dropletWetTwo.Play();
        }
        else if (num == 3)
        {
            dropletWetThree.Play();
        }
    }

    public void PlayWind()
    {
        // Fade in the wind sound
        StartCoroutine(FadeIn(windChimeAmbiance, fadeDuration));
        StartCoroutine(FadeIn(windAmbiance, fadeDuration));
    }

    public void StopWind()
    {
        // Fade out the wind sound
        StartCoroutine(FadeOut(windChimeAmbiance, windDuration));
        StartCoroutine(FadeOut(windAmbiance, windDuration));
    }

    public void PlaySnowflakeHit()
    {
        snowflakeHit.Play();
    }

    public void PlaySpikeHit()
    {
        spikeHit.Play();
    }

    public void PlayWaterSplash()
    {
        waterSplash.Play();
    }

    public void PlayWallExplode()
    {
        wallExplode.Play();
    }

    public void PlayHelpAnimal()
    {
        helpAnimal.Play();
    }

    public void PlayFinalMusic()
    {
        finalMusic.Play();
    }

    public void PlayBees()
    {
        bees.Play();
    }

    public void PlayVineHit()
    {
        vineHit.Play();
    }

    public void PlayBirdTweet()
    {
        birdTweet.Play();
    }

    public void PlaySnowBall()
    {
        snowBall.Play();
    }
    public void PlayPlantGrow()
    {
        plantGrow.Play();
    }
    public void PlayGainLeafMusic()
    {
        gainLeafMusic.Play();
    }
    public void PlayLoseLeafMusic()
    {
        loseLeafMusic.Play();
    }

    public void PlayPlantRustle()
    {
        plantRustle.Play();
    }
    public void PlayPop()
    {
        pop.Play();
    }
    public void PlaySnowballRoll()
    {
        snowballRoll.Play();
    }
    public void StopSnowballRoll()
    {
        snowballRoll.Stop();
    }
}
