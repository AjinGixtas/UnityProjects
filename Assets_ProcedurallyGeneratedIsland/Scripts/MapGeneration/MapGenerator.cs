using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
public class MapGenerator : MonoBehaviour {
    /*
     * In popular implementation of Perlin noise, there's also a parameter called "seed".
     * However, the way System.Random.Next() work and how seed affect the outcome is quite
     * confusing so I've decided to not implement it into my Perlin noise generator.
     *
     * Perlin noise research paper: 
     * -> https://mrl.cs.nyu.edu/~perlin/paper445.pdf
     * Wikipedia article on Perlin noise: 
     * -> https://en.wikipedia.org/wiki/Perlin_noise
     * Code shamelessly stole from Sebastian Lague: 
     * -> https://github.com/SebLague/Procedural-Landmass-Generation
     * Perlin noise series by Sebastian Lague: 
     * -> https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
     * Explaination of Perlin noise and an implmentation of it in Javascript by Daniel Shiffman 
     * from the Youtube channel "The Coding Train": 
     * -> https://www.youtube.com/watch?v=IKB1hWWedMk
     */
    public enum MeshDrawMode { PLANAR = 0, TERRAIN = 1 }
    public enum MeshPaintMode { MONOCHROME = 0, POLYCHROMATIC = 1 }
    public enum MeshColorDistribution { DEFAULT = 0, ISLAND = 1 }
    public MeshDrawMode meshDrawMode;
    public int levelOfDetail;
    public float globalHeightMultiplier;
    public AnimationCurve heightMultiplier;

    public MeshPaintMode meshPaintMode;
    public TerrainData[] terrainDatas;
    public TerrainRenderer terrainRenderer;

    public MeshColorDistribution meshColorDistribution;
    public AnimationCurve islandColorDistribution;

    public int width, height;
    public float scale;
    public int octave;
    public float persistance;
    [Range(0, 1)]
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public bool newSeed;
    public bool autoUpdate;
    public bool debugMode;
    public bool generateResource;

    public ResourceRenderer resourceRenderer;
    public ResourceType[] resourceTypes;

    public GameObject sea;
    public float seaLevel;

    public void GenerateMapInEditor() {
        if (newSeed) { seed = Random.Range(-2147483648, 2147483647); }
        //Generate the basic terrain and color it
        Mesh mesh; Color[] colors; Mesh colliderMesh; float[,] meshHeightMap;

        float[,] noiseMap = NoiseGenerator.GenerateNoise(width, height, scale, octave, persistance, lacunarity, seed, offset);

        if (meshColorDistribution == MeshColorDistribution.ISLAND && width == height) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    float distanceNormalized = CalculateDistanceToNearestEdge(x, y, width, height) / Mathf.Max(width, height);
                    float multiplier = islandColorDistribution.Evaluate(distanceNormalized);
                    noiseMap[x, y] *= multiplier;
                }
            }
        }

        if (meshDrawMode == MeshDrawMode.PLANAR) {
            MeshData meshData = MeshDataGenerator.GenerateTerrainMesh(noiseMap, 0, heightMultiplier, levelOfDetail);
            mesh = meshData.mesh;
            meshHeightMap = meshData.meshHeightMap;
        } else {
            MeshData meshData = MeshDataGenerator.GenerateTerrainMesh(noiseMap, globalHeightMultiplier, heightMultiplier, levelOfDetail);
            mesh = meshData.mesh;
            meshHeightMap = meshData.meshHeightMap;
            colliderMesh = MeshDataGenerator.GenerateTerrainMesh(noiseMap, globalHeightMultiplier, heightMultiplier, levelOfDetail + 1).mesh;
            terrainRenderer.DrawCollider(colliderMesh);
        }
        if (meshPaintMode == MeshPaintMode.MONOCHROME) {
            colors = ColorGenerator.GenerateMonochromeColor(noiseMap);
            Texture2D texture2D = TextureGenerator.GenerateTexture(colors, width, height);
            terrainRenderer.DrawTexture(mesh, texture2D);
        } else {
            colors = ColorGenerator.GeneratePolychromaticColor(noiseMap, terrainDatas);
            Texture2D texture2D = TextureGenerator.GenerateTexture(colors, width, height);
            terrainRenderer.DrawMesh(mesh, texture2D);
        }

        if (generateResource) {
            int[,] resourceMap = ResourcePositionGenerator.GenerateResourcePositionMap(noiseMap, ref resourceTypes);
            resourceRenderer.RenderResourceObject(resourceMap, meshHeightMap, resourceTypes, seed);
            resourceRenderer.RenderResource(resourceMap, resourceTypes);
        } else {
            resourceRenderer.KillAllChild();
        }
    }
    void OnValidate() {
        if (width < 1) width = 1;
        if (height < 1) height = 1;
        if (scale < 1) scale = 1;
        if (octave < 1) octave = 1;
        if (persistance < 1) persistance = 1;
        if (levelOfDetail != 0 && (width != 241 || height != 241)) { Debug.LogWarning("Mesh must have width and height equal to 241 in order to have levelOfDetail work correctly!"); }
        sea.transform.position = new(sea.transform.position.x, seaLevel * globalHeightMultiplier * heightMultiplier.keys[^1].value, sea.transform.position.z);
    }
    private float CalculateDistanceToNearestEdge(int x, int y, int width, int height) {
        float distanceLeft = x;
        float distanceRight = width - x - 1;
        float distanceTop = y;
        float distanceBottom = height - y - 1;

        return Mathf.Min(distanceLeft, distanceRight, distanceTop, distanceBottom);
    }
    void Start() { GenerateMapInEditor(); }
}
[Serializable]
public struct TerrainData {
    public string terrainName;
    [Range(0, 1)]
    public float terrainHeight;
    public Color terrainColor;
    public TerrainData(string terrainName, float terrainHeight, Color terrainColor) {
        this.terrainName = terrainName;
        this.terrainHeight = terrainHeight;
        this.terrainColor = terrainColor;
    }
}
[Serializable]
public struct ResourceType {
    public string resourceName;
    [Range(0, 1)]
    public float minHeight, maxHeight;

    public int octave;
    public float persistance;
    [Range(0, 1)]
    public float lacunarity;
    public float scale;

    // When generate the map for this resource to spawn, if this variable bigger than that position on the map, this resource will be able to spawn
    [Range(0, 1)]
    public float resourceSpawnRange;
    [Range(0, 1)]
    public float resourceDensity;

    public bool randomSeed;
    public int seed;

    public int amountOfResourceOnMap;

    public Color resourceColor;
    public GameObject resourcePrefab;

    public ResourceType(string resourceName, float minHeight, float maxHeight, int octave, float persistance, float lacunarity, float scale, float resourceSpawnRange, float resourceDensity, bool randomSeed, int seed, int amountOfResourceOnMap, Color resourceColor, GameObject resourcePrefab) {
        this.resourceName = resourceName;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.octave = octave;
        this.persistance = persistance;
        this.lacunarity = lacunarity;
        this.scale = scale;
        this.resourceSpawnRange = resourceSpawnRange;
        this.resourceDensity = resourceDensity;
        this.randomSeed = randomSeed;
        this.seed = seed;
        this.amountOfResourceOnMap = amountOfResourceOnMap;
        this.resourceColor = resourceColor;
        this.resourcePrefab = resourcePrefab;
    }
}
