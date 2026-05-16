using System;
using System.Collections.Generic;
using System.Text;
using TopDownRoguelite.Artifact;
using UnityEngine;

namespace TopDownRoguelite.Inventory
{
    public class ArtifactGridInventory : MonoBehaviour
    {
        [Header("Grid")]
        [Min(1)]
        [SerializeField] private int width = 6;
        [Min(1)]
        [SerializeField] private int height = 4;

        [Header("Debug")]
        [SerializeField] private bool logChanges = true;

        private readonly List<ArtifactInstance> placedArtifacts = new List<ArtifactInstance>();
        private ArtifactInstance[,] grid;
        private int nextDebugArtifactId = 1;

        public int Width => Mathf.Max(1, width);
        public int Height => Mathf.Max(1, height);
        public IReadOnlyList<ArtifactInstance> PlacedArtifacts => placedArtifacts;

        public event Action<ArtifactInstance> ArtifactPlaced;
        public event Action<ArtifactInstance> ArtifactRemoved;

        private void Awake()
        {
            RebuildGrid();
        }

        public bool CanPlace(ArtifactInstance artifact, Vector2Int origin)
        {
            EnsureGrid();

            if (artifact == null || artifact.Shape == null || placedArtifacts.Contains(artifact))
            {
                return false;
            }

            for (int i = 0; i < artifact.Shape.Cells.Count; i++)
            {
                Vector2Int cell = origin + artifact.Shape.Cells[i];
                if (!IsInsideGrid(cell) || grid[cell.x, cell.y] != null)
                {
                    return false;
                }
            }

            return true;
        }

        public bool TryPlace(ArtifactInstance artifact, Vector2Int origin)
        {
            if (!CanPlace(artifact, origin))
            {
                if (logChanges && artifact != null)
                {
                    Debug.LogWarning($"Cannot place artifact {artifact.DisplayName} at {origin}.", this);
                }

                return false;
            }

            for (int i = 0; i < artifact.Shape.Cells.Count; i++)
            {
                Vector2Int cell = origin + artifact.Shape.Cells[i];
                grid[cell.x, cell.y] = artifact;
            }

            artifact.MarkPlaced(origin);
            placedArtifacts.Add(artifact);
            ArtifactPlaced?.Invoke(artifact);

            if (logChanges)
            {
                Debug.Log($"Placed artifact {artifact.DisplayName} at {origin}.\n{BuildDebugGrid()}", this);
            }

            return true;
        }

        public bool TryRemove(ArtifactInstance artifact)
        {
            EnsureGrid();

            if (artifact == null || !placedArtifacts.Contains(artifact))
            {
                return false;
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (grid[x, y] == artifact)
                    {
                        grid[x, y] = null;
                    }
                }
            }

            placedArtifacts.Remove(artifact);
            artifact.MarkRemoved();
            ArtifactRemoved?.Invoke(artifact);

            if (logChanges)
            {
                Debug.Log($"Removed artifact {artifact.DisplayName}.\n{BuildDebugGrid()}", this);
            }

            return true;
        }

        public bool TryRemoveAt(Vector2Int cell)
        {
            ArtifactInstance artifact = GetArtifactAt(cell);
            return artifact != null && TryRemove(artifact);
        }

        public ArtifactInstance GetArtifactAt(Vector2Int cell)
        {
            EnsureGrid();
            return IsInsideGrid(cell) ? grid[cell.x, cell.y] : null;
        }

        public string BuildDebugGrid()
        {
            EnsureGrid();

            StringBuilder builder = new StringBuilder();
            for (int y = Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    ArtifactInstance artifact = grid[x, y];
                    builder.Append(artifact == null ? "." : GetArtifactDebugLetter(artifact));
                }

                if (y > 0)
                {
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        [ContextMenu("Debug Place Sample Artifact")]
        private void DebugPlaceSampleArtifact()
        {
            ArtifactInstance artifact = CreateDebugArtifact();
            if (!TryPlaceAtFirstAvailableCell(artifact))
            {
                Debug.LogWarning($"No available space for debug artifact {artifact.DisplayName}.", this);
            }
        }

        [ContextMenu("Debug Remove First Artifact")]
        private void DebugRemoveFirstArtifact()
        {
            if (placedArtifacts.Count == 0)
            {
                Debug.Log("There is no placed artifact to remove.", this);
                return;
            }

            TryRemove(placedArtifacts[0]);
        }

        private bool TryPlaceAtFirstAvailableCell(ArtifactInstance artifact)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (TryPlace(artifact, new Vector2Int(x, y)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private ArtifactInstance CreateDebugArtifact()
        {
            int artifactId = nextDebugArtifactId++;

            switch (artifactId % 3)
            {
                case 1:
                    return new ArtifactInstance($"debug-{artifactId}", $"Debug Relic {artifactId}", ArtifactShape.Rectangle(2, 1));
                case 2:
                    return new ArtifactInstance($"debug-{artifactId}", $"Debug Relic {artifactId}", ArtifactShape.LShape());
                default:
                    return new ArtifactInstance($"debug-{artifactId}", $"Debug Relic {artifactId}", ArtifactShape.Rectangle(1, 2));
            }
        }

        private string GetArtifactDebugLetter(ArtifactInstance artifact)
        {
            int index = placedArtifacts.IndexOf(artifact);
            if (index < 0)
            {
                return "?";
            }

            char letter = (char)('A' + index % 26);
            return letter.ToString();
        }

        private bool IsInsideGrid(Vector2Int cell)
        {
            return cell.x >= 0 && cell.x < Width && cell.y >= 0 && cell.y < Height;
        }

        private void EnsureGrid()
        {
            if (grid == null || grid.GetLength(0) != Width || grid.GetLength(1) != Height)
            {
                RebuildGrid();
            }
        }

        private void RebuildGrid()
        {
            grid = new ArtifactInstance[Width, Height];
            placedArtifacts.Clear();
        }
    }
}
