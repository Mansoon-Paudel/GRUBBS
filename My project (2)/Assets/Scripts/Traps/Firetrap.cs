using System.Collections;
using UnityEngine;

public class Firetrap : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float soundRange = 10f;

    [Header("Fire Trap Timers")]
    [SerializeField] private float animationDelay = 1f;
    [SerializeField] private float activateTime   = 2f;

    [Header("Sounds")]
    [SerializeField] private AudioClip Sound;

    private Animator       anim;
    private SpriteRenderer spriteRend;
    private Transform      player;
    private bool           active;

    private void Awake()
    {
        anim       = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Start()
    {
        StartCoroutine(TrapLoop());
    }

    private IEnumerator TrapLoop()
    {
        while (true)
        {
            spriteRend.color = Color.grey;
            yield return new WaitForSeconds(animationDelay);

            if (PlayerIsNearby())
                SoundManager.instance.PlaySound(Sound);

            spriteRend.color = Color.red;
            active = true;
            anim.SetBool("activated", true);
            yield return new WaitForSeconds(activateTime);

            active = false;
            anim.SetBool("activated", false);
            spriteRend.color = Color.black;
        }
    }

    private bool PlayerIsNearby()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= soundRange;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (active && collision.CompareTag("Player"))
            collision.GetComponent<Health>().TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }
}