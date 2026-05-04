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

        public Vector2Int Coordinates { get; private set; }
        public Vector2 Size => size;

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
    }
}
