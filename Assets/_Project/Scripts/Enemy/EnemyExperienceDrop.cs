using TopDownRoguelite.Item;
using UnityEngine;

namespace TopDownRoguelite.Enemy
{
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyExperienceDrop : MonoBehaviour
    {
        [Header("Drop")]
        [SerializeField] private ExperienceGem experienceGemPrefab;
        [Min(1)]
        [SerializeField] private int experienceValue = 1;
        [Min(0f)]
        [SerializeField] private float randomDropRadius = 0.2f;

        private EnemyHealth enemyHealth;
        private bool hasLoggedMissingGemWarning;

        private void Awake()
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            if (enemyHealth != null)
            {
                enemyHealth.Died += DropExperienceGem;
            }
        }

        private void OnDisable()
        {
            if (enemyHealth != null)
            {
                enemyHealth.Died -= DropExperienceGem;
            }
        }

        private void DropExperienceGem(EnemyHealth deadEnemy)
        {
            if (experienceGemPrefab == null)
            {
                LogMissingGemWarning();
                return;
            }

            Vector2 randomOffset = Random.insideUnitCircle * randomDropRadius;
            Vector3 dropPosition = deadEnemy.transform.position + (Vector3)randomOffset;
            ExperienceGem gem = Instantiate(experienceGemPrefab, dropPosition, Quaternion.identity);
            gem.SetExperienceValue(experienceValue);
        }

        private void LogMissingGemWarning()
        {
            if (hasLoggedMissingGemWarning)
            {
                return;
            }

            Debug.LogWarning("EnemyExperienceDrop experience gem prefab is not assigned.", this);
            hasLoggedMissingGemWarning = true;
        }
    }
}
