using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBarbedWireSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject barbedWirePrefab;
    public float spawnDepth = 1f;

    private Vector3 originalPosition;
    private GameObject currentSpawnedWire;
    private PlayerValues PlayerVal;

    void Awake()
    {
        originalPosition = transform.position;

        GameObject logicObj = GameObject.FindGameObjectWithTag("Logic");
        if (logicObj != null)
        {
            PlayerVal = logicObj.GetComponent<PlayerValues>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (barbedWirePrefab == null || PlayerVal == null)
            return;

        if (PlayerVal.BarbedWireCount > 0)
        {
            PlayerVal.BarbedWireCount--;

            Vector3 spawnPos = GetWorldPosition(eventData.position);
            currentSpawnedWire = Instantiate(barbedWirePrefab, spawnPos, Quaternion.identity);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSpawnedWire != null)
        {
            Vector3 worldPos = GetWorldPosition(eventData.position);
            currentSpawnedWire.transform.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = originalPosition;
        currentSpawnedWire = null;
    }

    private Vector3 GetWorldPosition(Vector2 screenPos)
    {
        Vector3 screenPoint = new Vector3(screenPos.x, screenPos.y, spawnDepth);
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
