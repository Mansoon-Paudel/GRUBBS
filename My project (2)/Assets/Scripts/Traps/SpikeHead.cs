using Enemy;
using UnityEngine;

public class Spikehead : EnemyDamage
{
    [Header("SpikeHead Attributes")]
    [SerializeField] private float speed;
    [SerializeField] private float range;
    [SerializeField] private float checkDelay;
    [SerializeField] private LayerMask playerLayer;

    [Header("Bounds")]
    [SerializeField] private float boundRange; 

    private Vector3[] directions = new Vector3[4];
    private Vector3 destination;
    private Vector3 spawnPosition; 
    private float checkTimer;
    private bool attacking;

    [Header("SFX")]
    [SerializeField] private AudioClip sound;
    private void Start()
    {
        spawnPosition = transform.position; 
    }

    private void OnEnable()
    {
        Stop();
    }

    private void Update()
    {
        if (attacking)
        {
        
        

            transform.Translate(destination * Time.deltaTime * speed);

          
            if (Vector3.Distance(transform.position, spawnPosition) >= boundRange)
            {
                transform.position = spawnPosition;
                Stop();
            }
        }
        else
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > checkDelay)
                CheckForPlayer();
        }
    }

    private void CheckForPlayer()
    {
        CalculateDirections();
        for (int i = 0; i < directions.Length; i++)
        {
            Debug.DrawRay(transform.position, directions[i], Color.red);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[i], range, playerLayer);

            if (hit.collider != null && !attacking)
            {
                attacking = true;
                destination = directions[i];
                checkTimer = 0;
            }
        }
    }

    private void CalculateDirections()
    {
        directions[0] = transform.right * range;
        directions[1] = -transform.right * range;
        directions[2] = transform.up * range;
        directions[3] = -transform.up * range;
    }

    private void Stop()
    {
        destination = transform.position;
        attacking = false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        SoundManager.instance.PlaySound(sound);
        Stop();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = Application.isPlaying ? spawnPosition : transform.position;
        Gizmos.DrawWireSphere(origin, boundRange);
    }
}