using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public ScrollingBackground gameProgress;  // Reference to the GameProgress script
    public Slider progressBar;         // Reference to the UI slider

    void Update()
    {
        // Update the progress bar with the player's progress
        progressBar.value = gameProgress.GetProgressPercentage();
    }
}
