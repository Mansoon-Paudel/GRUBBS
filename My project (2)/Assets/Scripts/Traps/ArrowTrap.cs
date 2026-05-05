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
    private SpriteRenderer _spriteRend;
    private Vector3 _firePointLocalPos;

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        _spriteRend = GetComponent<SpriteRenderer>();
        if (firePoint != null)
            _firePointLocalPos = firePoint.localPosition;
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
        GameObject arrow = arrows[idx];
        if (arrow == null) return;

        float dir = (_spriteRend != null && _spriteRend.flipX) ? -1f : 1f;
        Vector3 spawnWorld;
        if (firePoint != null)
        {
            Vector3 local = _firePointLocalPos;
            local.x = Mathf.Abs(local.x) * (dir < 0f ? -1f : 1f);
            Transform parent = firePoint.parent != null ? firePoint.parent : transform;
            spawnWorld = parent.TransformPoint(local);
        }
        else
        {
            spawnWorld = transform.position + new Vector3(1f * dir, 0f, 0f);
        }

        arrow.transform.position = spawnWorld;
        EnemyProjectile ep = arrow.GetComponent<EnemyProjectile>();
        if (ep != null)
        {
            ep.ActivateProjectile();
            ep.SetDirection(dir);
        }
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