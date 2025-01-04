using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerScript : MonoBehaviour
{
    public GameObject coinPrefab; // Reference to the coin prefab
    public GameObject BatPrefab; // Reference to the bat prefab
    public GameObject MeteorPrefab; // Reference to the meteor prefab

    public float spawnInterval = 2f; // Time between spawns
    public float xRange = 8f; // Horizontal range for spawning
    public float yStart = 5f; // Vertical start position for spawning

    private void Start()
    {
        // Start the spawning process
        InvokeRepeating("SpawnObject", spawnInterval, spawnInterval);
    }

    private void SpawnObject()
    {
        // Randomly choose a position within the xRange
        Vector2 spawnPosition = new Vector2(Random.Range(-xRange, xRange), yStart);

        // Randomly decide whether to spawn a coin, a bat, or a meteor
        float randomValue = Random.value;
        GameObject prefabToSpawn = null;

        if (randomValue < 0.4f) // 40% chance for coin
        {
            prefabToSpawn = coinPrefab;
        }
        else if (randomValue < 0.8f) // 40% chance for bat
        {
            prefabToSpawn = BatPrefab;
        }
        else // 20% chance for meteor
        {
            prefabToSpawn = MeteorPrefab;
        }

        // Check if the chosen position is unoccupied before spawning
        if (prefabToSpawn != null && !IsPositionOccupied(spawnPosition))
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        float checkRadius = 1f; // Radius for checking overlaps, adjust as needed
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius);
        foreach (Collider2D collider in colliders)
        {
            // Ensure it doesn't overlap with coins, bats, or meteors
            if (collider.CompareTag("Coin") || collider.CompareTag("Bat") || collider.CompareTag("Meteor"))
            {
                return true;
            }
        }
        return false;
    }
}


