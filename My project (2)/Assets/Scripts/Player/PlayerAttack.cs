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
        private float _cooldownTimer = Mathf.Infinity;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _playerMovement = GetComponent<PlayerMovement>();
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

            fireball.transform.position = firePoint.position;
            Projectile projectile = fireball.GetComponent<Projectile>();
            if (projectile != null)
                projectile.SetDirection(Mathf.Sign(transform.localScale.x));
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
