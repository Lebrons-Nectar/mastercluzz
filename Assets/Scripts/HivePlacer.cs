using UnityEngine;

public class HivePlacer : MonoBehaviour
{
    public GameObject hivePrefab; // Reference to the hive prefab
    private GameObject currentHive; // Current hive instance

    private bool isPlacing = false;

    void Update()
    {
        if (isPlacing && currentHive != null)
        {
            // Move the Hive object to the mouse position
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 9f; // Set the distance from the camera
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos); // Convert to world position
            currentHive.transform.position = worldPos;

            // Place the Hive when the mouse button is pressed
            if (Input.GetMouseButtonDown(0))
            {
                // Here you could add validation or specific conditions to confirm the placement
                isPlacing = false;
                currentHive = null; // Clear the reference after placement
            }
        }
    }

    public void StartPlacingHive()
    {
        // Start the placement process and instantiate a Hive object in the scene
        currentHive = Instantiate(hivePrefab);
        isPlacing = true;
    }
}
