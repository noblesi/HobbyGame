using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownRoguelite.Dungeon
{
    public class ChapterMapController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChapterMapGenerator mapGenerator;

        [Header("Startup")]
        [SerializeField] private bool generateOnStart = true;

        [Header("Debug")]
        [SerializeField] private bool logCurrentChoices = true;

        public ChapterMap CurrentMap { get; private set; }
        public StageNode CurrentNode => CurrentMap != null ? CurrentMap.CurrentNode : null;

        public event Action<StageNode> StageSelected;

        private void Start()
        {
            TryAssignGenerator();

            if (generateOnStart)
            {
                GenerateNewChapter();
            }
        }

        [ContextMenu("Generate New Chapter")]
        public void GenerateNewChapter()
        {
            if (mapGenerator == null)
            {
                Debug.LogWarning("ChapterMapController map generator is not assigned.", this);
                return;
            }

            CurrentMap = mapGenerator.Generate();
            StageSelected?.Invoke(CurrentNode);
            LogCurrentNodeAndChoices();
        }

        [ContextMenu("Select First Available Node")]
        public void SelectFirstAvailableNode()
        {
            List<StageNode> availableNodes = GetAvailableNextNodes();
            if (availableNodes.Count == 0)
            {
                Debug.Log("No available next stage node.", this);
                return;
            }

            SelectStageNode(availableNodes[0].NodeId);
        }

        public bool SelectStageNode(int nodeId)
        {
            if (CurrentMap == null)
            {
                Debug.LogWarning("Cannot select a stage node before generating a chapter map.", this);
                return false;
            }

            if (!CurrentMap.TrySelectNode(nodeId))
            {
                Debug.LogWarning($"Stage node {nodeId} is not available from the current node.", this);
                return false;
            }

            StageSelected?.Invoke(CurrentNode);
            LogCurrentNodeAndChoices();
            return true;
        }

        public List<StageNode> GetAvailableNextNodes()
        {
            List<StageNode> availableNodes = new List<StageNode>();

            if (CurrentMap == null || CurrentNode == null)
            {
                return availableNodes;
            }

            for (int i = 0; i < CurrentNode.ConnectedNextNodeIds.Count; i++)
            {
                int nodeId = CurrentNode.ConnectedNextNodeIds[i];
                if (CurrentMap.TryGetNode(nodeId, out StageNode node))
                {
                    availableNodes.Add(node);
                }
            }

            return availableNodes;
        }

        private void LogCurrentNodeAndChoices()
        {
            if (!logCurrentChoices || CurrentNode == null)
            {
                return;
            }

            List<StageNode> availableNodes = GetAvailableNextNodes();
            string choicesText = availableNodes.Count == 0 ? "None" : BuildChoiceText(availableNodes);
            Debug.Log(
                $"Current Stage Node: [{CurrentNode.NodeId}] {CurrentNode.StageType}\n" +
                $"Rooms: {string.Join(" > ", CurrentNode.RoomSequence)}\n" +
                $"Available Next Nodes: {choicesText}",
                this);
        }

        private string BuildChoiceText(List<StageNode> availableNodes)
        {
            List<string> parts = new List<string>();

            for (int i = 0; i < availableNodes.Count; i++)
            {
                StageNode node = availableNodes[i];
                parts.Add($"[{node.NodeId}] Stage {node.StageIndex} {node.StageType}");
            }

            return string.Join(", ", parts);
        }

        private void TryAssignGenerator()
        {
            if (mapGenerator != null)
            {
                return;
            }

            mapGenerator = GetComponent<ChapterMapGenerator>();
        }
    }
}
