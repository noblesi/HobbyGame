using System.Collections.Generic;
using TopDownRoguelite.Dungeon;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TopDownRoguelite.Reward
{
    public class RewardSelectionController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StageRoomController stageRoomController;
        [SerializeField] private RunRewardInventory rewardInventory;

        [Header("Reward Options")]
        [Min(1)]
        [SerializeField] private int optionCount = 3;
        [Min(1)]
        [SerializeField] private int minGold = 25;
        [Min(1)]
        [SerializeField] private int maxGold = 60;

        [Header("Optional UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Text titleText;
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private Text[] optionTexts;

        [Header("Debug Input")]
        [SerializeField] private bool allowNumberKeySelection = true;
        [SerializeField] private bool logAvailableRewards = true;

        private readonly List<RewardOption> currentOptions = new List<RewardOption>();
        private bool isWaitingForReward;

        private void OnEnable()
        {
            TryAssignReferences();

            if (stageRoomController != null)
            {
                stageRoomController.RoomStarted += HandleRoomStarted;
                stageRoomController.StageCompleted += HandleStageCompleted;
            }
        }

        private void Start()
        {
            HideRewardPanel();
        }

        private void Update()
        {
            if (!isWaitingForReward || !allowNumberKeySelection)
            {
                return;
            }

            ReadNumberKeySelection();
        }

        private void OnDisable()
        {
            if (stageRoomController != null)
            {
                stageRoomController.RoomStarted -= HandleRoomStarted;
                stageRoomController.StageCompleted -= HandleStageCompleted;
            }
        }

        private void HandleRoomStarted(StageNode stageNode, RoomType roomType, int roomIndex)
        {
            if (roomType != RoomType.Reward)
            {
                if (!isWaitingForReward)
                {
                    HideRewardPanel();
                }

                return;
            }

            StartRewardSelection(stageNode);
        }

        private void HandleStageCompleted(StageNode stageNode)
        {
            isWaitingForReward = false;
            currentOptions.Clear();
            HideRewardPanel();
        }

        private void StartRewardSelection(StageNode stageNode)
        {
            isWaitingForReward = true;
            currentOptions.Clear();
            BuildRewardOptions(stageNode);
            ShowRewardPanel(stageNode);
            LogRewards(stageNode);
        }

        [ContextMenu("Select First Reward")]
        public void SelectFirstReward()
        {
            SelectRewardIndex(0);
        }

        public void SelectRewardIndex(int rewardIndex)
        {
            if (!isWaitingForReward)
            {
                Debug.Log("There is no pending reward selection.", this);
                return;
            }

            if (rewardIndex < 0 || rewardIndex >= currentOptions.Count)
            {
                Debug.LogWarning($"Reward index {rewardIndex} is not available.", this);
                return;
            }

            RewardOption selectedReward = currentOptions[rewardIndex];
            ApplyReward(selectedReward);

            isWaitingForReward = false;
            currentOptions.Clear();
            HideRewardPanel();

            if (stageRoomController != null)
            {
                stageRoomController.CompleteCurrentRoom();
            }
        }

        private void ApplyReward(RewardOption rewardOption)
        {
            if (rewardInventory != null)
            {
                rewardInventory.AddReward(rewardOption);
            }

            Debug.Log($"Selected reward: {rewardOption.Title} ({rewardOption.Description})", this);
        }

        private void BuildRewardOptions(StageNode stageNode)
        {
            List<RewardType> rewardPool = BuildRewardPool(stageNode);
            int targetOptionCount = Mathf.Max(1, optionCount);

            for (int i = 0; i < targetOptionCount; i++)
            {
                if (rewardPool.Count == 0)
                {
                    rewardPool = BuildRewardPool(stageNode);
                }

                int rewardIndex = Random.Range(0, rewardPool.Count);
                RewardType rewardType = rewardPool[rewardIndex];
                rewardPool.RemoveAt(rewardIndex);
                currentOptions.Add(CreateRewardOption(rewardType));
            }
        }

        private List<RewardType> BuildRewardPool(StageNode stageNode)
        {
            List<RewardType> rewardPool = new List<RewardType>
            {
                RewardType.Gold,
                RewardType.Potion,
                RewardType.Artifact
            };

            if (stageNode != null)
            {
                if (stageNode.StageType == StageType.Event || stageNode.StageType == StageType.Modification)
                {
                    rewardPool.Add(RewardType.Tablet);
                }

                if (stageNode.StageType == StageType.Shop || stageNode.StageType == StageType.MiniBoss || stageNode.StageType == StageType.Boss)
                {
                    rewardPool.Add(RewardType.AttackModule);
                }

                if (stageNode.StageType == StageType.Elite || stageNode.StageType == StageType.Boss)
                {
                    rewardPool.Add(RewardType.Artifact);
                }
            }

            return rewardPool;
        }

        private RewardOption CreateRewardOption(RewardType rewardType)
        {
            switch (rewardType)
            {
                case RewardType.Gold:
                    int goldAmount = Random.Range(Mathf.Min(minGold, maxGold), Mathf.Max(minGold, maxGold) + 1);
                    return new RewardOption(RewardType.Gold, $"{goldAmount} Gold", "Save it for shops and future upgrades.", goldAmount);
                case RewardType.Potion:
                    return new RewardOption(RewardType.Potion, "Potion", "Store 1 potion for a later recovery system.", 1);
                case RewardType.Artifact:
                    return new RewardOption(RewardType.Artifact, "Artifact Cache", "Add 1 placeholder artifact to the run inventory.", 1);
                case RewardType.Tablet:
                    return new RewardOption(RewardType.Tablet, "Tablet Fragment", "Add 1 placeholder tablet for future artifact modification.", 1);
                case RewardType.AttackModule:
                    return new RewardOption(RewardType.AttackModule, "Attack Module", "Add 1 placeholder attack module for future swapping.", 1);
                default:
                    return new RewardOption(RewardType.Gold, "Gold", "Fallback reward.", 1);
            }
        }

        private void ShowRewardPanel(StageNode stageNode)
        {
            if (titleText != null)
            {
                titleText.text = $"Stage {stageNode.StageIndex} Reward";
            }

            ConfigureOptionButtons();

            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        private void HideRewardPanel()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        private void ConfigureOptionButtons()
        {
            if (optionButtons == null)
            {
                return;
            }

            for (int i = 0; i < optionButtons.Length; i++)
            {
                Button button = optionButtons[i];
                if (button == null)
                {
                    continue;
                }

                bool hasReward = i < currentOptions.Count;
                button.gameObject.SetActive(hasReward);
                button.onClick.RemoveAllListeners();

                if (!hasReward)
                {
                    continue;
                }

                int rewardIndex = i;
                Text optionText = GetOptionText(i, button);
                if (optionText != null)
                {
                    optionText.text = BuildRewardLabel(rewardIndex, currentOptions[i]);
                }

                button.onClick.AddListener(() => SelectRewardIndex(rewardIndex));
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

        private string BuildRewardLabel(int rewardIndex, RewardOption rewardOption)
        {
            return $"{rewardIndex + 1}. {rewardOption.DisplayText}";
        }

        private void LogRewards(StageNode stageNode)
        {
            if (!logAvailableRewards)
            {
                return;
            }

            List<string> labels = new List<string>();
            for (int i = 0; i < currentOptions.Count; i++)
            {
                labels.Add(BuildRewardLabel(i, currentOptions[i]).Replace("\n", " | "));
            }

            Debug.Log(
                $"Reward room started for stage [{stageNode.NodeId}] {stageNode.StageType}. " +
                $"Choose a reward with number keys or UI buttons:\n{string.Join("\n", labels)}",
                this);
        }

        private void ReadNumberKeySelection()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            int maxChoiceCount = Mathf.Min(currentOptions.Count, 9);
            for (int i = 0; i < maxChoiceCount; i++)
            {
                if (WasRewardKeyPressed(keyboard, i))
                {
                    SelectRewardIndex(i);
                    return;
                }
            }
        }

        private bool WasRewardKeyPressed(Keyboard keyboard, int rewardIndex)
        {
            switch (rewardIndex)
            {
                case 0:
                    return keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame;
                case 1:
                    return keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame;
                case 2:
                    return keyboard.digit3Key.wasPressedThisFrame || keyboard.numpad3Key.wasPressedThisFrame;
                case 3:
                    return keyboard.digit4Key.wasPressedThisFrame || keyboard.numpad4Key.wasPressedThisFrame;
                case 4:
                    return keyboard.digit5Key.wasPressedThisFrame || keyboard.numpad5Key.wasPressedThisFrame;
                case 5:
                    return keyboard.digit6Key.wasPressedThisFrame || keyboard.numpad6Key.wasPressedThisFrame;
                case 6:
                    return keyboard.digit7Key.wasPressedThisFrame || keyboard.numpad7Key.wasPressedThisFrame;
                case 7:
                    return keyboard.digit8Key.wasPressedThisFrame || keyboard.numpad8Key.wasPressedThisFrame;
                case 8:
                    return keyboard.digit9Key.wasPressedThisFrame || keyboard.numpad9Key.wasPressedThisFrame;
                default:
                    return false;
            }
        }

        private void TryAssignReferences()
        {
            if (stageRoomController == null)
            {
                stageRoomController = GetComponent<StageRoomController>();
            }

            if (rewardInventory == null)
            {
                rewardInventory = GetComponent<RunRewardInventory>();
            }
        }
    }
}
