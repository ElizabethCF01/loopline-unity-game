using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("UI Containers")]
    [SerializeField] private GameObject pauseMenuUI;
    
    [SerializeField] private AudioSource envAudioSource;
    
    private bool isPaused = false;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        envAudioSource.Play();
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
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuUI.SetActive(true);
        envAudioSource.Pause();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        envAudioSource.UnPause();
        pauseMenuUI.SetActive(false);
    }

    public void RestartGame()
    {
        var currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}