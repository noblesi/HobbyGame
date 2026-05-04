using TopDownRoguelite.Enemy;
using UnityEngine;

namespace TopDownRoguelite.Weapon
{
    public class AutoWeapon : MonoBehaviour
    {
        [Header("Projectile")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform firePoint;

        [Header("Attack")]
        [Min(0.1f)]
        [SerializeField] private float fireInterval = 0.75f;
        [Min(0.1f)]
        [SerializeField] private float targetRange = 8f;
        [Min(0f)]
        [SerializeField] private float damage = 10f;
        [Min(0.1f)]
        [SerializeField] private float projectileSpeed = 8f;

        private float fireTimer;
        private bool hasLoggedMissingProjectileWarning;

        private void Update()
        {
            if (projectilePrefab == null)
            {
                LogMissingProjectileWarning();
                return;
            }

            fireTimer += Time.deltaTime;
            if (fireTimer < fireInterval)
            {
                return;
            }

            EnemyHealth target = FindNearestEnemy();
            if (target == null)
            {
                return;
            }

            fireTimer = 0f;
            FireAt(target.transform);
        }

        private void FireAt(Transform target)
        {
            Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
            Vector2 direction = ((Vector2)target.position - (Vector2)spawnPosition).normalized;

            if (direction == Vector2.zero)
            {
                direction = Vector2.up;
            }

            Projectile projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            projectile.Initialize(direction, damage, projectileSpeed);
        }

        private EnemyHealth FindNearestEnemy()
        {
            EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
            EnemyHealth nearestEnemy = null;
            float nearestSqrDistance = targetRange * targetRange;

            for (int i = 0; i < enemies.Length; i++)
            {
                EnemyHealth enemy = enemies[i];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                float sqrDistance = (enemy.transform.position - transform.position).sqrMagnitude;
                if (sqrDistance > nearestSqrDistance)
                {
                    continue;
                }

                nearestEnemy = enemy;
                nearestSqrDistance = sqrDistance;
            }

            return nearestEnemy;
        }

        private void LogMissingProjectileWarning()
        {
            if (hasLoggedMissingProjectileWarning)
            {
                return;
            }

            Debug.LogWarning("AutoWeapon projectile prefab is not assigned.", this);
            hasLoggedMissingProjectileWarning = true;
        }
    }
}
