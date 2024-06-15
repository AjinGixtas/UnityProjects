using UnityEngine;
public static class ColorGenerator {
    public static Color[] GenerateMonochromeColor(float[,] noiseMap) {
        int width = noiseMap.GetLength(0), height = noiseMap.GetLength(1);
        Color[] result = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                result[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        return result;
    }
    public static Color[] GeneratePolychromaticColor(float[,] noiseMap, TerrainData[] terrainDatas) {
        int width = noiseMap.GetLength(0), height = noiseMap.GetLength(1);
        Color[] result = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                for(int i = 0; i< terrainDatas.Length; i++) {
                    if (terrainDatas[i].terrainHeight > noiseMap[x,y]) { continue; }
                    result[y * width + x] = terrainDatas[i].terrainColor;
                }
            }
        }
        return result;
    }
}
