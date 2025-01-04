using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CityScrollingBackground : MonoBehaviour
{
    public float scrollSpeed = 2f;       // Speed at which the background scrolls
    public float scrollDistance = 10f;   // The distance the background should scroll
    public Image fadeOverlay;            // Reference to the UI Image overlay for fade effect
    public Canvas congratsCanvas;        // Reference to the Congrats Canvas
    public float fadeDuration = 1.5f;    // Duration for the fade effect

    private Vector3 startingPosition;    // The initial position of the background
    private bool isScrolling = true;     // Whether or not the background is currently scrolling

    void Start()
    {
        startingPosition = transform.position;
        congratsCanvas.gameObject.SetActive(false); // Make sure the Congrats Canvas is initially inactive
        fadeOverlay.color = new Color(0, 0, 0, 0);  // Set initial fade overlay color to transparent
    }

    void Update()
    {
        if (isScrolling)
        {
            // Move the background downwards at a constant speed
            transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

            // Check if the background has scrolled the desired distance
            if (Vector3.Distance(startingPosition, transform.position) >= scrollDistance)
            {
                isScrolling = false;  // Stop scrolling once the distance is reached
                StartCoroutine(FadeToBlackAndShowCongrats()); // Start the fade-out coroutine
            }
        }
    }

    private IEnumerator FadeToBlackAndShowCongrats()
    {
        float elapsedTime = 0f;
        Color initialColor = fadeOverlay.color;

        // Gradually increase the alpha value of the fadeOverlay to create a fade-in effect
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeOverlay.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        // After fading, activate the Congrats canvas
        congratsCanvas.gameObject.SetActive(true);
    }

    // Optional: Call this function when the Congrats screen button is pressed to return to Main Menu
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
