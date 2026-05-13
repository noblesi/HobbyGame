using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownRoguelite.Dungeon
{
    public class ChapterMapGenerator : MonoBehaviour
    {
        [Header("Chapter")]
        [Min(1)]
        [SerializeField] private int chapterIndex = 1;
        [SerializeField] private bool useRandomSeed = true;
        [SerializeField] private int seed = 12345;

        [Header("Stage Options")]
        [Min(1)]
        [SerializeField] private int minMiddleStageOptions = 2;
        [Min(1)]
        [SerializeField] private int maxMiddleStageOptions = 3;

        [Header("Debug")]
        [SerializeField] private bool logGeneratedMap = true;

        private readonly StageType[] middleStageTypes =
        {
            StageType.Event,
            StageType.Shop,
            StageType.Elite,
            StageType.Modification,
            StageType.MiniBoss
        };

        public ChapterMap Generate()
        {
            System.Random random = useRandomSeed ? new System.Random() : new System.Random(seed);
            ChapterMap chapterMap = new ChapterMap();
            List<List<StageNode>> nodesByStage = CreateStageNodes(chapterMap, random);
            ConnectStages(nodesByStage, random);

            if (logGeneratedMap)
            {
                Debug.Log($"Generated Chapter {chapterIndex} map:\n{chapterMap.BuildDebugSummary()}", this);
            }

            return chapterMap;
        }

        private List<List<StageNode>> CreateStageNodes(ChapterMap chapterMap, System.Random random)
        {
            List<List<StageNode>> nodesByStage = new List<List<StageNode>>();
            int nextNodeId = 1;

            AddStage(nodesByStage, chapterMap, new StageNode(nextNodeId++, 1, 0, StageType.Start));

            for (int stageIndex = 2; stageIndex <= 5; stageIndex++)
            {
                int optionCount = random.Next(GetMinMiddleStageOptions(), GetMaxMiddleStageOptions() + 1);

                for (int optionIndex = 0; optionIndex < optionCount; optionIndex++)
                {
                    StageType stageType = GetRandomMiddleStageType(random);
                    AddStage(nodesByStage, chapterMap, new StageNode(nextNodeId++, stageIndex, optionIndex, stageType));
                }
            }

            AddStage(nodesByStage, chapterMap, new StageNode(nextNodeId, 6, 0, StageType.Boss));

            return nodesByStage;
        }

        private void AddStage(List<List<StageNode>> nodesByStage, ChapterMap chapterMap, StageNode node)
        {
            while (nodesByStage.Count < node.StageIndex)
            {
                nodesByStage.Add(new List<StageNode>());
            }

            AddRooms(node);
            nodesByStage[node.StageIndex - 1].Add(node);
            chapterMap.AddNode(node);
        }

        private void ConnectStages(List<List<StageNode>> nodesByStage, System.Random random)
        {
            for (int stageIndex = 0; stageIndex < nodesByStage.Count - 1; stageIndex++)
            {
                List<StageNode> currentStageNodes = nodesByStage[stageIndex];
                List<StageNode> nextStageNodes = nodesByStage[stageIndex + 1];

                for (int i = 0; i < currentStageNodes.Count; i++)
                {
                    StageNode currentNode = currentStageNodes[i];
                    StageNode firstConnection = nextStageNodes[random.Next(nextStageNodes.Count)];
                    currentNode.AddConnection(firstConnection.NodeId);

                    if (nextStageNodes.Count <= 1 || random.NextDouble() > 0.45d)
                    {
                        continue;
                    }

                    StageNode secondConnection = nextStageNodes[random.Next(nextStageNodes.Count)];
                    currentNode.AddConnection(secondConnection.NodeId);
                }

                EnsureEveryNextNodeHasIncomingConnection(currentStageNodes, nextStageNodes, random);
            }
        }

        private void EnsureEveryNextNodeHasIncomingConnection(
            List<StageNode> currentStageNodes,
            List<StageNode> nextStageNodes,
            System.Random random)
        {
            for (int i = 0; i < nextStageNodes.Count; i++)
            {
                StageNode nextNode = nextStageNodes[i];
                bool hasIncomingConnection = false;

                for (int j = 0; j < currentStageNodes.Count; j++)
                {
                    if (HasConnectionTo(currentStageNodes[j], nextNode.NodeId))
                    {
                        hasIncomingConnection = true;
                        break;
                    }
                }

                if (hasIncomingConnection)
                {
                    continue;
                }

                StageNode sourceNode = currentStageNodes[random.Next(currentStageNodes.Count)];
                sourceNode.AddConnection(nextNode.NodeId);
            }
        }

        private bool HasConnectionTo(StageNode node, int nextNodeId)
        {
            for (int i = 0; i < node.ConnectedNextNodeIds.Count; i++)
            {
                if (node.ConnectedNextNodeIds[i] == nextNodeId)
                {
                    return true;
                }
            }

            return false;
        }

        private StageType GetRandomMiddleStageType(System.Random random)
        {
            return middleStageTypes[random.Next(middleStageTypes.Length)];
        }

        private void AddRooms(StageNode node)
        {
            node.AddRoom(RoomType.Entrance);

            switch (node.StageType)
            {
                case StageType.Start:
                    node.AddRoom(RoomType.Combat);
                    node.AddRoom(RoomType.Reward);
                    node.AddRoom(RoomType.Exit);
                    break;
                case StageType.Event:
                    node.AddRoom(RoomType.EventQuest);
                    node.AddRoom(RoomType.Combat);
                    node.AddRoom(RoomType.Combat);
                    node.AddRoom(RoomType.Reward);
                    node.AddRoom(RoomType.Exit);
                    break;
                case StageType.Shop:
                    node.AddRoom(RoomType.Combat);
                    node.AddRoom(RoomType.Shop);
                    node.AddRoom(RoomType.Exit);
                    break;
                case StageType.Elite:
                    node.AddRoom(RoomType.Combat);
                    node.AddRoom(RoomType.Elite);
                    node.AddRoom(RoomType.Reward);
                    node.AddRoom(RoomType.Exit);
                    break;
                case StageType.Modification:
                    node.AddRoom(RoomType.Combat);
                    node.AddRoom(RoomType.Modification);
                    node.AddRoom(RoomType.Exit);
                    break;
                case StageType.MiniBoss:
                    node.AddRoom(RoomType.Combat);
                    node.AddRoom(RoomType.MiniBoss);
                    node.AddRoom(RoomType.Reward);
                    node.AddRoom(RoomType.Exit);
                    break;
                case StageType.Boss:
                    node.AddRoom(RoomType.Boss);
                    node.AddRoom(RoomType.Reward);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetMinMiddleStageOptions()
        {
            return Mathf.Max(1, Mathf.Min(minMiddleStageOptions, maxMiddleStageOptions));
        }

        private int GetMaxMiddleStageOptions()
        {
            return Mathf.Max(GetMinMiddleStageOptions(), maxMiddleStageOptions);
        }
    }
}
