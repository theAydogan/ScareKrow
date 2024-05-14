using UnityEngine;

public class CropScript : MonoBehaviour
//I, Ahmet Aydogan, 000792453 certify that this material is my original work.No other person's work has been used without due acknowledgement.
{
    public int maxHealth = 10; //sets the max health of a crop object
    public int currentHealth; // variable to see what the current health of the crop object is
    private GameManager gameManager; // reference to gamemanager script
    public GameObject explosionPrefab; // ref to explosion pre fab object
    [SerializeField] private AudioSource crowDeath; // ref to audio for a crow death

    private void Start()
    {
        currentHealth = maxHealth; //sets the initial health of the crop objects to 10
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check if the collided object has the "Crow" tag
        if (collision.gameObject.CompareTag("Crow"))
        {
            TakeDamage(10);
            PlayExplosionEffect();
            crowDeath.Play();
        }
    }

    void PlayExplosionEffect()
    {
        // this line places the explosion prefab at the crow's position
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        // destroy the explosion effect after a certain duration
        Destroy(explosion, 1.0f);
        crowDeath.Play();
    }

    public void TakeDamage(int damage)
    {
        // decrease the current health of the crop
        currentHealth -= damage;
        // check if the crow's health is depleted
        if (currentHealth <= 0)
        {
            // if it is then destroy the crop
            DestroyCrop();
        }
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    private void DestroyCrop()
    {
        // tells the gameManager that a crop has been destroyed
        gameManager.CropDestroyed();
        crowDeath.Play();
        Destroy(gameObject);
    }

}

