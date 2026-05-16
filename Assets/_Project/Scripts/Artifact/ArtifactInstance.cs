using UnityEngine;

namespace TopDownRoguelite.Artifact
{
    public class ArtifactInstance
    {
        public ArtifactInstance(string instanceId, string displayName, ArtifactShape shape)
        {
            InstanceId = string.IsNullOrWhiteSpace(instanceId) ? "artifact" : instanceId;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? "Artifact" : displayName;
            Shape = shape ?? ArtifactShape.SingleCell();
        }

        public string InstanceId { get; }
        public string DisplayName { get; }
        public ArtifactShape Shape { get; }
        public Vector2Int GridPosition { get; private set; }
        public bool IsPlaced { get; private set; }

        public void MarkPlaced(Vector2Int gridPosition)
        {
            GridPosition = gridPosition;
            IsPlaced = true;
        }

        public void MarkRemoved()
        {
            GridPosition = Vector2Int.zero;
            IsPlaced = false;
        }
    }
}
