using UnityEngine;

namespace TopDownRoguelite.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Follow Settings")]
        [SerializeField] private bool snapToTargetOnStart = true;
        [SerializeField] private bool useSmoothFollow = true;
        [Min(0f)]
        [SerializeField] private float smoothTime = 0.15f;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        private Vector3 velocity;
        private bool hasLoggedMissingTargetWarning;

        private void Start()
        {
            if (snapToTargetOnStart && target != null)
            {
                transform.position = GetFollowPosition();
            }
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                if (!hasLoggedMissingTargetWarning)
                {
                    Debug.LogWarning("CameraFollow target is not assigned.", this);
                    hasLoggedMissingTargetWarning = true;
                }

                return;
            }

            Vector3 targetPosition = GetFollowPosition();

            if (!useSmoothFollow || smoothTime <= 0f)
            {
                transform.position = targetPosition;
                return;
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            velocity = Vector3.zero;
            hasLoggedMissingTargetWarning = false;
        }

        private Vector3 GetFollowPosition()
        {
            Vector3 targetPosition = target.position;
            targetPosition.z = 0f;
            return targetPosition + offset;
        }
    }
}
