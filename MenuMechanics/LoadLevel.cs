using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // include so we can load new scenes
using UnityEngine.UI; // include UI namespace since references UI Buttons directly

public class LoadLevel : MonoBehaviour {
    public AudioClip sound;
    AudioSource audioSource;
    private IEnumerator coroutine;
	public Fade fade;
    public AudioSource music;
    public GameObject mainMenu;
    public GameObject levelsMenu;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
        GameObject musicPlayer = GameObject.FindGameObjectWithTag("Music");
        music = musicPlayer.GetComponent<AudioSource>();
    }
	
    public IEnumerator WaitAndLoad(float waitTime, string levelToLoad)
    {
        StartCoroutine(FadeOut(music, .3f));
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(levelToLoad);
    }
    public void loadLevel(string level)
    {
		fade.FadeIn(level);
        StartCoroutine(WaitAndLoad(2f, level));
    }

    public void ShowMenu(string menu)
    {
        if(menu == "Main")
        {
            levelsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        else if(menu == "Level")
        {
            mainMenu.SetActive(false);
            levelsMenu.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlaySound()
    {
        audioSource.PlayOneShot(sound, .7f);
    }
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
}
