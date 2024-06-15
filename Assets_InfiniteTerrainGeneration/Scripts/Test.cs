using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class Test : MonoBehaviour {
    public PerlinNoiseMapData perlinNoiseMapData;
    public AnimationCurve foo;
    public GameObject thisTile;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public int width, height;
    public bool autoUpdate;
    public bool mode;
    public Color[] duh;
    public Vector2 offset;
    public ColoredWorleyNoise.DistanceCalculationMethod distanceCalculationMethod;
    public uint p;
    public int size;
    public float scale;
    void Update() { if (autoUpdate) RenderPerlinMap(); }
    public void RenderPerlinMap() {
        //Debug.Log("Updated!");
        //float[,] noiseMap = NoiseGenerator.GeneratePerlinNoiseMap(perlinNoiseMapData.width, perlinNoiseMapData.height, perlinNoiseMapData.scale,
        //  perlinNoiseMapData.octave, perlinNoiseMapData.persistance, perlinNoiseMapData.lacunarity, perlinNoiseMapData.seed,
        //perlinNoiseMapData.offset);
        Color[] colorMap = new Color[width * height];
        if (mode) {
            ColoredWorleyNoise foo = new(size, duh.Length);
            for (int i = 0, x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    // colorMap[i] = duh[noiseValue[x, y]];
                    colorMap[i] = duh[foo.GetSampleColoredWorleyNoise((x + offset.x) / scale, (y + offset.y) / scale, distanceCalculationMethod, p)];
                    i++;
                }
            }
        } else {
            int[,] noiseValue = NoiseGenerator.GenerateColoredWorleyNoiseMap(width, height, size, size, scale, duh.Length, offset);

            for (int i = 0, x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    colorMap[i] = duh[noiseValue[x, y]];
                    i++;
                }
            }
        }
        Texture2D texture2D = ChunkTextureGenerator.GenerateChunkTexture(width, height, colorMap);
        thisTile.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture2D;
    }
}

[CustomEditor(typeof(Test))]
public class TestEditor : Editor {
    public override void OnInspectorGUI() {
        Test mapGen = (Test)target;

        if (DrawDefaultInspector()) {
            if (mapGen.autoUpdate) {
                mapGen.RenderPerlinMap();
            }
        }

        if (GUILayout.Button("Generate")) {
            mapGen.RenderPerlinMap();
        }
    }
}
