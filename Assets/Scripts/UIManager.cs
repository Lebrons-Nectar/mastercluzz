using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject hiveUIPanel;  // The panel that contains the draggable hive image

    public void ToggleHiveUI()
    {
        hiveUIPanel.SetActive(!hiveUIPanel.activeSelf);
    }
}
