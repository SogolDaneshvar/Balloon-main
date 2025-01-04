using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public float topMargin = 0.1f; // Space between balloon and top of screen
    public float bottomMargin = 0.1f; // Space between balloon and bottom of screen
    public float leftMargin = 0.1f; // Space between balloon and left side of screen
    public float rightMargin = 0.1f; // Space between balloon and right side of screen

    private float minX, maxX, minY, maxY;
    private bool isGameOver = false;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Calculate the screen boundaries based on camera and margins
        CalculateBoundaries();
    }

    void Update()
    {

        if (!isGameOver)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
            transform.position = clampedPosition;
        }
    }

    void CalculateBoundaries()
    {
        // Get the screen bounds in world units
        float screenAspect = mainCamera.aspect;
        float camHeight = mainCamera.orthographicSize;

        // Define screen edges with separate left and right margins, along with top and bottom margins
        minX = mainCamera.transform.position.x - camHeight * screenAspect + leftMargin;
        maxX = mainCamera.transform.position.x + camHeight * screenAspect - rightMargin;
        minY = mainCamera.transform.position.y - camHeight + bottomMargin;
        maxY = mainCamera.transform.position.y + camHeight - topMargin;
    }

    public void DeactivateBoundary()
    {
        isGameOver = true;
    }

    void OnDrawGizmos()
    {
        // Visualize the boundaries for easier adjustments (in the editor)
        if (mainCamera == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
        Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));
        Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
        Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
    }
}

