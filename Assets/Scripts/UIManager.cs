using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject deathScreenPanel;
    public GameObject hiveUIPanel;
    public GameObject turretUIPanel;
    public GameObject shopUIPanel;

    public static bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f; // Unpause game at start

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (deathScreenPanel != null) deathScreenPanel.SetActive(false);
        if (hiveUIPanel != null) hiveUIPanel.SetActive(false);
        if (hiveUIPanel != null) hiveUIPanel.SetActive(false);
        if (turretUIPanel != null) turretUIPanel.SetActive(false);
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
        shopUIPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void CloseShop()
    {
        shopUIPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ToggleHiveUI()
    {
        if (hiveUIPanel != null)
            hiveUIPanel.SetActive(!hiveUIPanel.activeSelf);
    }

        public void ToggleturretUI()
    {
        if (turretUIPanel != null)
            turretUIPanel.SetActive(!turretUIPanel.activeSelf);
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
        Debug.Log("🔄 Restarting Scene...");
        Time.timeScale = 1f; // UNPAUSE before reloading
       SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }

    public void ReturnToMainMenu()
    {
    Time.timeScale = 1f; // Unpause in case it was paused
    SceneManager.LoadScene("MainMenu"); // your main menu scene name
    }

}
