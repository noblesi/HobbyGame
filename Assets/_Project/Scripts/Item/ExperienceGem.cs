using TopDownRoguelite.Player;
using UnityEngine;

namespace TopDownRoguelite.Item
{
    [RequireComponent(typeof(Collider2D))]
    public class ExperienceGem : MonoBehaviour
    {
        [Header("Experience")]
        [Min(1)]
        [SerializeField] private int experienceValue = 1;

        private void Awake()
        {
            Collider2D gemCollider = GetComponent<Collider2D>();
            gemCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerExperience playerExperience = other.GetComponentInParent<PlayerExperience>();
            if (playerExperience == null)
            {
                return;
            }

            playerExperience.AddExperience(experienceValue);
            Destroy(gameObject);
        }

        public void SetExperienceValue(int newValue)
        {
            experienceValue = Mathf.Max(1, newValue);
        }
    }
}
