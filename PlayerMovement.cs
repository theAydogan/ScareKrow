using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
//I, Ahmet Aydogan, 000792453 certify that this material is my original work.No other person's work has been used without due acknowledgement.
{
    public float moveSpeed = 5f; //player movement speed
    public float jumpForce = 100f; //player jump force
    public scarecrowProjectileBehaviour scarecrowPrefab; // reference to be able to access the scareCrowProjectileBehaviour script because this movement script will be attached to the player object and both need to work with one another
    public Transform shootOffset; //shooting point position
    

    private enum MovementState { idle, running, jumping, falling } // enum to represent different movement states of the player which are all defined in the animator object attached to the player object
    private MovementState state = MovementState.idle; // the players current state though will be set to idle

    private Rigidbody2D rb = null;
    private Animator anim = null;
    private SpriteRenderer spritePlayer = null;

    private float horizontalInput; // input for horizontal movement
    private bool isJumping = false; //flag to see if the player is on the ground or is in the air and jumping

    private Vector2 initialFiringPointPosition; // a vector type to hold the position of where the firing point of the scare crow projectiles will come out from

    // minimum and maximum x bounds for player movement
    public float minXBound = -8f;
    public float maxXBound = 8f;

    [SerializeField] private AudioSource scareCrowShoot; //audio ref for when the player shoots a scare crow

    private void Awake()
    {
        // initializes the references to components attached to the player object
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spritePlayer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // when the game begins this stores the initial position of the firing point
        initialFiringPointPosition = shootOffset.localPosition;
    }

    private void Update() //this gets called every frame
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");  // gets the horizontal input from the player

        // calculate the new position of the player according to the coordinates and the movement speed, along with the rigidbody component to move along with it so it makes sure the player doesnt just fall right through the floor while moving horizontally
        Vector2 moveDirection = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        float newXPosition = Mathf.Clamp(rb.position.x + moveDirection.x * Time.deltaTime, minXBound, maxXBound);
        rb.position = new Vector2(newXPosition, rb.position.y);

        // handles jumping input, if spacebar is pressed and the player is not already jumping it sets the isjumping flag to true and moves the player object up in the y axis by the force value defined in the beginning
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            isJumping = true;
            moveDirection.y = jumpForce;
        }
        //if the player presses either left shift or right shift key it will shoot a scarecrow projectile
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            ShootScarecrow();
        }

        rb.velocity = moveDirection;

        UpdateAnimationState();   // calls the method that handles the animation state based on player's movement
        HandleSpriteDirection(); // calls the method that handles the player objects sprite directuib based on player's movement once again
    }

    void ShootScarecrow(){
        scarecrowProjectileBehaviour projectile = Instantiate(scarecrowPrefab, shootOffset.position, transform.rotation);

        // set the direction of the projectile based on player's facing direction and play the sound effect that is linked to the shooting of a scareCrow projectile
        if (spritePlayer.flipX)
        {
            projectile.SetDirection(-1);
            scareCrowShoot.Play();
        }
        else
        {
            projectile.SetDirection(1);
            scareCrowShoot.Play();
        }
    }

    // this method is called when the player is colliding with the ground
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    private void HandleSpriteDirection()
    {
        //checks player moving right and flips the shooting poiint accordingly to the same direction
        if (horizontalInput > 0) // 
        {
            spritePlayer.flipX = false;
            shootOffset.localPosition = initialFiringPointPosition; 
            shootOffset.rotation = Quaternion.Euler(0, 0, 0);
        }

        else if (horizontalInput < 0) 
        {
            spritePlayer.flipX = true;
            shootOffset.localPosition = new Vector2(-initialFiringPointPosition.x, initialFiringPointPosition.y);
            shootOffset.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void UpdateAnimationState()
    {
        // set movement state to running if the player is moving horizontally, this will then play the running animation
        if (horizontalInput != 0)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idle;
        }
        // set movement state to jumping if the player is moving upwards, this will then play the jumping animation
        if (rb.velocity.y > 0.1f)
        {
            state = MovementState.jumping;
        }
        // set movement state to falling if the player is falling downwards, this will then play the falling animation
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }
        // update the animator with the current movement state
        anim.SetInteger("state", (int)state);
    }
}


