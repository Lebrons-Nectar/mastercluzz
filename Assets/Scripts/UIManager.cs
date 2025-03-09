using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject hivePanel;  // Reference to the Hive UI Panel

    // Method to toggle the panel visibility
    public void ToggleHiveUI()
    {
        hivePanel.SetActive(!hivePanel.activeSelf);
    }
}
