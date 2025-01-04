using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BalloonScript : MonoBehaviour
{
    

    public float speed = 5f; // Speed of balloon movement
    public float verticalSpeed = 5f; // Speed of vertical movement
    private Rigidbody2D rb;
    public ScoreManager scoreManager; // Reference to the ScoreManager
    public float fallingSpeed= 1f; //Speed of the balloon with hole whe falling down
    private int collisionCount = 0; // Track the number of collisions
    private Animator BalloonAnimator; // Reference to the balloon Animator component
    public Animator HeartBarAnimator; // Reference to the heart bar animator component
    public GameObject AirPuffPrefab; // Reference to the airpuff prefab
   // private Vector3 balloonSize; // Size of the balloon sprite

    // Positions for air puff to spawn near holes
    private Vector3 firstHoleLocalPosition;
    //private Vector3 secondHoleLocalPosition;

    private Boundary boundaryScript; //Refering to the boundary script



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Disable gravity for the balloon
        // Ensure scoreManager is assigned to avoid null reference issues
        if (scoreManager == null)
        {
            Debug.LogError("ScoreManager not assigned in the Inspector!");
        }
        BalloonAnimator = GetComponent<Animator>(); //get the balloon animator component
      if (HeartBarAnimator == null)
        {
            Debug.LogError("heartbar animation is not assigned in the inspector");
       }
       
        // Calculate the local positions of the holes using double precision
        CalculateHolePositions();

        boundaryScript = GetComponent<Boundary>();
    }

    void Update()
    {
        // Combine horizontal and vertical input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Create a movement vector based on input
        Vector2 movement = new Vector2(horizontalInput * speed, verticalInput * verticalSpeed);

        // Apply movement
        transform.Translate(movement * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            scoreManager.IncreaseCoinScore(0.5f); // Call the IncreaseScore method on the ScoreManager instance
            Destroy(other.gameObject); // Destroy the coin
            Debug.Log("Coin collected"); // Debug log
        }
         else if (other.CompareTag("Bat")) 
        {
            Debug.Log("Collision with Bat detected");

            collisionCount++;
            if(collisionCount == 1)
            {
                TriggerFirstHole();
                SpawnAirPuff(firstHoleLocalPosition, Quaternion.identity); // No rotation for the first air puff
            }
        }
             if (collisionCount == 2)
            {
                TriggerSecondHole();
               // SpawnAirPuff(secondHoleLocalPosition, Quaternion.Euler(0, 0, 180)); // Rotate the second air puff by 180 degrees on the Z-axis

        }
             else if(collisionCount == 3)
            {
                HeartBarAnimator.SetTrigger("ThirdCollision"); // this will trigger the transition to the third state
                ActivateGravityOnBalloon();
            }

           
            
        }

     void CalculateHolePositions()
     {
         // Get the SpriteRenderer component
         SpriteRenderer balloonRenderer = GetComponent<SpriteRenderer>();

         // Ensure the balloonRenderer is valid
         if (balloonRenderer == null || balloonRenderer.sprite == null)
         {
             Debug.LogError("Balloon SpriteRenderer or sprite is missing!");
             return;
         } 

         // Get the size of the sprite in local space, accounting for the local scale of the balloon
         Vector3 balloonSize = balloonRenderer.sprite.bounds.size; // Local size of the sprite
         Vector3 localScale = transform.localScale; // Get the local scale of the balloon GameObject

         // Multiply the sprite's size by the local scale to get the actual size in the world
         balloonSize = new Vector3(balloonSize.x * localScale.x, balloonSize.y * localScale.y, 0);

         // Debug the balloon size to check the values
        // Debug.Log("Balloon Size: " + balloonSize);

         // Calculate the positions for the holes using more precise double calculations, converted to float for Vector3
         double firstHoleX = balloonSize.x / 2;    // Near the right side
         double firstHoleY = -balloonSize.y / 25;  // Slightly towards the top-left
         double secondHoleX = -balloonSize.x / 2;  // Near the left side
         double secondHoleY = balloonSize.y / 4;   // Slightly towards the bottom-right

         // Convert the double values to floats and store in Vector3 for local positioning
         firstHoleLocalPosition = new Vector3((float)firstHoleX, (float)firstHoleY, 0);
        // secondHoleLocalPosition = new Vector3((float)secondHoleX, (float)secondHoleY, 0);

         // Debug the hole positions to check the values
        // Debug.Log("First Hole Position: " + firstHoleLocalPosition);
        // Debug.Log("Second Hole Position: " + secondHoleLocalPosition);
     } 

    void TriggerFirstHole()
    {
        // Set Animator parameter or trigger for the first hole
       BalloonAnimator.SetTrigger("FirstHole"); // This will trigger the transition to the first hole animation state
       HeartBarAnimator.SetTrigger("FirstCollision"); // this will trigger the transition to the first hear bar state
        Debug.Log("Triggered first hole animation & first hearbat animation");
    }

    void TriggerSecondHole()
    {
        // Set Animator parameter or trigger for the second hole
        BalloonAnimator.SetTrigger("SecondHole"); // This will trigger the transition to the second hole animation state
        HeartBarAnimator.SetTrigger("SecondCollision"); // this will trigger the transition to the Second collision state
        Debug.Log("Triggered second hole animation & the second heartbar animation");
    }


    // Spawn the AirPuff prefab at the specified position
    void SpawnAirPuff(Vector3 localPosition, Quaternion rotation)
    {
        // Instantiate air puff at the balloon's position plus the local offset for the hole
        Vector3 spawnPosition = transform.position + localPosition;
        spawnPosition.z = -2; // Move the air puff to the front (adjust this as needed)

        GameObject airPuff = Instantiate(AirPuffPrefab, spawnPosition, rotation);

        // Set the air puff as a child of the balloon, so it moves with the balloon
        airPuff.transform.SetParent(transform);

        // Destroy the air puff object after the animation finishes (e.g., 1 second)
        Destroy(airPuff, 1f); // Adjust the time based on your animation length
    }

   



    void ActivateGravityOnBalloon()
    {
        // Find the Balloon GameObject (if not already referenced)
        GameObject Balloon = GameObject.FindWithTag("Balloon");

        if (Balloon != null)
        {
            // Get the Rigidbody2D component attached to the Balloon
            Rigidbody2D RB = Balloon.GetComponent<Rigidbody2D>();

            if (RB != null)
            {
                // Set gravityScale to 1 to enable gravity and make the balloon fall
                RB.gravityScale = fallingSpeed;

                // Debug log to confirm gravity activation
                Debug.Log("Gravity activated on Balloon"); 

                // Deactivate boundary to allow falling out of screen
                boundaryScript.DeactivateBoundary();

                //  Add a script to handle when the balloon exits the camera view**
                Balloon.AddComponent<BalloonExitHandler>();
            }
            
        }
        else
        {
            Debug.LogError("Balloon instance not found");
        }
    }
}

       
public class BalloonExitHandler : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        // Trigger the game over screen from the GameManager once balloon exits camera view
        Object.FindFirstObjectByType<GameManagerScript>().GameOver();
        Debug.Log("Balloon has exited the camera view. Game Over triggered.");
    }
}
  




