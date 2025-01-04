using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class BalloonSensorScript : MonoBehaviour
{
    public float speed; // Speed of balloon movement
    public float verticalSpeed; // Speed of vertical movement
    private Rigidbody2D rb;
    public ScoreManager scoreManager; // Reference to the ScoreManager
    public float fallingSpeed = 1f; //Speed of the balloon with hole whe falling down
    private int collisionCount = 0; // Track the number of collisions
    private Animator BalloonAnimator; // Reference to the balloon Animator component
    public Animator HeartBarAnimator; // Reference to the heart bar animator component
    public GameObject AirPuffPrefab; // Reference to the airpuff prefab
    public GameObject ExplosionPrefab; // Reference to the explosion smoke prefab
    public MainMenu mainMenu;

    public AudioSource batAudioSource;
    public AudioSource looseAudioSource; 

    // Positions for air puff to spawn near holes
    private Vector3 firstHoleLocalPosition;
    //private Vector3 secondHoleLocalPosition;

    private Boundary boundaryScript; //Refering to the boundary script

    // Serial port for sensor data
    SerialPort serialPort;
    //public string portName;
    public int baudRate = 9600;

    private float averaged_ax = 0;
    private float averaged_ay = 0;
    private float averaged_az = 0;

    // Smoothing variables
    private Vector2 previousSpeed = Vector2.zero;
    private Vector2 targetSpeed = Vector2.zero;
    private Vector2 currentSpeed = Vector2.zero; // Holds the current smoothed speed
    public float smoothingFactor = 0.2f; // Increase for more responsiveness

    public TMP_Text AXText;
    public TMP_Text AYText;

    void Start()
    {
        string portName = MainMenu.selectedPort;
        Debug.Log("Selected Port: " + portName);

        ConnectToPort(portName);

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

    private void ConnectToPort(string port)
    {
        try
        {
            serialPort = new SerialPort(port, baudRate);
            serialPort.Open();
            Debug.Log("Connecting to port: " + port);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error connecting to port: " + ex.Message);
        }
    }

    void Update()
    {
        if (serialPort != null && serialPort.IsOpen && serialPort.BytesToRead > 0)
        {
            try
            {
                string line = serialPort.ReadLine();
                string[] sensorData = line.Split(',');

                // Hadis2
                averaged_ay = -int.Parse(sensorData[0]);
                averaged_ax = int.Parse(sensorData[1]);
                averaged_az = int.Parse(sensorData[2]);  

                // Detect downward motion based on the clue
                if (averaged_az < 13000)
                {
                   // Debug.Log("Downward motion detected");
                    DetectDownwardMotion(); // Trigger downward motion event
                  

                }
          
           
                // Hadis2
                targetSpeed = new Vector2(averaged_ax * speed, averaged_ay * verticalSpeed);
                currentSpeed = Vector2.Lerp(currentSpeed, targetSpeed, smoothingFactor);
                rb.linearVelocity = currentSpeed;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error reading serial data: " + e.Message);
            }
        }

        AXText.text = averaged_ax.ToString("F2");
        AYText.text = averaged_ay.ToString("F2");

        // if F12 is pressed
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Application.Quit();
        }
    }

// Method to handle downward motion
private void DetectDownwardMotion()
{
    Collider2D BalloonCollider = GetComponent<Collider2D>();
    // Find all meteors currently colliding with the playeritio
    Collider2D[] hits = Physics2D.OverlapCircleAll(BalloonCollider.bounds.center, 2.5f); // Adjust radius as needed

    foreach (Collider2D hit in hits)
    {
        if (hit.CompareTag("Meteor")) // Check if the object is a meteor
        {
            DestroyMeteor(hit.gameObject);
        }
    }
}

// Method to destroy a meteor
private void DestroyMeteor(GameObject meteor)
{
    // Instantiate the explosion at the meteor's position
    Instantiate(ExplosionPrefab, meteor.transform.position, Quaternion.identity);

    Destroy(meteor);
    Debug.Log("Meteor destroyed due to downward motion!");
}
    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
            Debug.Log("Serial port closed.");
        }
    }
private void OnDrawGizmos()
{
    Gizmos.color = Color.red; // Choose a color for the circle
    Gizmos.DrawWireSphere(transform.position, 2.5f); // Replace 1.0f with the actual radius used
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            scoreManager.IncreaseCoinScore(1.0f); // Call the IncreaseScore method on the ScoreManager instance
            Destroy(other.gameObject); // Destroy the coin
            Debug.Log("Coin collected"); // Debug log
        }
        else if (other.CompareTag("Bat"))
        {
            Debug.Log("Collision with Bat detected");
            batAudioSource.Play();

            collisionCount++;
            if (collisionCount == 1)
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
        else if (collisionCount == 3)
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

        looseAudioSource.Play();

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