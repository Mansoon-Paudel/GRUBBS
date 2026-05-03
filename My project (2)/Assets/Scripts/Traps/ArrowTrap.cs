using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private AudioClip arrowSound;
    [SerializeField] private float soundRange = 10f;

    private float cooldownTimer;
    private Transform player;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= attackCooldown)
            Attack();
    }

    private void Attack()
    {
        cooldownTimer = 0;

        if (PlayerIsNearby())
            SoundManager.instance.PlaySound(arrowSound);

        int idx = FindArrow();
        arrows[idx].transform.position = firePoint.position;
        arrows[idx].GetComponent<EnemyProjectile>().ActivateProjectile();
    }

    private bool PlayerIsNearby()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) <= soundRange;
    }

    private int FindArrow()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            if (!arrows[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }
}