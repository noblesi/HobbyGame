using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownRoguelite.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform target;
        [Min(0.1f)]
        [SerializeField] private float spawnInterval = 2f;
        [Min(0.1f)]
        [SerializeField] private float spawnDistance = 8f;
        [Min(1)]
        [SerializeField] private int maxAliveEnemies = 30;
        [SerializeField] private bool spawnOnStart = true;

        private readonly List<GameObject> spawnedEnemies = new List<GameObject>();
        private float spawnTimer;
        private bool hasLoggedMissingPrefabWarning;

        public bool IsSpawning { get; private set; }

        public event Action<EnemyHealth> EnemySpawned;

        private void Start()
        {
            IsSpawning = spawnOnStart;
            TryAssignPlayerTarget();
        }

        private void Update()
        {
            if (!IsSpawning)
            {
                return;
            }

            if (enemyPrefab == null)
            {
                LogMissingPrefabWarning();
                return;
            }

            RemoveMissingEnemies();

            if (spawnedEnemies.Count >= maxAliveEnemies)
            {
                return;
            }

            spawnTimer += Time.deltaTime;
            if (spawnTimer < Mathf.Max(0.1f, spawnInterval))
            {
                return;
            }

            spawnTimer = 0f;
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            Vector2 spawnCenter = target != null ? target.position : transform.position;
            Vector2 spawnDirection = UnityEngine.Random.insideUnitCircle.normalized;

            if (spawnDirection == Vector2.zero)
            {
                spawnDirection = Vector2.up;
            }

            Vector2 spawnPosition = spawnCenter + spawnDirection * spawnDistance;
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(enemy);

            if (enemy.TryGetComponent(out EnemyMovement enemyMovement) && target != null)
            {
                enemyMovement.SetTarget(target);
            }

            if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
            {
                EnemySpawned?.Invoke(enemyHealth);
            }
        }

        public void SetSpawning(bool isSpawning)
        {
            IsSpawning = isSpawning;

            if (isSpawning)
            {
                spawnTimer = 0f;
            }
        }

        private void TryAssignPlayerTarget()
        {
            if (target != null)
            {
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        private void RemoveMissingEnemies()
        {
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if (spawnedEnemies[i] == null)
                {
                    spawnedEnemies.RemoveAt(i);
                }
            }
        }

        private void LogMissingPrefabWarning()
        {
            if (hasLoggedMissingPrefabWarning)
            {
                return;
            }

            Debug.LogWarning("EnemySpawner enemy prefab is not assigned.", this);
            hasLoggedMissingPrefabWarning = true;
        }
    }
}
