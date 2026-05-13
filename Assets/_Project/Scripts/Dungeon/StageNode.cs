using System;
using System.Collections.Generic;

namespace TopDownRoguelite.Dungeon
{
    [Serializable]
    public class StageNode
    {
        private readonly List<int> connectedNextNodeIds = new List<int>();
        private readonly List<RoomType> roomSequence = new List<RoomType>();

        public StageNode(int nodeId, int stageIndex, int optionIndex, StageType stageType)
        {
            NodeId = nodeId;
            StageIndex = stageIndex;
            OptionIndex = optionIndex;
            StageType = stageType;
        }

        public int NodeId { get; }
        public int StageIndex { get; }
        public int OptionIndex { get; }
        public StageType StageType { get; }
        public IReadOnlyList<int> ConnectedNextNodeIds => connectedNextNodeIds;
        public IReadOnlyList<RoomType> RoomSequence => roomSequence;

        public void AddConnection(int nextNodeId)
        {
            if (connectedNextNodeIds.Contains(nextNodeId))
            {
                return;
            }

            connectedNextNodeIds.Add(nextNodeId);
        }

        public void AddRoom(RoomType roomType)
        {
            roomSequence.Add(roomType);
        }
    }
}
