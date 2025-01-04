using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public float speed = 2f;

    private void Update()
    {
        // Move the bat downward
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // Destroy the bat if it moves off-screen
        if (transform.position.y < -10f) // Adjust the threshold as needed
        {
            Destroy(gameObject);
        }
    }
}
