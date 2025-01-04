using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FogFadingScript : MonoBehaviour
{
    private SpriteRenderer fogRenderer;
    public float fadeSpeed = 0.5f;  // Adjust fade speed to be slow and gradual
    private bool shouldFade = false;
    private float initialAlpha;  // Track the initial alpha value of the fog

    void Start()
    {
        fogRenderer = GetComponent<SpriteRenderer>();

        // Get the current alpha (transparency) of the fog's color
        initialAlpha = fogRenderer.color.a;
    }

    void Update()
    {
        // Check if we should start fading the fog
        if (shouldFade)
        {
            Color fogColor = fogRenderer.color;

            // Gradually decrease the alpha value based on fadeSpeed
            fogColor.a = Mathf.Lerp(fogColor.a, 0f, fadeSpeed * Time.deltaTime);

            // Apply the updated color to the fog sprite
            fogRenderer.color = fogColor;

            // If the fog has almost completely disappeared, destroy the object
            if (fogColor.a <= 0.01f)  // Adjust the threshold for full transparency
            {
                Destroy(gameObject);  // Remove fog object when fully transparent
            }
        }
    }

    // Trigger the fade-out when the balloon enters the fog's area
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Balloon"))  // Ensure the balloon has a "Balloon" tag
        {
            shouldFade = true;  // Start fading when the balloon enters the fog
        }
    }
}
