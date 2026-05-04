using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform _currentCheckpoint;
    private Health _playerHealth;
    private UIManager _uiManager;

    private void Awake()
    {
        _playerHealth = GetComponent<Health>();
        _uiManager = FindObjectOfType<UIManager>();
    }

    /// <summary>Called by Health.cs when player dies</summary>
    public void OnPlayerDeath()
    {
        if (_uiManager != null)
            _uiManager.GameOver();
        else
            Debug.LogWarning("PlayerRespawn: UIManager not found. Game over screen cannot be displayed.");
    }

    /// <summary>Called by checkpoint or respawn button to respawn player</summary>
    public void RespawnCheck()
    {
        if (_currentCheckpoint == null)
        {
            Debug.LogWarning("PlayerRespawn: no checkpoint available.");
            return;
        }

        if (_playerHealth == null)
        {
            Debug.LogWarning("PlayerRespawn: missing Health component.");
            return;
        }

        _playerHealth.Respawn();
        transform.position = _currentCheckpoint.position;

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        CameraController cameraController = mainCamera.GetComponent<CameraController>();
        if (cameraController == null || _currentCheckpoint.parent == null) return;

        cameraController.MoveToNewRoom(_currentCheckpoint.parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Checkpoint")) return;

        _currentCheckpoint = collision.transform;

        if (SoundManager.instance != null && checkpointSound != null)
            SoundManager.instance.PlaySound(checkpointSound);

        Collider2D checkpointCollider = collision.GetComponent<Collider2D>();
        if (checkpointCollider != null)
            checkpointCollider.enabled = false;

        Animator checkpointAnimator = collision.GetComponent<Animator>();
        if (checkpointAnimator != null)
            checkpointAnimator.SetTrigger("activate");
    }
}