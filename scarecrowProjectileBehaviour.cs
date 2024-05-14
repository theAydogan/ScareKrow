using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script controls the behavior of scarecrow projectiles in the game
// these projectiles move in a specified direction and trigger damage to flying crow enemies upon collision
// an explosion effect and crow death sound are played upon collision with a crow


public class scarecrowProjectileBehaviour : MonoBehaviour
//I, Ahmet Aydogan, 000792453 certify that this material is my original work.No other person's work has been used without due acknowledgement.
{
    public float speed = 4.5f; // speed of the scarecrow projectile
    private int direction = 1; // direction in which the projectile moves (1 for right, -1 for left)


    public GameObject explosionPrefab; // reference to the explosion prefab to be instantiated on collision

    [SerializeField] private AudioSource crowDeath; // reference to the audio source for crow death sound

    void Update()
    {
        // move the projectile in the specified direction at a constant speed
        transform.position += direction * transform.right * Time.deltaTime * speed;
    }

    public void SetDirection(int dir)
    {
        direction = dir;
    }

    // when a collision between two 2Dcollision components happens this method runs
    void OnCollisionEnter2D(Collision2D collision)
    {
        // checks if the collided object is the flying crow enemy, uses the tag "Crow" which i have assigned to the Crow objects
        if (collision.gameObject.CompareTag("Crow"))
        {
            // retrieves the flyingCrowEnemy script from the collided object, because each Crow object has a flyingCrowEnemy script attached to it
            flyingCrowEnemy enemyScript = collision.gameObject.GetComponent<flyingCrowEnemy>();

            // checks if the script is not null
            if (enemyScript != null)
            {
                // calls the TakeDamage method in the flyingCrowEnemy script
                enemyScript.TakeDamage(1);
                PlayExplosionEffect(); // plays the explosion effect upon collision
            }
        }

        // destroys the projectile regardless of what it collides with, it can only collide with crows anyways so its no an issue
        Destroy(gameObject);
    }

    void PlayExplosionEffect()
    {
        // makes an instance of the explosion prefab at the projectile's position
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // play the sound effect if crowDeath is not null
        if (crowDeath != null)
        {
            crowDeath.Play(); // plays the sound
        }
        else
        {
            Debug.LogWarning("crowDeath AudioSource is null."); // this debug log is more for me to see if the audio file is not corrupted or placed into its inspector element
        }

        // destroy the explosion effect after a certain duration
        Destroy(explosion, 1.0f);
    }

}
