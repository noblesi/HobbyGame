using UnityEngine;

namespace TopDownRoguelite.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Movement")]
        [Min(0f)]
        [SerializeField] private float moveSpeed = 2.5f;

        private Rigidbody2D rb;
        private bool hasLoggedMissingTargetWarning;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }

        private void Start()
        {
            TryAssignPlayerTarget();
        }

        private void FixedUpdate()
        {
            if (target == null)
            {
                LogMissingTargetWarning();
                return;
            }

            Vector2 direction = ((Vector2)target.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            hasLoggedMissingTargetWarning = false;
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
                SetTarget(player.transform);
            }
        }

        private void LogMissingTargetWarning()
        {
            if (hasLoggedMissingTargetWarning)
            {
                return;
            }

            Debug.LogWarning("EnemyMovement target is not assigned.", this);
            hasLoggedMissingTargetWarning = true;
        }
    }
}
