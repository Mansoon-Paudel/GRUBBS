using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private float attackCooldown;
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject[] fireballs;
        [SerializeField] private AudioClip attackSound;

        private static readonly int AttackHash = Animator.StringToHash("attack");

        private Animator _anim;
        private PlayerMovement _playerMovement;
                private SpriteRenderer _spriteRenderer;
        private float _cooldownTimer = Mathf.Infinity;
                            private Vector3 _firePointLocalPos;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _playerMovement = GetComponent<PlayerMovement>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (firePoint != null)
                _firePointLocalPos = firePoint.localPosition;
        }

        private void Update()
        {
            if (Mouse.current != null && Mouse.current.leftButton.isPressed && _cooldownTimer > attackCooldown && _playerMovement.CanAttack())
                Attack();

            _cooldownTimer += Time.deltaTime;
        }

        private void Attack()
        {
            if (SoundManager.instance != null && attackSound != null)
                SoundManager.instance.PlaySound(attackSound);

            _anim.SetTrigger(AttackHash);
            _cooldownTimer = 0f;

            int idx = FindFireball();
            if (idx < 0 || idx >= fireballs.Length) return;

            GameObject fireball = fireballs[idx];
            if (fireball == null) return;

            // Compute spawn position so the fire point mirrors horizontally when player is flipped
            float dir = 1f;
            if (_spriteRenderer != null)
                dir = _spriteRenderer.flipX ? -1f : 1f;
            else
                dir = Mathf.Sign(transform.localScale.x);

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

            fireball.transform.position = spawnWorld;
            Projectile projectile = fireball.GetComponent<Projectile>();
            if (projectile != null)
                projectile.SetDirection(dir);
        }

        private int FindFireball()
        {
            for (int i = 0; i < fireballs.Length; i++)
            {
                if (!fireballs[i].activeInHierarchy)
                    return i;
            }
            return 0;
        }
    }
}
