using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Game Over")] [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")] [SerializeField] private GameObject pauseScreen;

    private void Awake()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }
    #region Game Over
    //Activate game over screen
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    //Restart level - respawn at checkpoint or reload scene
    public void Restart()
    {
        PlayerRespawn playerRespawn = Object.FindAnyObjectByType<PlayerRespawn>();
        if (playerRespawn != null)
        {
            gameOverScreen.SetActive(false);
            playerRespawn.RespawnCheck();
        }
        else
        {
            // Fallback: reload scene if no checkpoint found
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //Main Menu
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //Quit game/exit play mode if in Editor
    public void Quit()
    {
        Application.Quit(); //Quits the game (only works in build)

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //Exits play mode (will only be executed in the editor)
#endif
    }

    #endregion;
}