
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// this script controls the doggo powerup in the game. Doggo has a special ability
// to run and destroy all crows, but it comes with a cooldown period between uses. The script handles user input,
// triggers animations, manages cooldowns, and updates UI text for doggo's ability availability.


public class Doggo : MonoBehaviour
//I, Ahmet Aydogan, 000792453 certify that this material is my original work.No other person's work has been used without due acknowledgement.
{

    public Animator animator; // reference to the animator component for controlling animations
    public float runSpeed = 5.0f; // speed at which the doggo runs
    private Vector3 originalPosition; // original position and scale of the dog for resetting after using the ability, this way doggo runs forward and back to its original position and doesnt change its size according to animation and sprite changes, which was an issue for a bit for me
    private Vector3 originalScale;
    private bool doggoEnabled = false; // flag indicating whether the doggo ability is currently enabled
    private float lastDoggoTime; // timestamp of the last time the dog ability was used
    public Text doggoAvailabilityText; // UI text displaying doggo ability availability that is shown during the game
    private float doggoCooldownDuration = 10.0f; // duration of the cooldown period for the doggo ability, it is set to 10 seconds
    [SerializeField] private AudioSource doggoBark; // audio source for the doggo bark sound

    private void Start()
    {   
        if (animator == null)     // if the animator is not assigned, get it from the GameObject
        {
            animator = GetComponent<Animator>();
        }
        // store the original position and scale of the doggo according to the transform component's values listed in the inspector menu
        originalPosition = transform.position;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        UpdateDoggoAvailabilityText(); // update the UI text displaying doggo ability availability

        // if the doggo ability is enabled, handle user input
        if (doggoEnabled)
        {
            HandleInput();
        }
    }
    // check if the doggo can be used based on cooldown and score requirements
    private bool CanUseDoggo()
    {
        return doggoEnabled && (Time.time - lastDoggoTime >= doggoCooldownDuration);
    }


    private void HandleInput()
    {

        if (doggoEnabled && GameManager.instance.score >= 5 && Input.GetKeyDown(KeyCode.Z)) // check if the doggo ability is enabled, score(wave number) is sufficient, and the input key is pressed, I set it such that when the player reaches wave 5, doggo powerup becomes available then.

        {
            // if the ability can be used, play the bark sound, trigger run animation, and start the ability coroutine
            if (CanUseDoggo())
            {
                doggoBark.Play();
                animator.SetTrigger("RunTrigger");
                StartCoroutine(RunAndDestroyCrows());
                lastDoggoTime = Time.time;  // this line update the timestamp of the last usage
                doggoEnabled = false;
                StartCoroutine(DoggoCooldown());  // pretty straight forward, this starts the cooldown coroutine

            }
            else
            {
                // if the ability is on cooldown in this case it would be !CanUseDoggo, calculate the remaining cooldown time and I set it to a variable called remainingCooldown
                float remainingCooldown = lastDoggoTime + doggoCooldownDuration - Time.time;
            }
        }
    }

    private void UpdateDoggoAvailabilityText() // this method updates the UI text in the game displaying doggo availibility, i feel like im repeating myself constantly hahaha

    {
        if (CanUseDoggo())
        {
           doggoAvailabilityText.text = "Doggo Usable PRESS Z !";
        }
        else
        {
            float remainingCooldown = lastDoggoTime + doggoCooldownDuration - Time.time;
            doggoAvailabilityText.text = "Doggo Usable: False";
        }
    }

    // coroutine for the doggo's running ability and destroying crows
    private IEnumerator RunAndDestroyCrows()
    {
        Vector3 targetPosition = originalPosition - Vector3.right * 10.0f; // this is the target position in which i want doggo to run towards, I found the perfect range and so I set it as is.

        // this while loop moves doggo towards the target position and destroy crows in the path as it is on its way to the target position, destroying all crows before it reaches it
        while (transform.position != targetPosition)
        {

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, runSpeed * Time.deltaTime);
            DestroyCrows();
            yield return null;
        }

        // these 3 lines handle flipping doggos scale to show it turning around
        Vector3 flippedScale = transform.localScale;
        flippedScale.x *= -1;
        transform.localScale = flippedScale;

        // this variable which we introduced before is being overwritten to now adjust its value to doggos original position so he can run back to it
        targetPosition = originalPosition;

        // as doggo is running back to the original position it once again destroys crows in its path to show them his wrath (didnt mean to rhyme but it works)
        while (transform.position != targetPosition)
        {

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, runSpeed * Time.deltaTime);

            DestroyCrows();

            yield return null;
        }

        // trigger the idle animation and reset the doggos position and scale so he can sit and wag his tail like a good boy
        animator.SetTrigger("IdleTrigger");
        transform.position = originalPosition;
        transform.localScale = originalScale;
    }

    private void DestroyCrows() // this method just destroys all game objects with the "Crow" tag assigned to it

    {
        GameObject[] crows = GameObject.FindGameObjectsWithTag("Crow");
        foreach (GameObject crow in crows)
        {
            Destroy(crow);
        }
    }

    public void EnableDoggoAbility()
    {
        doggoEnabled = true;
    }

    // coroutine for the cooldown period of the doggo ability, its good to use coroutines in Unity from what I learned because
    // this way I dont need to wait for certain lines of code to execute, this code can run in the back along with other lines of code
    // which makes sense because you would want your player to be able to move for example and have other abilities run in the back
    private IEnumerator DoggoCooldown()
    {
        // wait for the specified cooldown duration
        yield return new WaitForSeconds(doggoCooldownDuration);

        doggoEnabled = true; //after cooldown durartion is compltete, set the flag to true
    }
}


