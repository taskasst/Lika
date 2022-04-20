using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class Pause : MonoBehaviour {
    public GameObject Panel;
    public GameObject Button1;
    public GameObject Button2;
    RaycastHit hit;

	// Fairy cursor icon gets in the way of raycast
	// So we have to disable it in order to use buttons
	public GameObject fairyCursor;

    // Use this for initialization
    void Start () {
        
        Time.timeScale = 1;
        Panel.SetActive(false);
        Button1.SetActive(false);
        Button2.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        // ------------------------------- MOBILE CHANGE LATER -------------------------------
        if (Input.GetKey(KeyCode.Escape) && Time.timeScale==1)
        {
            Time.timeScale = 0;
            Panel.SetActive(true);
            Button1.SetActive(true);
            Button2.SetActive(true);

			fairyCursor.SetActive (false);
			Cursor.visible = true;
        }
    }

	public void Resume()
	{
		Time.timeScale = 1;
		Panel.SetActive(false);
		Button1.SetActive(false);
		Button2.SetActive(false);

		fairyCursor.SetActive (true);
		Cursor.visible = false;
	}

	public void MainMenu()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene("MainMenu");
	}
}
