using System.Collections;
using UnityEngine;

namespace TopDownRoguelite.Dungeon
{
    public class StageRoomPlaceholderController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StageRoomController stageRoomController;

        [Header("Placeholder Rooms")]
        [SerializeField] private bool autoCompleteEntranceAndExit = true;
        [SerializeField] private bool autoCompleteUnimplementedRooms = true;
        [Min(0f)]
        [SerializeField] private float autoCompleteDelay = 0.35f;

        [Header("Debug")]
        [SerializeField] private bool logPlaceholderProgress = true;

        private Coroutine autoCompleteRoutine;

        private void OnEnable()
        {
            TryAssignReferences();

            if (stageRoomController != null)
            {
                stageRoomController.RoomStarted += HandleRoomStarted;
                stageRoomController.StageCompleted += HandleStageCompleted;
            }
        }

        private void OnDisable()
        {
            if (stageRoomController != null)
            {
                stageRoomController.RoomStarted -= HandleRoomStarted;
                stageRoomController.StageCompleted -= HandleStageCompleted;
            }

            StopAutoCompleteRoutine();
        }

        private void HandleRoomStarted(StageNode stageNode, RoomType roomType, int roomIndex)
        {
            StopAutoCompleteRoutine();

            if (!ShouldAutoComplete(roomType))
            {
                return;
            }

            autoCompleteRoutine = StartCoroutine(AutoCompleteRoomAfterDelay(stageNode, roomType, roomIndex));
        }

        private void HandleStageCompleted(StageNode stageNode)
        {
            StopAutoCompleteRoutine();
        }

        private IEnumerator AutoCompleteRoomAfterDelay(StageNode stageNode, RoomType roomType, int roomIndex)
        {
            if (autoCompleteDelay > 0f)
            {
                yield return new WaitForSeconds(autoCompleteDelay);
            }

            autoCompleteRoutine = null;

            if (stageRoomController == null ||
                stageRoomController.IsStageComplete ||
                stageRoomController.CurrentStage != stageNode ||
                stageRoomController.CurrentRoomIndex != roomIndex ||
                stageRoomController.CurrentRoomType != roomType)
            {
                yield break;
            }

            if (logPlaceholderProgress)
            {
                Debug.Log(
                    $"Auto-completing placeholder room: Stage [{stageNode.NodeId}] " +
                    $"{stageNode.StageType} Room {roomIndex + 1} {roomType}",
                    this);
            }

            stageRoomController.CompleteCurrentRoom();
        }

        private bool ShouldAutoComplete(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Entrance:
                case RoomType.Exit:
                    return autoCompleteEntranceAndExit;
                case RoomType.EventQuest:
                case RoomType.Shop:
                case RoomType.Elite:
                case RoomType.Modification:
                case RoomType.MiniBoss:
                case RoomType.Boss:
                    return autoCompleteUnimplementedRooms;
                case RoomType.Combat:
                case RoomType.Reward:
                    return false;
                default:
                    return false;
            }
        }

        private void StopAutoCompleteRoutine()
        {
            if (autoCompleteRoutine == null)
            {
                return;
            }

            StopCoroutine(autoCompleteRoutine);
            autoCompleteRoutine = null;
        }

        private void TryAssignReferences()
        {
            if (stageRoomController == null)
            {
                stageRoomController = GetComponent<StageRoomController>();
            }
        }
    }
}
