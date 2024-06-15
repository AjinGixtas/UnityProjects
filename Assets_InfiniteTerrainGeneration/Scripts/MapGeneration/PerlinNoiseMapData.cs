using UnityEngine;

[CreateAssetMenu(fileName = "PerlinNoiseMapData")]
public class PerlinNoiseMapData : ScriptableObject {
    public int width, height;
    public float scale;
    public int octave;
    public float lacunarity;
    public float persistance;
    public int seed;
    public Vector2 offset;
    public PerlinNoiseMapData(int width, int height, float scale, int octave, float lacunarity, float persistance, int seed, Vector2 offset) {
        this.width = width;
        this.height = height;
        this.scale = scale;
        this.octave = octave;
        this.lacunarity = lacunarity;
        this.persistance = persistance;
        this.seed = seed;
        this.offset = offset;
    }
}