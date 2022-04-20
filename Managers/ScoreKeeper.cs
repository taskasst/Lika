using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
	public static ScoreKeeper skInstance;

	// Total pickups the player has gotten throughout the game
	static public int totalScore;

	// Score from the last season
	static public int seasonScore;

	// Pickups collected in each season
	static public int springScore;
	static public int summerScore;
	static public int fallScore;
	static public int winterScore;

	// Max num pickups in the most recent leaf manager
	static public int seasonMaxPickups;

	// Max pickups so far in the game
    static public int totalMaxPickups;

    // The current season
	private bool spring;
    private bool summer;
    private bool fall;
	private bool winter;


	void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		if (skInstance == null)
        {
			skInstance = gameObject.GetComponent<ScoreKeeper>();
		}
        else
        {
			Destroy (gameObject);
		}
	}


	/* Gets the number of pickups from the set season */
	public int GetSeasonScore()
    {
        if (spring)
            return springScore;
        else if (summer)
            return summerScore;
        else if (fall)
            return fallScore;
        else if (winter)
            return winterScore;
        else
            return seasonScore;
	}


    /* Sets the current season */
    public void SetSeason(int season)
    {
        if (season == 1)
        {
            spring = true;
            summer = false;
            fall = false;
            winter = false;
        }
        else if (season == 2)
        {
            spring = false;
            summer = true;
            fall = false;
            winter = false;
        }
        else if (season == 3)
        {
            spring = false;
            summer = false;
            fall = true;
            winter = false;
        }
        else if (season == 4)
        {
            spring = false;
            summer = false;
            fall = false;
            winter = true;
        }
    }


	/* Gets the number of pickups the player has collected from all seasons */
	public int GetTotalScore()
    { 
		return totalScore; 
	}


	/* Gets the max number of pickups the player could have collected from all seasons */
	public int GetTotalMax()
    { 
		return totalMaxPickups; 
	}


	/* Gets the max number of pickups the player could have collected from the set season */
	public int GetSeasonMax()
    { 
		return seasonMaxPickups;
	}


    /* Sets the max number of pickups the player can collect from this season */
    public void SetSeasonMax(int max)
    {
        // Set this season's max
        seasonMaxPickups = max;

        // Set the total max pickups
        totalMaxPickups += seasonMaxPickups;
    }


	/* Sets the number of pickups the player has collected */
	public void UpdateScore(int pickups)
    {
		seasonScore = pickups;
		if (spring)
			springScore = pickups;
		else if (summer)
			summerScore = pickups;
		else if (fall)
			fallScore = pickups;
		else if (winter)
			winterScore = pickups;
		else
			Debug.Log ("Season not set in the score keeper");

		totalScore = springScore + summerScore + fallScore + winterScore;
    }
}
