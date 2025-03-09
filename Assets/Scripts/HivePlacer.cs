using UnityEngine;

public class HivePlacer : MonoBehaviour
{
    public HiveManager hiveManager;
    public LayerMask groundLayer; // Ground layer for the raycast (we'll remove this temporarily for debugging)
    private bool isPlacing = false;

    void Update()
    {
        if (isPlacing)
        {
            Debug.Log("Placing mode active! Waiting for mouse click...");

            if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
            {
                Debug.Log("Mouse clicked!");

                // Cast a ray from the camera to the mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Debug the ray to visualize it in the Scene view
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f); // Red line in scene view

                // Temporary change: Remove LayerMask for debugging (no filtering)
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) // No LayerMask
                {
                    Debug.Log("Raycast hit at: " + hit.point); // Log hit point
                    hiveManager.PlaceHive(hit.point);
                    isPlacing = false; // Disable placing mode
                }
                else
                {
                    Debug.LogError("Raycast did not hit anything! Ensure the ground has a collider and is not a trigger.");
                }
            }
        }
    }

    public void EnablePlacing()
    {
        isPlacing = true;
        Debug.Log("Placing mode enabled!");
    }
}
