using TopDownRoguelite.Player;
using UnityEngine;

namespace TopDownRoguelite.Enemy
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyContactDamage : MonoBehaviour
    {
        [Header("Damage")]
        [Min(0f)]
        [SerializeField] private float damage = 10f;
        [Min(0.1f)]
        [SerializeField] private float damageInterval = 1f;
        [SerializeField] private bool useTriggerCollider = true;

        private float nextDamageTime;

        private void Awake()
        {
            if (!useTriggerCollider)
            {
                return;
            }

            Collider2D[] colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].isTrigger = true;
            }
        }

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
