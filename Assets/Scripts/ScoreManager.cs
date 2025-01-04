using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI CoinScoreText;
    public TextMeshProUGUI MeteorScoreText;

    public AudioSource cionAudioSource;

    private float meteorScore;
    float CoinScore = 0;

    public void IncreaseCoinScore(float amount)
    {
        cionAudioSource.Play();
        CoinScore += amount;
        CoinScoreText.text = CoinScore.ToString(); // Display only the score number
        Debug.Log("Score updated: " + CoinScore); // Debug log
    }
    // Method to increase meteor score
    public void IncreaseMeteorScore(float amount)
    {
        meteorScore += amount;
        MeteorScoreText.text = meteorScore.ToString();
        Debug.Log("Meteor Score updated: " + meteorScore);
    }
    // Getter method to retrieve the current score
    public float GetScore()
    {
        Debug.Log("Score: " + CoinScore); // Debug log
        return (float)CoinScore; // Return the current score
    }
}
