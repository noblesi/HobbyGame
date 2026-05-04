using TopDownRoguelite.Enemy;
using UnityEngine;

namespace TopDownRoguelite.Weapon
{
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile")]
        [SerializeField] private float defaultSpeed = 8f;
        [SerializeField] private float defaultDamage = 10f;
        [Min(0.1f)]
        [SerializeField] private float lifetime = 3f;

        private Vector2 direction = Vector2.up;
        private float speed;
        private float damage;
        private float lifeTimer;

        private void Awake()
        {
            speed = defaultSpeed;
            damage = defaultDamage;

            Collider2D projectileCollider = GetComponent<Collider2D>();
            projectileCollider.isTrigger = true;
        }

        private void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);

            lifeTimer += Time.deltaTime;
            if (lifeTimer >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth == null)
            {
                return;
            }

            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
        }

        public void Initialize(Vector2 newDirection, float newDamage, float newSpeed)
        {
            direction = newDirection.sqrMagnitude > 0f ? newDirection.normalized : Vector2.up;
            damage = newDamage;
            speed = newSpeed;
            lifeTimer = 0f;
        }
    }
}
