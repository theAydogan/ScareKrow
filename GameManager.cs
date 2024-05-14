using UnityEngine;
using UnityEngine.SceneManagement;

// this script manages game-related information, including crop count, scoring, doggo ability cooldown,
// and game over logic in a Unity game. It updates UI elements and handles spawning,
// destruction, and restart functionalities.


public class GameManager : MonoBehaviour
//I, Ahmet Aydogan, 000792453 certify that this material is my original work.No other person's work has been used without due acknowledgement.
{
    private int cropsAlive; // count of remaining crops in the game
    public RandomSpawner randomSpawner; // reference to the RandomSpawner script to have a variable to store the spawn points inside the crowSpawner object
    public RandomSpawner crowSpawner; // reference to the RandomSpawner specifically for spawning crows
    public static GameManager instance;
    public int score = 0;               // players score
    public HealthBarScript healthBar; // reference to the healthbar using the HealthBarScript
    public float minXBound = -10f; //boundaries on the x axis so the player cant fall off the world
    public float maxXBound = 10f;

    public Doggo doggo; // reference to the Doggo script

    private int doggoCooldown = 45;
    private int doggoCooldownTimer;

    void Start()
    {
        // set the initial amount of crops alive based on the number of crops at the beginning using the objects that have the tag "Crop" attached to them
        cropsAlive = GameObject.FindGameObjectsWithTag("Crop").Length;

        // initialize the doggo timer
        doggoCooldownTimer = doggoCooldown;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // ensures only one instance of GameManager exists, I had an issue where the game would have two players inside of it but this seemed to solve it.
        }
    }

    void Update()
    {
        if (doggoCooldownTimer > 0)
        {
            doggoCooldownTimer -= Mathf.CeilToInt(Time.deltaTime);
        }

        // checks if Doggo ability is available
        if (score >= 5 && doggoCooldownTimer <= 0)
        {
            doggo.EnableDoggoAbility();
        }
    }

    public void CropDestroyed()
    {
        cropsAlive--;  // decrease the count of remaining crops

        if (cropsAlive <= 0)  // checkd if all crops are destroyed and triggers the game over method if true
        {
            GameOver();
        }
        // calls the method to update the UI for the healthbar when a crop dies
        UpdateHealthBar();
    }

    private void UpdateHealthBar() // method to update the health bar UI based on the remaining crops
    {
        // calculate the percentage of crops alive
        float percentage = (float)cropsAlive / (float)4;

        // update the health bar size
        healthBar.SetSize(percentage);
    }

    private void GameOver()
    {
        // save the score of the player to PlayerPrefs so we can access it in different scenes without losing information
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();

        // finds all the crows on screen, destroys them to reduce the load on the GPU and then moves the player to the gameOverScreen
        GameObject[] crows = GameObject.FindGameObjectsWithTag("Crow");
        foreach (GameObject crow in crows)
        {
            Destroy(crow);
        }

        SceneManager.LoadScene("gameOverScreen");
    }

    // if the user presses play again in the gameover scene it restarts the game again
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // when the wave advances this method gets called which
    public void OnWaveChanged(int newWave)
    {
        // updates the score accoding to the newWave parameter
        score = newWave;

        // resets doggos cooldown timer
        doggoCooldownTimer = doggoCooldown;
    }
}
