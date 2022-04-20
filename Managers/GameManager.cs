using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager

public class GameManager : MonoBehaviour
{
    // static reference to game manager so can be called from other scripts directly (not just through gameobject component)
    public static GameManager gm;

    public enum Season { Spring, Summer, Fall, Winter };
    public Season season = Season.Spring;

    // UI elements to control
    //public Text UILevel;
    public GameObject UIGamePaused;

    // private variables
    GameObject _player;
    Vector3 _spawnLocation;
    Scene _scene;

	public Fade fadeObject;
    public AudioSource music;

    // set things up here
    void Awake()
    {
        //GameObject musicPlayer = GameObject.FindGameObjectWithTag("Music");
        //music = musicPlayer.GetComponent<AudioSource>();
        // setup reference to game manager
        if (gm == null)
            gm = this.GetComponent<GameManager>();

        // setup all the variables, the UI, and provide errors if things not setup properly.
        setupDefaults();
    }

    // game loop
    void Update()
    {
        // if ESC pressed then pause the game
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0f)
            {
                UIGamePaused.SetActive(true); // this brings up the pause UI
                Time.timeScale = 0f; // this pauses the game action
            }
            else
            {
                Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
                UIGamePaused.SetActive(false); // remove the pause UI
            }
        }*/
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            // User switched away from the game
            // Pause
            UIGamePaused.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // setup all the variables, the UI, and provide errors if things not setup properly.
    void setupDefaults()
    {
        // setup reference to player
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("LeafSwarm"); //the "player"/what we need to keep track of is the leaf swarms

        if (_player == null)
            Debug.LogError("Player not found in Game Manager");

        // get current scene
        _scene = SceneManager.GetActiveScene();

        // get initial _spawnLocation based on initial position of player
        _spawnLocation = _player.transform.position;

        SetScoreKeeperSeason();

        // friendly error messages
        //if (UILevel == null)
            //Debug.LogError("Need to set UILevel on Game Manager.");

        if (UIGamePaused == null)
            Debug.LogError("Need to set UIGamePaused on Game Manager.");

        // get the UI ready for the game
        refreshGUI();
    }


    // refresh all the GUI elements
    void refreshGUI()
    {
        // set the text elements of the UI
       // UILevel.text = _scene.name;
    }

    // public function for level complete
  //  public void LevelCompete()
  //  {
  //      // use a coroutine to allow the player to get fanfare before moving to next level
  //      StartCoroutine(LoadNextLevel());
  //  }

  //  // load the nextLevel after delay
  //  IEnumerator LoadNextLevel()
  //  {
		////fadeObject.FadeIn();

  //      //StartCoroutine(FadeOut(music, fadeObject.fadeDuration));


  //      yield return new WaitForSeconds(1.0f);
  //      SceneManager.LoadScene(levelAfterVictory);
  //  }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private void SetScoreKeeperSeason()
    {
        if(season == Season.Spring)
        {
            ScoreKeeper.skInstance.SetSeason(1);
        }
        else if (season == Season.Summer)
        {
            ScoreKeeper.skInstance.SetSeason(2);
        }
        else if (season == Season.Fall)
        {
            ScoreKeeper.skInstance.SetSeason(3);
        }
        else if (season == Season.Winter)
        {
            ScoreKeeper.skInstance.SetSeason(4);
        }
    }
}
