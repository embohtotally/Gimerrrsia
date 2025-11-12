using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] bool isPaused = false;

    void PauseToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }
        }
    }

    void PauseMenu()
    {
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void ResumeButton()
    {
        isPaused = false;
    }

    public void MainMenuButton()
    {
        isPaused = false;
        //Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        PauseToggle();
        PauseMenu();
    }
}
