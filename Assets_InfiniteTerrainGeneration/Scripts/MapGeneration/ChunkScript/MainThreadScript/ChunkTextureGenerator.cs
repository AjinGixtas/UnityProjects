using UnityEngine;
public static class ChunkTextureGenerator {
    public static Texture2D GenerateChunkTexture(int width, int height, Color[] terrainColorMap) {
        Texture2D texture = new(width, height) {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels(terrainColorMap);
        texture.Apply();
        return texture;
    }
}
