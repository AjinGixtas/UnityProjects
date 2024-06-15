using System;
using UnityEngine;
public static class ChunkColorMapGenerator {
    public static Color[] GenerateChunkColorMapBasedOnBiome(int[,] biomeChunkMap, BiomeData[] biomeDatas) {
        int width = biomeChunkMap.GetLength(0), height = biomeChunkMap.GetLength(1);
        Color[] chunkColorMap = new Color[width * height];
        for (int i = 0, y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                chunkColorMap[i] = biomeDatas[biomeChunkMap[x, y]].biomeGroundColor;
                i++;
            }
        }
        return chunkColorMap;
    }
    public static Color[] AddChunkColorMapBasedOnElevation(Color[] chunkColorMap, float[,] elevationMap, ColorRangeToLerpBasedOnHeight[] colorRangeToLerps) {
        int width = elevationMap.GetLength(0) - 1, height = elevationMap.GetLength(1) - 1;
        AnimationCurve[] lerpMapForAllColor = new AnimationCurve[colorRangeToLerps.Length];
        float elevationOfPoint;
        for (int i = 0; i < colorRangeToLerps.Length; i++) {
            lerpMapForAllColor[i] = new AnimationCurve(colorRangeToLerps[i].lerpMap.keys);
        }
        for (int i = -1, y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                elevationOfPoint = (
                    elevationMap[x, y]
                    + elevationMap[x + 1, y]
                    + elevationMap[x, y + 1]
                    + elevationMap[x + 1, y + 1]
                    ) / 4;

                i++;
                for (int j = 0; j < colorRangeToLerps.Length; j++) {
                    if (colorRangeToLerps[j].minHeight > elevationOfPoint) { continue; }
                    if (colorRangeToLerps[j].maxHeight < elevationOfPoint) { continue; }
                    chunkColorMap[i] = Color.Lerp(
                        chunkColorMap[i],
                        colorRangeToLerps[j].color,
                        lerpMapForAllColor[j].Evaluate(elevationOfPoint)
                    );
                    break;
                }
            }
        }
        return chunkColorMap;
    }
    public static Color[] GenerateChunkColorMapBasedOnHeightMap(float[,] heightMap, BiomeData[] biomeDatas) {
        int width = heightMap.GetLength(0), height = heightMap.GetLength(1);
        Color[] chunkColorMap = new Color[width * height];
        for (int i = 0, x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                chunkColorMap[i] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                i++;
            }
        }
        return chunkColorMap;
    }

}
[Serializable]
public struct ColorRangeToLerpBasedOnHeight {
    public Color color;
    public float minHeight, maxHeight;
    public AnimationCurve lerpMap;
    public ColorRangeToLerpBasedOnHeight(Color color, float minHeight, float maxHeight, AnimationCurve lerpMap) {
        this.color = color;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.lerpMap = lerpMap;
    }
}
