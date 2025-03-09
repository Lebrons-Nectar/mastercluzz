using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Dodaj to!

public class HiveManager : MonoBehaviour
{
    public GameObject hivePrefab; // Prefab ula
    private List<GameObject> placedHives = new List<GameObject>(); // Lista postawionych uli

    public void PlaceHive(Vector3 position)
    {
        GameObject newHive = Instantiate(hivePrefab, position, Quaternion.identity);
        placedHives.Add(newHive);
    }
}