using UnityEngine;

namespace Enemy
{
    public class EnemyDamage : MonoBehaviour
    {
        [SerializeField] private float damage;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Health health = collision.GetComponentInParent<Health>();
                if (health != null)
                    health.TakeDamage(damage);
            }
        }
    }
}
