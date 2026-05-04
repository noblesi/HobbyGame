using System;
using UnityEngine;

namespace TopDownRoguelite.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Health")]
        [Min(1f)]
        [SerializeField] private float maxHealth = 20f;
        [SerializeField] private bool destroyOnDeath = true;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;
        public bool IsDead { get; private set; }

        public event Action<EnemyHealth> Died;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead || damage <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);

            if (CurrentHealth <= 0f)
            {
                Die();
            }
        }

        private void Die()
        {
            IsDead = true;
            Died?.Invoke(this);

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
