using UnityEngine;

namespace TopDownRoguelite.Map
{
    public class MapChunk : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer floorRenderer;
        [SerializeField] private bool resizeFloorRenderer = true;

        [Header("Chunk")]
        [SerializeField] private Vector2 size = new Vector2(20f, 20f);

        [Header("Decorations")]
        [SerializeField] private GameObject[] decorationPrefabs;
        [Min(0)]
        [SerializeField] private int decorationCount = 8;
        [SerializeField] private Vector2 decorationPadding = new Vector2(1f, 1f);

        public Vector2Int Coordinates { get; private set; }
        public Vector2 Size => size;

        private readonly System.Collections.Generic.List<GameObject> decorations = new System.Collections.Generic.List<GameObject>();

        private void Awake()
        {
            if (floorRenderer == null)
            {
                floorRenderer = GetComponent<SpriteRenderer>();
            }

            ApplyVisualSize();
        }

        public void Configure(Vector2Int coordinates, Vector2 newSize)
        {
            Coordinates = coordinates;
            size = newSize;
            transform.position = GetWorldPosition(coordinates, size);
            ApplyVisualSize();
            RebuildDecorations();
        }

        public static Vector3 GetWorldPosition(Vector2Int coordinates, Vector2 chunkSize)
        {
            return new Vector3(coordinates.x * chunkSize.x, coordinates.y * chunkSize.y, 0f);
        }

        private void ApplyVisualSize()
        {
            if (!resizeFloorRenderer || floorRenderer == null)
            {
                return;
            }

            floorRenderer.drawMode = SpriteDrawMode.Simple;

            if (floorRenderer.sprite == null)
            {
                floorRenderer.transform.localScale = new Vector3(size.x, size.y, 1f);
                return;
            }

            Vector2 spriteSize = floorRenderer.sprite.bounds.size;
            float scaleX = size.x / Mathf.Max(0.01f, spriteSize.x);
            float scaleY = size.y / Mathf.Max(0.01f, spriteSize.y);
            floorRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        private void RebuildDecorations()
        {
            ClearDecorations();

            if (decorationPrefabs == null || decorationPrefabs.Length == 0 || decorationCount <= 0)
            {
                return;
            }

            System.Random random = new System.Random(Coordinates.x * 73856093 ^ Coordinates.y * 19349663);

            for (int i = 0; i < decorationCount; i++)
            {
                GameObject prefab = decorationPrefabs[random.Next(0, decorationPrefabs.Length)];
                if (prefab == null)
                {
                    continue;
                }

                float minX = -size.x * 0.5f + decorationPadding.x;
                float maxX = size.x * 0.5f - decorationPadding.x;
                float minY = -size.y * 0.5f + decorationPadding.y;
                float maxY = size.y * 0.5f - decorationPadding.y;
                float x = Mathf.Lerp(minX, maxX, (float)random.NextDouble());
                float y = Mathf.Lerp(minY, maxY, (float)random.NextDouble());
                Vector3 localPosition = new Vector3(x, y, -0.05f);
                GameObject decoration = Instantiate(prefab, transform);
                decoration.transform.localPosition = localPosition;
                decorations.Add(decoration);
            }
        }

        private void ClearDecorations()
        {
            for (int i = decorations.Count - 1; i >= 0; i--)
            {
                if (decorations[i] != null)
                {
                    Destroy(decorations[i]);
                }
            }

            decorations.Clear();
        }
    }
}
