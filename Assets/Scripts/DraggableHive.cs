using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableHive : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    // Reference to the actual Hive prefab that will be placed in the scene
    public GameObject hivePrefab; // Reference to the actual Hive object that will be placed

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.position;
        canvasGroup.alpha = 0.6f; // Make the image slightly transparent while dragging
        canvasGroup.blocksRaycasts = false; // Prevent raycasting while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position; // Move the object with the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Cast a ray from the camera to where the user has dropped the hive in the scene
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))  // Check if ray hits anything in the scene
        {
            // Place the hive in the world at the point where the ray hit
            PlaceHive(hit.point);
        }
        else
        {
            // If the ray doesn't hit anything, return the hive to its original position
            rectTransform.position = originalPosition;
        }
    }

    // Method to instantiate the hive prefab at the drop location
    private void PlaceHive(Vector3 worldPosition)
    {
        // Instantiate the actual hive object in the scene at the drop location
        Instantiate(hivePrefab, worldPosition, Quaternion.identity);
    }
}
