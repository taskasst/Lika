using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishGame : MonoBehaviour
{
    public string levelToLoad = "Credits";
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            // Fade out to main menu
            StartCoroutine("EndGame");
        }
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelToLoad);
    }
}
