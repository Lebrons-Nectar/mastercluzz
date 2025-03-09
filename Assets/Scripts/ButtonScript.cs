using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    public UIManager uiManager;
    public Button openHiveUIButton;

    void Start()
    {
        // Make sure the button is assigned and call ToggleHiveUI() when clicked
        openHiveUIButton.onClick.AddListener(uiManager.ToggleHiveUI);
    }
}
