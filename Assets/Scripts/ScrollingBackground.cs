using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScrollingBackground : MonoBehaviour
{
    //public GameManagerScript GameManager;  // Reference to the Game manager script
    public float scrollSpeed;
    public GameObject clouds; // Reference to the clouds object
    [SerializeField] private Renderer bgrenderer;
    public float distanceTraveled = 0f;  // Track the total distance traveled
    public float distanceToCity = 1000f;  // The total distance needed to reach the city
    public float distanceToEnableClouds = 900f; // The distance at which the clouds should be enabled
     public float cloudScrollSpeedMultiplier = 5f;  // Clouds scroll faster than the background
    private bool cloudsEnabled = false;  // Track if the clouds have been enabled

   
   void Start()
   {
     if(clouds!= null)
     {
        clouds.SetActive(false);
     }
   }    // Update is called once per frame
    void Update()
    {
        bgrenderer.material.mainTextureOffset += new Vector2(0, Time.deltaTime * scrollSpeed);
        //Increment the distance based on background scroll speed and time passed
        distanceTraveled += scrollSpeed * Time.deltaTime;

       
         // Enable the clouds at the specified distance
        if (!cloudsEnabled && distanceTraveled >= distanceToEnableClouds)
        {
            clouds.SetActive(true);
            cloudsEnabled = true;
        }

       if (cloudsEnabled && clouds != null)
        {
            clouds.transform.position += Vector3.down * scrollSpeed * cloudScrollSpeedMultiplier * Time.deltaTime;
        }
         // Check if player has reached the city
        if (distanceTraveled >= distanceToCity)
        {
            SceneManager.LoadScene("CityScene");
            //TriggerEndSceneTransition();
        }
    }
   /* private void TriggerEndSceneTransition()
    {
        // Use SceneTransitionManager to transition to the final city scene
        GameManager.EndingSceneTransition();

        //UnityEngine.SceneManagement.SceneManager.LoadScene("CityScene");

    } */

    public float GetProgressPercentage()
    {
        // Return the percentage of the journey completed
        return Mathf.Clamp01(distanceTraveled / distanceToCity);
    }
}

