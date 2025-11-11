using UnityEngine;

public class PauseManager : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;
    
    void Start()
    {
        if(!pauseMenu)
        {
            Debug.LogError("pauseMenu is not attached, git gud");
            return;
        }
        ResumeGame();
    }
    
    public void OnClick()
    {
        if (!isPaused) PauseGame();
    }
    void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void ExitGame()
    {
        SceneLoader.Load("MAINMENUNEW", useBlackFade: false);
    }
}
