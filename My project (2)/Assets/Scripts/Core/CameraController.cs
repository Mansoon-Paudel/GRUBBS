using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Room camera
    [SerializeField] private float speed;
    private float currentPosX;
    private float currentPosY;
    private float currentPosZ;
    private Vector3 velocity = Vector3.zero;
    // Player follower camera
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float lookAhead;

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentPosX, currentPosY, currentPosZ), ref velocity, speed);

        //Follow player camera
        //transform.position = new Vector3(player.position.x + lookAhead, transform.position.y, transform.position.z);
        //lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }

    public void MoveToNewRoom(Transform _newRoom)
    {
        if (_newRoom == null) return;

        currentPosX = _newRoom.position.x;
        currentPosY = _newRoom.position.y ;
        currentPosZ = transform.position.z;
    }

    private void Awake()
    {
        currentPosX = transform.position.x;
        currentPosY = transform.position.y;
        currentPosZ = transform.position.z;
    } 
}