using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableHiveSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject hivePrefab;  // Drag your hive prefab here in the Inspector
    public float spawnDepth = 10f;   // Adjust based on your camera and scene setup

    private Vector3 originalPosition;  // To store the UI image's original screen position
    private GameObject currentSpawnedHive;  // Reference to the spawned hive in the scene

    void Awake()
    {
        originalPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (hivePrefab == null)
        {
            Debug.LogError("Hive prefab not assigned in DraggableHiveSpawner!");
            return;
        }

        // Convert the screen position to world position and spawn the hive object
        Vector3 spawnPos = GetWorldPosition(eventData.position);
        currentSpawnedHive = Instantiate(hivePrefab, spawnPos, Quaternion.identity);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSpawnedHive != null)
        {
            // Update the spawned hive's position to follow the mouse
            Vector3 worldPos = GetWorldPosition(eventData.position);
            currentSpawnedHive.transform.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset the UI image back to its original position in the UI
        transform.position = originalPosition;
        // The spawned hive remains in the scene; clear the reference
        currentSpawnedHive = null;
    }

    // Helper method to convert screen coordinates to world coordinates
    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, spawnDepth);
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
