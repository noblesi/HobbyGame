using System.Collections.Generic;
using UnityEngine;

namespace TopDownRoguelite.Map
{
    public class InfiniteMapScroller : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;

        [Header("Chunk")]
        [SerializeField] private MapChunk chunkPrefab;
        [SerializeField] private Vector2 chunkSize = new Vector2(20f, 20f);
        [Min(1)]
        [SerializeField] private int visibleChunksPerAxis = 3;

        private readonly Dictionary<Vector2Int, MapChunk> activeChunks = new Dictionary<Vector2Int, MapChunk>();
        private readonly List<MapChunk> chunkPool = new List<MapChunk>();
        private Vector2Int currentCenterCoordinates;
        private bool hasInitialized;
        private bool hasLoggedMissingPrefabWarning;

        private void Start()
        {
            TryAssignPlayerTarget();

            if (target == null)
            {
                Debug.LogWarning("InfiniteMapScroller target is not assigned.", this);
                return;
            }

            if (chunkPrefab == null)
            {
                LogMissingPrefabWarning();
                return;
            }

            InitializeChunks();
        }

        private void LateUpdate()
        {
            if (!hasInitialized || target == null)
            {
                return;
            }

            Vector2Int nextCenterCoordinates = WorldToChunkCoordinates(target.position);
            if (nextCenterCoordinates == currentCenterCoordinates)
            {
                return;
            }

            currentCenterCoordinates = nextCenterCoordinates;
            RebuildVisibleChunks();
        }

        private void InitializeChunks()
        {
            currentCenterCoordinates = WorldToChunkCoordinates(target.position);

            int chunkCount = GetVisibleChunkCount();
            for (int i = 0; i < chunkCount; i++)
            {
                MapChunk chunk = Instantiate(chunkPrefab, transform);
                chunkPool.Add(chunk);
            }

            RebuildVisibleChunks();
            hasInitialized = true;
        }

        private void RebuildVisibleChunks()
        {
            activeChunks.Clear();

            int radius = GetVisibleChunkRadius();
            Vector2 safeChunkSize = GetSafeChunkSize();
            int poolIndex = 0;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    Vector2Int coordinates = currentCenterCoordinates + new Vector2Int(x, y);
                    MapChunk chunk = chunkPool[poolIndex];
                    chunk.Configure(coordinates, safeChunkSize);
                    activeChunks.Add(coordinates, chunk);
                    poolIndex++;
                }
            }
        }

        private Vector2Int WorldToChunkCoordinates(Vector3 worldPosition)
        {
            Vector2 safeChunkSize = GetSafeChunkSize();
            int x = Mathf.FloorToInt(worldPosition.x / safeChunkSize.x);
            int y = Mathf.FloorToInt(worldPosition.y / safeChunkSize.y);
            return new Vector2Int(x, y);
        }

        private int GetVisibleChunkRadius()
        {
            int oddChunkCount = visibleChunksPerAxis % 2 == 0 ? visibleChunksPerAxis + 1 : visibleChunksPerAxis;
            return Mathf.Max(1, oddChunkCount / 2);
        }

        private int GetVisibleChunkCount()
        {
            int diameter = GetVisibleChunkRadius() * 2 + 1;
            return diameter * diameter;
        }

        private Vector2 GetSafeChunkSize()
        {
            return new Vector2(Mathf.Max(0.1f, chunkSize.x), Mathf.Max(0.1f, chunkSize.y));
        }

        private void TryAssignPlayerTarget()
        {
            if (target != null)
            {
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        private void LogMissingPrefabWarning()
        {
            if (hasLoggedMissingPrefabWarning)
            {
                return;
            }

            Debug.LogWarning("InfiniteMapScroller chunk prefab is not assigned.", this);
            hasLoggedMissingPrefabWarning = true;
        }
    }
}
