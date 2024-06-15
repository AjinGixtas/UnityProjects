using UnityEngine;
public static class TextureGenerator {
    public static Texture2D GenerateTexture(Color[] colors, int width, int height) {
        Texture2D texture = new(width, height) {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
}
