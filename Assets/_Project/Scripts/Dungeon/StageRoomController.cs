using System;
using UnityEngine;

namespace TopDownRoguelite.Dungeon
{
    public class StageRoomController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChapterMapController chapterMapController;

        [Header("Debug")]
        [SerializeField] private bool logRoomProgress = true;

        public StageNode CurrentStage { get; private set; }
        public int CurrentRoomIndex { get; private set; } = -1;
        public bool IsStageComplete { get; private set; }

        public RoomType CurrentRoomType
        {
            get
            {
                if (CurrentStage == null || CurrentRoomIndex < 0 || CurrentRoomIndex >= CurrentStage.RoomSequence.Count)
                {
                    return default;
                }

                return CurrentStage.RoomSequence[CurrentRoomIndex];
            }
        }

        public event Action<StageNode, RoomType, int> RoomStarted;
        public event Action<StageNode> StageCompleted;

        private void OnEnable()
        {
            TryAssignChapterMapController();

            if (chapterMapController != null)
            {
                chapterMapController.StageSelected += StartStage;
            }
        }

        private void Start()
        {
            if (chapterMapController != null && chapterMapController.CurrentNode != null && CurrentStage == null)
            {
                StartStage(chapterMapController.CurrentNode);
            }
        }

        private void OnDisable()
        {
            if (chapterMapController != null)
            {
                chapterMapController.StageSelected -= StartStage;
            }
        }

        public void StartStage(StageNode stageNode)
        {
            if (stageNode == null)
            {
                Debug.LogWarning("Cannot start a null stage node.", this);
                return;
            }

            if (stageNode.RoomSequence.Count == 0)
            {
                Debug.LogWarning($"Stage node {stageNode.NodeId} has no rooms.", this);
                return;
            }

            CurrentStage = stageNode;
            CurrentRoomIndex = 0;
            IsStageComplete = false;
            NotifyRoomStarted();
        }

        [ContextMenu("Complete Current Room")]
        public void CompleteCurrentRoom()
        {
            if (CurrentStage == null)
            {
                Debug.LogWarning("Cannot complete a room before a stage has started.", this);
                return;
            }

            if (IsStageComplete)
            {
                Debug.Log("Current stage is already complete.", this);
                return;
            }

            if (CurrentRoomIndex >= CurrentStage.RoomSequence.Count - 1)
            {
                CompleteStage();
                return;
            }

            CurrentRoomIndex++;
            NotifyRoomStarted();
        }

        [ContextMenu("Complete Current Stage")]
        public void CompleteCurrentStage()
        {
            if (CurrentStage == null)
            {
                Debug.LogWarning("Cannot complete a stage before it has started.", this);
                return;
            }

            CompleteStage();
        }

        private void CompleteStage()
        {
            if (IsStageComplete)
            {
                return;
            }

            IsStageComplete = true;

            if (logRoomProgress)
            {
                Debug.Log($"Stage complete: [{CurrentStage.NodeId}] {CurrentStage.StageType}", this);
            }

            StageCompleted?.Invoke(CurrentStage);
        }

        private void NotifyRoomStarted()
        {
            RoomType currentRoomType = CurrentRoomType;

            if (logRoomProgress)
            {
                Debug.Log(
                    $"Room started: Stage [{CurrentStage.NodeId}] {CurrentStage.StageType} " +
                    $"Room {CurrentRoomIndex + 1}/{CurrentStage.RoomSequence.Count} {currentRoomType}",
                    this);
            }

            RoomStarted?.Invoke(CurrentStage, currentRoomType, CurrentRoomIndex);
        }

        private void TryAssignChapterMapController()
        {
            if (chapterMapController != null)
            {
                return;
            }

            chapterMapController = GetComponent<ChapterMapController>();
        }
    }
}
