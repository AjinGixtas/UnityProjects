using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Runtime.InteropServices;

public class ChunkManager : MonoBehaviour {
    public ChunkIntializationDataGenerator chunkIntializationDataGenerator;
    public ChunkResourceRenderer chunkResourceRenderer;
    public Transform chunkContainer;
    public GameObject player;
    public int chunkRenderDistance;
    public int chunkSize;
    public float halfChunkSize;
    public Dictionary<Vector2, ChunkData> exploredChunk = new();
    public ChunkData[] activeChunkLastIteration;
    public GameObject seaObject;
    public int[,] LOD_map_of_chunk_terrain = new int[,] {
        { 0, 0, 1, 2, 3, 4 },
        { 0, 0, 1, 2, 3, 4 },
        { 1, 1, 2, 3, 4, 4 },
        { 2, 2, 3, 3, 4, 4 },
        { 3, 3, 4, 4, 4, 4 },
        { 4, 4, 4, 4, 4, 4 }
    };
    public int[,] LOD_map_of_chunk_collider = new int[,] {
        { 1, 1, 2, 3, 4, -1 },
        { 1, 1, 2, 3, 4, -1 },
        { 2, 2, 3, 4, 4, -1 },
        { 3, 3, 4, 4, 4, -1 },
        { 4, 4, 4, 4, 4, -1 },
        { -1, -1, -1, -1, -1, -1}
    };
    void Start() {
        halfChunkSize = chunkSize / 2f;
        activeChunkLastIteration = new ChunkData[(int)Mathf.Pow(chunkRenderDistance + chunkRenderDistance + 1, 2)];
        InvokeRepeating(nameof(RenderNearbyChunk), 0f, 5f);
    }
    void ClearPreviousChunkIteration() {
        for (int i = 0; i < activeChunkLastIteration.Length; i++) {
            if (activeChunkLastIteration[i] == null) { continue; }
            activeChunkLastIteration[i].SetChunkVisibility(false);
            activeChunkLastIteration[i] = null;
        }
    }
    void RenderNearbyChunk() {
        ClearPreviousChunkIteration();
        int xLayer = RoundAwayFromZero(player.GetComponent<Transform>().position.x / (halfChunkSize - 0.5f)),
            yLayer = RoundAwayFromZero(player.GetComponent<Transform>().position.z / (halfChunkSize - 0.5f));
        int xChunk = RoundTowardZero(xLayer / 2f), yChunk = RoundTowardZero(yLayer / 2f);
        seaObject.transform.position = new Vector3(xChunk * (chunkSize - 1), seaObject.transform.position.y, yChunk * (chunkSize - 1));
        Vector2Int chunkKeyThatPlayerStand = new(xChunk, yChunk);
        for (int x = -chunkRenderDistance, i = -1; x <= chunkRenderDistance; x++) {
            for (int y = -chunkRenderDistance; y <= chunkRenderDistance; y++) {
                i++;

                Vector2Int chunkKey = chunkKeyThatPlayerStand + new Vector2Int(x, y);
                Vector2Int chunkPos = new(chunkKey.x, -chunkKey.y);
                if (exploredChunk.ContainsKey(chunkPos)) {
                    activeChunkLastIteration[i] = exploredChunk[chunkPos];
                        exploredChunk[chunkPos].SetChunkVisibility(true,
                        GetColliderLOD(Math.Abs(x), Math.Abs(y)),
                        GetTerrainLOD(Math.Abs(x), Math.Abs(y)));
                    continue;
                }
                activeChunkLastIteration[i] = new(chunkKey,
                    chunkResourceRenderer,
                    chunkIntializationDataGenerator,
                    chunkContainer, chunkSize, halfChunkSize,
                    GetColliderLOD(Math.Abs(x), Math.Abs(y)),
                    GetTerrainLOD(Math.Abs(x), Math.Abs(y)));
                exploredChunk.Add(chunkPos, activeChunkLastIteration[i]);
            }
        }
    }
    int GetColliderLOD(int x, int y) {
        if (x >= LOD_map_of_chunk_collider.GetLength(0)
            || y >= LOD_map_of_chunk_collider.GetLength(0)) { return -1; }
        return LOD_map_of_chunk_collider[x, y];
    }
    int GetTerrainLOD(int x, int y) {
        if (x >= LOD_map_of_chunk_terrain.GetLength(0)
        || y >= LOD_map_of_chunk_terrain.GetLength(0)) { return 4; }
        return LOD_map_of_chunk_terrain[x, y];
    }
    int RoundTowardZero(float f) {
        if (f > 0) { return Mathf.FloorToInt(f); }
        return Mathf.CeilToInt(f);
    }
    int RoundAwayFromZero(float f) {
        if (f > 0) { return Mathf.CeilToInt(f); }
        return Mathf.FloorToInt(f);
    }
    public class ChunkData {
        public GameObject chunk;
        public ChunkIntializationData chunkIntializationData;
        public ChunkResourceRenderer chunkResourceRenderer;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        public Vector2Int offsetFromCenter;
        public int chunkSize;
        public float halfChunkSize;
        public Mesh[] meshCache = new Mesh[5];
        public int[] meshRequestState = new int[5];
        // 0 -> Haven't requested yet
        // 1 -> Have requested mesh and are now waiting
        // 2 -> Have requested mesh and have recieved it
        public ChunkData(Vector2Int chunkKey,
            ChunkResourceRenderer chunkResourceRenderer,
            ChunkIntializationDataGenerator chunkIntializationDataGenerator,
            Transform chunkContainer,
            int chunkSize, float halfChunkSize,
            int colliderLOD, int terrainLOD) {
            chunk = GameObject.CreatePrimitive(PrimitiveType.Plane);
            chunk.name = $"X_{chunkKey.x}_Y_{chunkKey.y}";
            chunk.transform.parent = chunkContainer;
            chunk.transform.position = new Vector3(chunkKey.x, 0, chunkKey.y) * (chunkSize - 1);
            meshRenderer = chunk.GetComponent<MeshRenderer>();
            meshFilter = chunk.GetComponent<MeshFilter>();
            meshCollider = chunk.GetComponent<MeshCollider>();

            this.chunkSize = chunkSize;
            this.halfChunkSize = halfChunkSize;
            this.chunkResourceRenderer = chunkResourceRenderer;
            this.offsetFromCenter = chunkKey;


            Thread childThread = new(
                () =>
                IntializeChunkData(chunkIntializationDataGenerator,
                new(chunkKey.x, -chunkKey.y), chunkSize, halfChunkSize, colliderLOD, terrainLOD)
                );
            childThread.Start();
        }
        void IntializeChunkData(
            ChunkIntializationDataGenerator chunkIntializationDataGenerator,
            Vector2Int offsetFromCenter,
            int chunkSize, float halfChunkSize,
            int colliderLOD, int terrainLOD) {
            chunkIntializationData = chunkIntializationDataGenerator.
                GenerateChunkIntializationData(offsetFromCenter,
                chunkSize, halfChunkSize,
                colliderLOD, terrainLOD);
            UnityMainThreadDispatcher.Instance().Enqueue(() => RenderChunk(colliderLOD, terrainLOD, chunkIntializationData.minHeight));
        }
        void RenderChunk(int colliderLOD, int terrainLOD, float minHeightForResource) {
            // Cache these variable for future uses
            if (colliderLOD != -1) {
                meshCache[colliderLOD] = chunkIntializationData.colliderMeshData.CreateMesh();
                meshCollider.sharedMesh = meshCache[colliderLOD];
            }
            meshCache[terrainLOD] = chunkIntializationData.terrainData.CreateMesh();


            meshRenderer.material.mainTexture =
                ChunkTextureGenerator.GenerateChunkTexture(
                    chunkSize - 1, chunkSize - 1,
                    chunkIntializationData.colorMap);
            meshFilter.mesh = meshCache[terrainLOD];
            chunkResourceRenderer.RenderChunkResource(
                chunkIntializationData.resourceMap, 
                chunkIntializationData.heightMap, 
                minHeightForResource, offsetFromCenter, 
                chunk.transform);
        }
        public void SetChunkVisibility(bool isVisible,
            [Optional] int colliderLOD,
            [Optional] int terrainLOD) {
            if (isVisible) {
                SetTerrainMesh(terrainLOD);
                if (colliderLOD != -1) { SetColliderMesh(colliderLOD); }
            }
            chunk.SetActive(isVisible);
        }
        public void SetTerrainMesh(int LOD) {
            if (meshCache[LOD] == null && meshRequestState[LOD] == 0) {
                Thread childThread = new(() => GenerateMeshOnChildThread(LOD, SetAndGetCacheMesh, SetTerrainMesh));
                childThread.Start();
                return;
            }
            meshFilter.mesh = meshCache[LOD];
        }
        public void SetColliderMesh(int LOD) {
            if (meshCache[LOD] == null && meshRequestState[LOD] == 0) {
                Thread childThread = new(() => GenerateMeshOnChildThread(LOD, SetAndGetCacheMesh, SetColliderMesh));
                childThread.Start();
                return;
            }
            meshCollider.sharedMesh = meshCache[LOD];
        }
        public void GenerateMeshOnChildThread(int LOD, Action<MeshData, int, Action<int>> callback, Action<int> callBackParam) {
            meshRequestState[LOD] = 1;
            MeshData meshData = ChunkMeshDataGenerator.
                GenerateMeshData(chunkIntializationData.heightMap, LOD);
            meshRequestState[LOD] = 2;
            UnityMainThreadDispatcher.Instance().Enqueue(() => callback(meshData, LOD, callBackParam));
        }
        public void SetAndGetCacheMesh(MeshData meshData, int LOD, Action<int> action) {
            meshCache[LOD] = meshData.CreateMesh();
            action(LOD);
        }
    }
}
