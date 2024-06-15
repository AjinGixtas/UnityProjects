using UnityEngine;

public class ChunkIntializationDataGenerator : MonoBehaviour {
    // DO NOT FUCKING MODIFY **ANY** OF THESE VARIABLE DURING RUN-TIME
    // YES! ALL OF THEM, IF YOU EVEN DARE TO TOUCH ANY OF THEM, I'M GONNA SLIT YOUR FUCKING THROAT!
    // Wait... You are me...
    // I'm gonna kill myself if I touch any of these variable during run-time
    public PerlinNoiseMapData humidityMapData, temperatureMapData, elevationMapData, erosionMapData;
    public BiomeData[] biomeDatas;

    public float globalHeightMultiplier;
    public float seaLevel;
    public float minHeightForResource;
    public GameObject seaObject;
    public AnimationCurve heightMapConversion;
    public Keyframe[] heightMapConversionKeyframe;
    public int biomeSize;
    public int gridSizeOfResoure;
    public ColoredWorleyNoise biomeGenerator;
    public ColoredWorleyNoise.DistanceCalculationMethod distanceCalculationMethod;
    public ColorRangeToLerpBasedOnHeight[] colorRangeToLerpBasedOnHeights;
    public void Start() {
        biomeGenerator = new(biomeSize, biomeDatas.Length);
        heightMapConversionKeyframe = heightMapConversion.keys;
        seaObject.transform.position = new(0, heightMapConversion.Evaluate(seaLevel) * globalHeightMultiplier, 0);
    }
    public ChunkIntializationData GenerateChunkIntializationData(Vector2Int offsetFromCenter,
        int chunkSize, float halfChunkSize, int colliderLOD, int terrainLOD) {
        float minHeight = heightMapConversion.Evaluate(minHeightForResource) * globalHeightMultiplier;
        int[,] biomeMap = ChunkBiomeMapGenerator.GenerateChunkBiomeMap(chunkSize - 1, chunkSize - 1,
            biomeGenerator, distanceCalculationMethod,
            offsetFromCenter * (chunkSize - 1));

        float[,] elevationMap = NoiseGenerator.GeneratePerlinNoiseMap(chunkSize, chunkSize,
            elevationMapData.scale, elevationMapData.octave,
            elevationMapData.persistance, elevationMapData.lacunarity,
            elevationMapData.seed, offsetFromCenter * (chunkSize - 1));
        Color[] colorMapBasedOnBiome = ChunkColorMapGenerator.
    GenerateChunkColorMapBasedOnBiome(biomeMap, biomeDatas);

        Color[] colorMapAddedHeightEvaluation = ChunkColorMapGenerator.
            AddChunkColorMapBasedOnElevation(colorMapBasedOnBiome, elevationMap,
            colorRangeToLerpBasedOnHeights);
        float[,] heightMap = ChunkScaledHeightMapGenerator.
            GetScaledHeightMap(elevationMap, heightMapConversionKeyframe, globalHeightMultiplier);
        int[,] resourceMap = ChunkResourceMapGenerator.
            GenerateChunkResourceMap(biomeMap, offsetFromCenter, biomeDatas, gridSizeOfResoure);

        MeshData terrainMeshData = ChunkMeshDataGenerator.
            GenerateMeshData(heightMap, terrainLOD);

        if (terrainLOD == colliderLOD || colliderLOD == -1) {
            return new ChunkIntializationData(heightMap, minHeight,
                resourceMap,
            colorMapAddedHeightEvaluation,
            terrainMeshData, terrainMeshData) ;
        }

        MeshData colliderMeshData = ChunkMeshDataGenerator.
            GenerateMeshData(heightMap, colliderLOD);

        return new ChunkIntializationData(heightMap, minHeight,
            resourceMap,
            colorMapAddedHeightEvaluation,
            terrainMeshData, colliderMeshData) ;
    }
    void Print2DArray(float[,] arr) {
        string str = "";
        for (int x = 0; x < arr.GetLength(0); x++) {
            for (int y = 0; y < arr.GetLength(1); y++) {
                str += $"{arr[x, y]} ";
            }
        }
        Debug.Log(str);
    }
}
public class ChunkIntializationData {
    public float[,] heightMap;
    public float minHeight;
    public int[,] resourceMap;
    public Color[] colorMap;
    public MeshData terrainData;
    public MeshData colliderMeshData;
    public ChunkIntializationData(float[,] heightMap, float minHeight, int[,] resourceMap, Color[] colorMap, MeshData terrainData, MeshData colliderMeshData) {
        this.heightMap = heightMap;
        this.minHeight = minHeight;
        this.resourceMap = resourceMap;
        this.colorMap = colorMap;
        this.terrainData = terrainData;
        this.colliderMeshData = colliderMeshData;
    }
}