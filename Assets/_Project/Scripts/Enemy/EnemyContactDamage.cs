using TopDownRoguelite.Player;
using UnityEngine;

namespace TopDownRoguelite.Enemy
{
    public class EnemyContactDamage : MonoBehaviour
    {
        [Header("Damage")]
        [Min(0f)]
        [SerializeField] private float damage = 10f;
        [Min(0.1f)]
        [SerializeField] private float damageInterval = 1f;

        private float nextDamageTime;

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryDamagePlayer(collision.collider);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamagePlayer(other);
        }

        private void TryDamagePlayer(Collider2D other)
        {
            if (Time.time < nextDamageTime)
            {
                return;
            }

            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null)
            {
                return;
            }

            playerHealth.TakeDamage(damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }
}
