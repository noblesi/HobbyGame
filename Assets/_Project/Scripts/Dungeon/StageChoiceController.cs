using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TopDownRoguelite.Dungeon
{
    public class StageChoiceController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChapterMapController chapterMapController;
        [SerializeField] private StageRoomController stageRoomController;

        [Header("Optional UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Text titleText;
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private Text[] optionTexts;

        [Header("Debug Input")]
        [SerializeField] private bool allowNumberKeySelection = true;
        [SerializeField] private bool logAvailableChoices = true;

        private readonly List<StageNode> availableNodes = new List<StageNode>();
        private bool isWaitingForChoice;

        private void OnEnable()
        {
            TryAssignReferences();

            if (stageRoomController != null)
            {
                stageRoomController.StageCompleted += HandleStageCompleted;
            }

            if (chapterMapController != null)
            {
                chapterMapController.StageSelected += HandleStageSelected;
            }
        }

        private void Start()
        {
            HideChoicePanel();
        }

        private void Update()
        {
            if (!isWaitingForChoice || !allowNumberKeySelection)
            {
                return;
            }

            ReadNumberKeySelection();
        }

        private void OnDisable()
        {
            if (stageRoomController != null)
            {
                stageRoomController.StageCompleted -= HandleStageCompleted;
            }

            if (chapterMapController != null)
            {
                chapterMapController.StageSelected -= HandleStageSelected;
            }
        }

        private void HandleStageCompleted(StageNode completedStage)
        {
            availableNodes.Clear();

            if (chapterMapController == null)
            {
                Debug.LogWarning("StageChoiceController chapter map controller is not assigned.", this);
                return;
            }

            availableNodes.AddRange(chapterMapController.GetAvailableNextNodes());
            if (availableNodes.Count == 0)
            {
                isWaitingForChoice = false;
                HideChoicePanel();
                Debug.Log($"Chapter route complete after stage [{completedStage.NodeId}] {completedStage.StageType}.", this);
                return;
            }

            isWaitingForChoice = true;
            ShowChoicePanel(completedStage);
            LogChoices(completedStage);
        }

        private void HandleStageSelected(StageNode selectedStage)
        {
            isWaitingForChoice = false;
            availableNodes.Clear();
            HideChoicePanel();
        }

        [ContextMenu("Select First Stage Choice")]
        public void SelectFirstChoice()
        {
            SelectChoiceIndex(0);
        }

        public void SelectChoiceIndex(int choiceIndex)
        {
            if (!isWaitingForChoice)
            {
                Debug.Log("There is no pending stage choice.", this);
                return;
            }

            if (choiceIndex < 0 || choiceIndex >= availableNodes.Count)
            {
                Debug.LogWarning($"Stage choice index {choiceIndex} is not available.", this);
                return;
            }

            StageNode selectedNode = availableNodes[choiceIndex];
            if (chapterMapController != null && chapterMapController.SelectStageNode(selectedNode.NodeId))
            {
                return;
            }

            Debug.LogWarning($"Failed to select stage node {selectedNode.NodeId}.", this);
        }

        private void ShowChoicePanel(StageNode completedStage)
        {
            if (titleText != null)
            {
                titleText.text = $"Stage {completedStage.StageIndex} Clear - Choose Next Stage";
            }

            ConfigureOptionButtons();

            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        private void HideChoicePanel()
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

                bool hasChoice = i < availableNodes.Count;
                button.gameObject.SetActive(hasChoice);
                button.onClick.RemoveAllListeners();

                if (!hasChoice)
                {
                    continue;
                }

                int choiceIndex = i;
                StageNode node = availableNodes[i];
                Text optionText = GetOptionText(i, button);

                if (optionText != null)
                {
                    optionText.text = BuildChoiceLabel(choiceIndex, node);
                }

                button.onClick.AddListener(() => SelectChoiceIndex(choiceIndex));
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

        private string BuildChoiceLabel(int choiceIndex, StageNode node)
        {
            return $"{choiceIndex + 1}. Stage {node.StageIndex} {node.StageType}\nRooms: {string.Join(" > ", node.RoomSequence)}";
        }

        private void LogChoices(StageNode completedStage)
        {
            if (!logAvailableChoices)
            {
                return;
            }

            List<string> labels = new List<string>();
            for (int i = 0; i < availableNodes.Count; i++)
            {
                labels.Add(BuildChoiceLabel(i, availableNodes[i]).Replace("\n", " | "));
            }

            Debug.Log(
                $"Stage [{completedStage.NodeId}] {completedStage.StageType} is complete. " +
                $"Choose the next stage with number keys or UI buttons:\n{string.Join("\n", labels)}",
                this);
        }

        private void ReadNumberKeySelection()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            int maxChoiceCount = Mathf.Min(availableNodes.Count, 9);
            for (int i = 0; i < maxChoiceCount; i++)
            {
                if (WasChoiceKeyPressed(keyboard, i))
                {
                    SelectChoiceIndex(i);
                    return;
                }
            }
        }

        private bool WasChoiceKeyPressed(Keyboard keyboard, int choiceIndex)
        {
            switch (choiceIndex)
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
            if (chapterMapController == null)
            {
                chapterMapController = GetComponent<ChapterMapController>();
            }

            if (stageRoomController == null)
            {
                stageRoomController = GetComponent<StageRoomController>();
            }
        }
    }
}
