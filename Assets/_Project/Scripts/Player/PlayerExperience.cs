using System;
using TopDownRoguelite.Core;
using UnityEngine;

namespace TopDownRoguelite.Player
{
    public class PlayerExperience : MonoBehaviour
    {
        [Header("Experience")]
        [Min(1)]
        [SerializeField] private int baseExperienceToNextLevel = 5;
        [Min(0)]
        [SerializeField] private int experienceIncreasePerLevel = 5;

        public int Level { get; private set; } = 1;
        public int CurrentExperience { get; private set; }
        public int ExperienceToNextLevel { get; private set; }

        public event Action<int> LeveledUp;
        public event Action<int, int, int> ExperienceChanged;

        private void Awake()
        {
            ExperienceToNextLevel = CalculateExperienceToNextLevel();
            NotifyExperienceChanged();
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            CurrentExperience += amount;

            while (CurrentExperience >= ExperienceToNextLevel)
            {
                CurrentExperience -= ExperienceToNextLevel;
                Level++;
                ExperienceToNextLevel = CalculateExperienceToNextLevel();
                GameAudioPlayer.PlayLevelUp();
                LeveledUp?.Invoke(Level);
            }

            NotifyExperienceChanged();
        }

        private int CalculateExperienceToNextLevel()
        {
            return baseExperienceToNextLevel + experienceIncreasePerLevel * (Level - 1);
        }

        private void NotifyExperienceChanged()
        {
            ExperienceChanged?.Invoke(Level, CurrentExperience, ExperienceToNextLevel);
        }
    }
}
