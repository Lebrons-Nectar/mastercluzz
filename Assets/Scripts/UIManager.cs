using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject deathScreenPanel;
    public GameObject hiveUIPanel;
    public GameObject turretUIPanel;
    public GameObject barbedWireUIPanel; // ✅ Added
    public GameObject shopUIPanel;

    public static bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f;

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (deathScreenPanel != null) deathScreenPanel.SetActive(false);
        if (hiveUIPanel != null) hiveUIPanel.SetActive(false);
        if (turretUIPanel != null) turretUIPanel.SetActive(false);
        if (barbedWireUIPanel != null) barbedWireUIPanel.SetActive(false); // ✅ Added
        if (shopUIPanel != null) shopUIPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isPaused)
                CloseShop();
            else
                OpenShop();
        }
    }

    public void OpenShop()
    {
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void CloseShop()
    {
        if (shopUIPanel != null)
        {
            shopUIPanel.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public void ToggleHiveUI()
    {
        if (hiveUIPanel != null)
            hiveUIPanel.SetActive(!hiveUIPanel.activeSelf);
    }

    public void ToggleTurretUI()
    {
        if (turretUIPanel != null)
            turretUIPanel.SetActive(!turretUIPanel.activeSelf);
    }

    public void ToggleBarbedWireUI() // ✅ Added
    {
        if (barbedWireUIPanel != null)
            barbedWireUIPanel.SetActive(!barbedWireUIPanel.activeSelf);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        isPaused = false;
    }

    public void ShowDeathScreen()
    {
        Time.timeScale = 1f;
        if (deathScreenPanel != null) deathScreenPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
