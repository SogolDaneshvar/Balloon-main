using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System;

public class GameManagerScript : MonoBehaviour
{
    //[SerializeField] private GameObject StartTransitionPrefab; // Reference to the start transition prefab
   // [SerializeField] private GameObject EndTransitionPrefab; // Reference to the end transition prefab
    //public Animator StartingTransitionAnimation;  // Reference to the start transition Animation component
    //public Animator EndingTransitionAnimation;    // Reference to the end transition Animation component
   // public string startTransitionClipName;      // Name of the start transition animation clip
  //  public string endTransitionClipName;        // Name of the end transition animation clip
   // public float transitionDuration = 1f;       // Duration of the transition animations (match to animation length)
    public GameObject gameOverCanvas; // Reference to the GameOverCanvas
    public TextMeshProUGUI CoinScoreText; // Reference to the score text UI element
    private CanvasGroup canvasGroup; // Reference to the Canvas Group for fading
    private ScoreManager scoreManager;
    void Start()
    {
       // StartingSceneTransition("MainScene"); //Start the transition when opening the main scene
        gameOverCanvas.SetActive(false); // Ensure the game over UI is hidden at start
        canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>(); // Get the Canvas Group component
        scoreManager =UnityEngine.Object.FindFirstObjectByType<ScoreManager>(); // Find the ScoreManager in the scene


    }

    /*

    // Call this method to start the transition to a new scene
    public void StartingSceneTransition(string sceneName)
    {
        StartCoroutine(PlayStartTransition(sceneName));
    }
    // Coroutine to handle the start transition and scene load
    private IEnumerator PlayStartTransition(string sceneName)
    {
        // Activate the start transition prefab
        StartTransitionPrefab.SetActive(true);

        //  Wait a frame to ensure the Animator initializes
        yield return null;

        // Play the "start transition" animation
        StartingTransitionAnimation.Play(startTransitionClipName);

        // Wait for the animation to finish (adjust transitionDuration to match animation length)
        yield return new WaitForSeconds(transitionDuration);

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // Deactivate the start transition prefab
        StartTransitionPrefab.SetActive(false);

    }

    // Call this method when ending a scene
    public void EndingSceneTransition()
    {
        StartCoroutine(PlayEndTransitionRoutine());
    }

    private IEnumerator PlayEndTransitionRoutine()
    {
        // Activate the end transition prefab
        EndTransitionPrefab.SetActive(true);

        //  Wait a frame to ensure the Animator initializes
        yield return null;

        // Play the "end transition" animation
        EndingTransitionAnimation.Play(endTransitionClipName);

        // Wait for the animation to finish
        yield return new WaitForSeconds(transitionDuration);

        //Deactivate the end transition prefab
        EndTransitionPrefab.SetActive(false);
    } */


    // Method to trigger the game over screen with fade-in
    public void GameOver()
    {
        gameOverCanvas.SetActive(true); // Activate the Canvas

        // Get the final score from the ScoreManager and display it
        float finalScore = scoreManager.GetScore();
        CoinScoreText.text = "Score: " + finalScore.ToString();

        StartCoroutine(FadeIn()); // Start the fade-in effect

        Time.timeScale = 0; // Pause the game
    }
    // Fade-in coroutine to gradually increase alpha
    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0; // Start fully transparent
        float duration = 1.5f; // Duration of fade-in
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time since time is paused
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration); // Gradually increase alpha
            yield return null;
        }
    }

    // Method linked to the Play Again button
    public void PlayAgain()
    {
        Debug.Log("Play again"); // Debug log
        Time.timeScale = 1; // Resume the game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}



