using System;
using System.Collections.Generic;
using TopDownRoguelite.Player;
using TopDownRoguelite.Weapon;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownRoguelite.UI
{
    public class UpgradeSelectionUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private AutoWeapon autoWeapon;

        [Header("UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Text titleText;
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private Text[] optionTexts;

        private readonly List<UpgradeOption> availableOptions = new List<UpgradeOption>();
        private readonly List<UpgradeOption> currentOptions = new List<UpgradeOption>();
        private int pendingLevelUps;
        private float previousTimeScale = 1f;

        private void Start()
        {
            TryAssignPlayerReferences();
            BuildAvailableOptions();

            if (panel != null)
            {
                panel.SetActive(false);
            }

            if (playerExperience == null)
            {
                Debug.LogWarning("UpgradeSelectionUI player experience is not assigned.", this);
                return;
            }

            playerExperience.LeveledUp += QueueLevelUp;
        }

        private void OnDestroy()
        {
            if (playerExperience != null)
            {
                playerExperience.LeveledUp -= QueueLevelUp;
            }
        }

        private void QueueLevelUp(int level)
        {
            pendingLevelUps++;

            if (panel == null || panel.activeSelf)
            {
                return;
            }

            ShowOptions(level);
        }

        private void ShowOptions(int level)
        {
            if (availableOptions.Count == 0)
            {
                return;
            }

            if (optionButtons == null || optionButtons.Length == 0)
            {
                Debug.LogWarning("UpgradeSelectionUI option buttons are not assigned.", this);
                return;
            }

            bool wasPanelActive = panel != null && panel.activeSelf;
            if (!wasPanelActive)
            {
                previousTimeScale = Time.timeScale;
            }

            Time.timeScale = 0f;

            if (titleText != null)
            {
                titleText.text = $"Level {level} Upgrade";
            }

            currentOptions.Clear();
            FillCurrentOptions();

            for (int i = 0; i < optionButtons.Length; i++)
            {
                Button button = optionButtons[i];
                if (button == null)
                {
                    continue;
                }

                bool hasOption = i < currentOptions.Count;
                button.gameObject.SetActive(hasOption);
                button.onClick.RemoveAllListeners();

                if (!hasOption)
                {
                    continue;
                }

                UpgradeOption option = currentOptions[i];
                Text optionText = GetOptionText(i, button);
                if (optionText != null)
                {
                    optionText.text = option.DisplayText;
                }

                button.onClick.AddListener(() => SelectOption(option));
            }

            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        private void SelectOption(UpgradeOption option)
        {
            option.Apply?.Invoke();
            pendingLevelUps = Mathf.Max(0, pendingLevelUps - 1);

            if (pendingLevelUps > 0)
            {
                ShowOptions(playerExperience != null ? playerExperience.Level : 1);
                return;
            }

            if (panel != null)
            {
                panel.SetActive(false);
            }

            Time.timeScale = previousTimeScale;
        }

        private void FillCurrentOptions()
        {
            List<UpgradeOption> optionPool = new List<UpgradeOption>(availableOptions);
            int optionCount = Mathf.Min(Mathf.Min(3, optionButtons.Length), optionPool.Count);

            for (int i = 0; i < optionCount; i++)
            {
                int optionIndex = UnityEngine.Random.Range(0, optionPool.Count);
                currentOptions.Add(optionPool[optionIndex]);
                optionPool.RemoveAt(optionIndex);
            }
        }

        private Text GetOptionText(int index, Button button)
        {
            if (optionTexts != null && index < optionTexts.Length && optionTexts[index] != null)
            {
                return optionTexts[index];
            }

            return button.GetComponentInChildren<Text>();
        }

        private void BuildAvailableOptions()
        {
            availableOptions.Clear();

            if (autoWeapon != null)
            {
                availableOptions.Add(new UpgradeOption("Weapon Damage", "+5 projectile damage", () => autoWeapon.AddDamage(5f)));
                availableOptions.Add(new UpgradeOption("Fire Rate", "Attack 15% faster", () => autoWeapon.MultiplyFireInterval(0.85f)));
                availableOptions.Add(new UpgradeOption("Attack Range", "+1.5 targeting range", () => autoWeapon.AddTargetRange(1.5f)));
            }

            if (playerMovement != null)
            {
                availableOptions.Add(new UpgradeOption("Move Speed", "+0.5 movement speed", () => playerMovement.AddMoveSpeed(0.5f)));
            }

            if (playerHealth != null)
            {
                availableOptions.Add(new UpgradeOption("Max Health", "+20 max health", () => playerHealth.IncreaseMaxHealth(20f)));
            }
        }

        private void TryAssignPlayerReferences()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }

            if (playerExperience == null)
            {
                playerExperience = player.GetComponent<PlayerExperience>();
            }

            if (playerMovement == null)
            {
                playerMovement = player.GetComponent<PlayerMovement>();
            }

            if (playerHealth == null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }

            if (autoWeapon == null)
            {
                autoWeapon = player.GetComponent<AutoWeapon>();
            }
        }

        private class UpgradeOption
        {
            public UpgradeOption(string title, string description, Action apply)
            {
                Title = title;
                Description = description;
                Apply = apply;
            }

            public string Title { get; }
            public string Description { get; }
            public Action Apply { get; }
            public string DisplayText => $"{Title}\n{Description}";
        }
    }
}
