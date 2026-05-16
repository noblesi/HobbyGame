using System;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownRoguelite.Artifact
{
    [Serializable]
    public class ArtifactShape
    {
        private readonly List<Vector2Int> cells = new List<Vector2Int>();

        public ArtifactShape(IEnumerable<Vector2Int> shapeCells)
        {
            foreach (Vector2Int cell in shapeCells)
            {
                if (!cells.Contains(cell))
                {
                    cells.Add(cell);
                }
            }

            if (cells.Count == 0)
            {
                cells.Add(Vector2Int.zero);
            }
        }

        public IReadOnlyList<Vector2Int> Cells => cells;

        public static ArtifactShape SingleCell()
        {
            return new ArtifactShape(new[] { Vector2Int.zero });
        }

        public static ArtifactShape Rectangle(int width, int height)
        {
            List<Vector2Int> shapeCells = new List<Vector2Int>();
            int safeWidth = Mathf.Max(1, width);
            int safeHeight = Mathf.Max(1, height);

            for (int y = 0; y < safeHeight; y++)
            {
                for (int x = 0; x < safeWidth; x++)
                {
                    shapeCells.Add(new Vector2Int(x, y));
                }
            }

            return new ArtifactShape(shapeCells);
        }

        public static ArtifactShape LShape()
        {
            return new ArtifactShape(new[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, 1),
                new Vector2Int(1, 0)
            });
        }
    }
}
