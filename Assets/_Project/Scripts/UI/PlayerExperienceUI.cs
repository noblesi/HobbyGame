using TopDownRoguelite.Player;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownRoguelite.UI
{
    public class PlayerExperienceUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private Text levelText;
        [SerializeField] private Slider experienceSlider;
        [SerializeField] private Text experienceText;

        private void Start()
        {
            TryAssignPlayerExperience();

            if (playerExperience == null)
            {
                Debug.LogWarning("PlayerExperienceUI player experience is not assigned.", this);
                return;
            }

            playerExperience.ExperienceChanged += Refresh;
            Refresh(playerExperience.Level, playerExperience.CurrentExperience, playerExperience.ExperienceToNextLevel);
        }

        private void OnDestroy()
        {
            if (playerExperience != null)
            {
                playerExperience.ExperienceChanged -= Refresh;
            }
        }

        private void Refresh(int level, int currentExperience, int experienceToNextLevel)
        {
            if (levelText != null)
            {
                levelText.text = $"Lv. {level}";
            }

            if (experienceSlider != null)
            {
                experienceSlider.minValue = 0f;
                experienceSlider.maxValue = experienceToNextLevel;
                experienceSlider.value = currentExperience;
            }

            if (experienceText != null)
            {
                experienceText.text = $"{currentExperience} / {experienceToNextLevel}";
            }
        }

        private void TryAssignPlayerExperience()
        {
            if (playerExperience != null)
            {
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerExperience = player.GetComponent<PlayerExperience>();
            }
        }
    }
}
