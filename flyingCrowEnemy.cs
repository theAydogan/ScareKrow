using System.Collections.Generic;
using UnityEngine;

public class flyingCrowEnemy : MonoBehaviour

//I, Ahmet Aydogan, 000792453 certify that this material is my original work.No other person's work has been used without due acknowledgement.
{
    public float crowSpeed;
    public int maxHealth = 3;
    private int currentHealth;
    public int damage = 10;
    private GameObject crop;
    private SpriteRenderer spriteRenderer;
    public GameObject explosionPrefab;
    [SerializeField] private AudioSource crowDeath;

    void Start()
    {
        crop = GameObject.FindGameObjectWithTag("Crop");
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (crop == null)
        {
            return;
        }

        Chase();
    }

    private void Chase()
    {
        // get all crop objects with the "Crop" tag
        GameObject[] cropObjects = GameObject.FindGameObjectsWithTag("Crop");
        CropScript[] crops = new CropScript[cropObjects.Length];

        // get the CropScript components from the crop objects
        for (int i = 0; i < cropObjects.Length; i++)
        {
            crops[i] = cropObjects[i].GetComponent<CropScript>();
        }

        // create a list to store alive crops
        List<CropScript> cropList = new List<CropScript>();

        // populate the crop list with alive crops
        foreach (GameObject cropObject in GameObject.FindGameObjectsWithTag("Crop"))
        {
            CropScript crop = cropObject.GetComponent<CropScript>();
            if (crop != null)
            {
                cropList.Add(crop);
            }
        }
        crops = cropList.ToArray();

        // exit the method if no crops are available
        if (crops.Length == 0)
        {
            return;
        }

        // get a random alive crop as the target
        CropScript randomCrop = GetRandomCrop(crops);

        // calculate target and current positions
        Vector2 targetPosition = randomCrop.transform.position;
        Vector2 currentPosition = transform.position;

        // determine the direction
        Vector2 direction = (targetPosition - currentPosition).normalized;

        // move towards the target position with a controlled speed
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, crowSpeed);
    }

    private CropScript GetRandomCrop(CropScript[] crops)
    {
        List<CropScript> aliveCrops = new List<CropScript>();

        foreach (var crop in crops)
        {
            if (crop != null && crop.IsAlive())
            {
                aliveCrops.Add(crop);
            }
        }

        if (aliveCrops.Count > 0)
        {
            // Select a random alive crop as the new target
            return aliveCrops[Random.Range(0, aliveCrops.Count)];
        }
        else
        {
            // If no alive crops are available, return null
            return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // check if the collision is with a crop
        if (other.CompareTag("Crop"))
        {
            // damage the crop
            CropScript cropScript = other.GetComponent<CropScript>();
            if (cropScript != null)
            {
                cropScript.TakeDamage(damage);
                Debug.Log($"Crop took {damage} damage. Current health: {cropScript.currentHealth}");
                PlayExplosionEffect(); // play explosion effect
                crowDeath.Play(); // play crow death sound
            }
            // destroy the crow
            Destroy(gameObject);
        }
    }

    void PlayExplosionEffect()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 1.0f);
        crowDeath.Play();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            crowDeath.Play();
        }
    }
}
