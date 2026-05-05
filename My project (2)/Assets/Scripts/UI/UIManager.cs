using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Game Over")] [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")] [SerializeField] private GameObject pauseScreen;

    private InputAction _pauseAction;

    private void Awake()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (pauseScreen != null) pauseScreen.SetActive(false);
    }

    private void OnEnable()
    {
        _pauseAction = new InputAction(type: InputActionType.Button);
        _pauseAction.AddBinding("<Keyboard>/escape");
        _pauseAction.AddBinding("<Gamepad>/start");
        _pauseAction.performed += OnPausePerformed;
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        if (_pauseAction != null)
        {
            _pauseAction.performed -= OnPausePerformed;
            _pauseAction.Disable();
            _pauseAction.Dispose();
            _pauseAction = null;
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        // Toggle pause screen
        if (pauseScreen == null) return;
        PauseGame(!pauseScreen.activeInHierarchy);
    }

    public void GameOver()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
        if (SoundManager.instance != null)
            SoundManager.instance.PlaySound(gameOverSound);
    }

    public void Restart()
    {
        PlayerRespawn playerRespawn = Object.FindAnyObjectByType<PlayerRespawn>();
        if (playerRespawn != null)
        {
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
            playerRespawn.RespawnCheck();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PauseGame(bool status)
    {
         pauseScreen.SetActive(status);

         if (status)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

}  




