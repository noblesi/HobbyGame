using System.Collections.Generic;
using System.Text;

namespace TopDownRoguelite.Dungeon
{
    public class ChapterMap
    {
        private readonly List<StageNode> nodes = new List<StageNode>();
        private readonly Dictionary<int, StageNode> nodesById = new Dictionary<int, StageNode>();

        public IReadOnlyList<StageNode> Nodes => nodes;
        public StageNode CurrentNode { get; private set; }

        public void AddNode(StageNode node)
        {
            if (node == null || nodesById.ContainsKey(node.NodeId))
            {
                return;
            }

            nodes.Add(node);
            nodesById.Add(node.NodeId, node);

            if (node.StageType == StageType.Start)
            {
                CurrentNode = node;
            }
        }

        public bool TryGetNode(int nodeId, out StageNode node)
        {
            return nodesById.TryGetValue(nodeId, out node);
        }

        public bool TrySelectNode(int nodeId)
        {
            if (!TryGetNode(nodeId, out StageNode node))
            {
                return false;
            }

            if (CurrentNode == null)
            {
                CurrentNode = node;
                return true;
            }

            if (!HasConnectionTo(nodeId))
            {
                return false;
            }

            CurrentNode = node;
            return true;
        }

        private bool HasConnectionTo(int nodeId)
        {
            for (int i = 0; i < CurrentNode.ConnectedNextNodeIds.Count; i++)
            {
                if (CurrentNode.ConnectedNextNodeIds[i] == nodeId)
                {
                    return true;
                }
            }

            return false;
        }

        public List<StageNode> GetNodesAtStage(int stageIndex)
        {
            List<StageNode> results = new List<StageNode>();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].StageIndex == stageIndex)
                {
                    results.Add(nodes[i]);
                }
            }

            return results;
        }

        public string BuildDebugSummary()
        {
            StringBuilder builder = new StringBuilder();
            int highestStageIndex = 0;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].StageIndex > highestStageIndex)
                {
                    highestStageIndex = nodes[i].StageIndex;
                }
            }

            for (int stageIndex = 1; stageIndex <= highestStageIndex; stageIndex++)
            {
                builder.Append("Stage ");
                builder.Append(stageIndex);
                builder.AppendLine();

                List<StageNode> stageNodes = GetNodesAtStage(stageIndex);
                for (int i = 0; i < stageNodes.Count; i++)
                {
                    StageNode node = stageNodes[i];
                    builder.Append("  [");
                    builder.Append(node.NodeId);
                    builder.Append("] ");
                    builder.Append(node.StageType);
                    builder.Append(" -> ");

                    if (node.ConnectedNextNodeIds.Count == 0)
                    {
                        builder.Append("End");
                    }
                    else
                    {
                        builder.Append(string.Join(", ", node.ConnectedNextNodeIds));
                    }

                    builder.Append(" | Rooms: ");
                    builder.AppendLine(string.Join(" > ", node.RoomSequence));
                }
            }

            return builder.ToString();
        }
    }
}
