using UnityEngine;

namespace TopDownRoguelite.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [Min(1f)]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool disableOnDeath = true;

        public float CurrentHealth { get; private set; }
        public float MaxHealth => maxHealth;
        public bool IsDead { get; private set; }

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

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f)
            {
                return;
            }

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        }

        private void Die()
        {
            IsDead = true;
            Debug.Log("Player died.", this);

            if (disableOnDeath)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
