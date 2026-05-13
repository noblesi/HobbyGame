using System.Collections.Generic;
using TopDownRoguelite.Enemy;
using UnityEngine;

namespace TopDownRoguelite.Dungeon
{
    public class CombatRoomController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StageRoomController stageRoomController;
        [SerializeField] private EnemySpawner enemySpawner;

        [Header("Clear Condition")]
        [SerializeField] private RoomClearCondition clearCondition = RoomClearCondition.KillEnemies;
        [Min(1)]
        [SerializeField] private int requiredKills = 10;
        [Min(1f)]
        [SerializeField] private float surviveSeconds = 30f;

        [Header("Debug")]
        [SerializeField] private bool logCombatProgress = true;

        private readonly HashSet<EnemyHealth> trackedEnemies = new HashSet<EnemyHealth>();
        private int currentKills;
        private float combatTimer;
        private bool isCombatRoomActive;

        private void OnEnable()
        {
            TryAssignReferences();

            if (stageRoomController != null)
            {
                stageRoomController.RoomStarted += HandleRoomStarted;
                stageRoomController.StageCompleted += HandleStageCompleted;
            }

            if (enemySpawner != null)
            {
                enemySpawner.EnemySpawned += TrackEnemy;
            }
        }

        private void Update()
        {
            if (!isCombatRoomActive || clearCondition != RoomClearCondition.SurviveTime)
            {
                return;
            }

            combatTimer += Time.deltaTime;
            if (combatTimer >= surviveSeconds)
            {
                CompleteCombatRoom();
            }
        }

        private void OnDisable()
        {
            if (stageRoomController != null)
            {
                stageRoomController.RoomStarted -= HandleRoomStarted;
                stageRoomController.StageCompleted -= HandleStageCompleted;
            }

            if (enemySpawner != null)
            {
                enemySpawner.EnemySpawned -= TrackEnemy;
            }

            StopCombatRoom();
        }

        private void HandleRoomStarted(StageNode stageNode, RoomType roomType, int roomIndex)
        {
            if (roomType == RoomType.Combat)
            {
                StartCombatRoom(stageNode, roomIndex);
                return;
            }

            StopCombatRoom();
        }

        private void HandleStageCompleted(StageNode stageNode)
        {
            StopCombatRoom();
        }

        private void StartCombatRoom(StageNode stageNode, int roomIndex)
        {
            currentKills = 0;
            combatTimer = 0f;
            isCombatRoomActive = true;
            ClearTrackedEnemies();
            TrackExistingEnemies();

            if (enemySpawner != null)
            {
                enemySpawner.SetSpawning(true);
            }

            if (logCombatProgress)
            {
                Debug.Log(
                    $"Combat room started: Stage [{stageNode.NodeId}] Room {roomIndex + 1} " +
                    $"Condition {clearCondition} ({GetConditionTargetText()})",
                    this);
            }

            if (clearCondition == RoomClearCondition.ManualDebug)
            {
                Debug.Log("ManualDebug combat room is waiting for Complete Combat Room.", this);
            }
        }

        private void StopCombatRoom()
        {
            if (enemySpawner != null)
            {
                enemySpawner.SetSpawning(false);
            }

            isCombatRoomActive = false;
            combatTimer = 0f;
            ClearTrackedEnemies();
        }

        private void TrackExistingEnemies()
        {
            EnemyHealth[] enemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None);
            for (int i = 0; i < enemies.Length; i++)
            {
                TrackEnemy(enemies[i]);
            }
        }

        private void TrackEnemy(EnemyHealth enemyHealth)
        {
            if (!isCombatRoomActive || enemyHealth == null || trackedEnemies.Contains(enemyHealth))
            {
                return;
            }

            trackedEnemies.Add(enemyHealth);
            enemyHealth.Died += HandleEnemyDied;
        }

        private void HandleEnemyDied(EnemyHealth enemyHealth)
        {
            if (enemyHealth != null)
            {
                enemyHealth.Died -= HandleEnemyDied;
                trackedEnemies.Remove(enemyHealth);
            }

            if (!isCombatRoomActive || clearCondition != RoomClearCondition.KillEnemies)
            {
                return;
            }

            currentKills++;

            if (logCombatProgress)
            {
                Debug.Log($"Combat room kills: {currentKills}/{requiredKills}", this);
            }

            if (currentKills >= requiredKills)
            {
                CompleteCombatRoom();
            }
        }

        [ContextMenu("Complete Combat Room")]
        public void CompleteCombatRoom()
        {
            if (!isCombatRoomActive)
            {
                Debug.Log("No active combat room to complete.", this);
                return;
            }

            if (logCombatProgress)
            {
                Debug.Log("Combat room complete.", this);
            }

            StopCombatRoom();

            if (stageRoomController != null)
            {
                stageRoomController.CompleteCurrentRoom();
            }
        }

        private void ClearTrackedEnemies()
        {
            foreach (EnemyHealth enemyHealth in trackedEnemies)
            {
                if (enemyHealth != null)
                {
                    enemyHealth.Died -= HandleEnemyDied;
                }
            }

            trackedEnemies.Clear();
        }

        private string GetConditionTargetText()
        {
            switch (clearCondition)
            {
                case RoomClearCondition.KillEnemies:
                    return $"{requiredKills} kills";
                case RoomClearCondition.SurviveTime:
                    return $"{surviveSeconds:0.#} seconds";
                case RoomClearCondition.ManualDebug:
                    return "manual";
                default:
                    return "unknown";
            }
        }

        private void TryAssignReferences()
        {
            if (stageRoomController == null)
            {
                stageRoomController = GetComponent<StageRoomController>();
            }

            if (enemySpawner == null)
            {
                enemySpawner = FindFirstObjectByType<EnemySpawner>();
            }
        }
    }
}
