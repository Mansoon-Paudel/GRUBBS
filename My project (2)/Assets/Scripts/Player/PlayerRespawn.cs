using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform _currentCheckpoint;
    private Health _playerHealth;
    private UIManager _uiManager;
    private Vector3 _initialPosition;
    private Transform _initialRoom;

    private void Awake()
    {
        _playerHealth = GetComponent<Health>();
        _uiManager = FindObjectOfType<UIManager>();
        // remember initial spawn position so we can respawn even if no checkpoint was reached
        _initialPosition = transform.position;
        _initialRoom = transform.parent;
    }

    public void OnPlayerDeath()
    {
        if (_uiManager != null)
            _uiManager.GameOver();
    }

    public void RespawnCheck()
    {
        Transform target = _currentCheckpoint;
        if (target == null)
        {
        }

        if (_playerHealth == null)
        {
            return;
        }

        _playerHealth.AllowRespawnAndPerform();
        if (target != null)
            transform.position = target.position;
        else
            transform.position = _initialPosition;

        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        CameraController cameraController = mainCamera.GetComponent<CameraController>();
        if (cameraController == null) return;

        Transform room = (target != null && target.parent != null) ? target.parent : _initialRoom;
        if (room != null)
            cameraController.MoveToNewRoom(room);
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
    }
}