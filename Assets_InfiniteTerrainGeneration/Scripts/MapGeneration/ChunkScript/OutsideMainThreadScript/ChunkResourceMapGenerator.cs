using System;
using System.Collections.Generic;
using UnityEngine;
public static class ChunkResourceMapGenerator {
    public static int[,] GenerateChunkResourceMap(int[,] biomeMap, Vector2Int offsetFromCenter, BiomeData[] biomeDatas, int gridSize) {
        System.Random rng = new(offsetFromCenter.x << 16 | offsetFromCenter.y);
        int width = biomeMap.GetLength(0), height = biomeMap.GetLength(1);
        float halfWidth = width / 2f, halfHeight = height / 2f;
        int[,] chunkResourceMap = new int[width, height];
        for (int x = 0; x < width; x += gridSize) {
            for (int y = 0; y < height; y += gridSize) {
                double rollValue = rng.NextDouble();
                int xPos = x + rng.Next(0, gridSize);
                int yPos = y + rng.Next(0, gridSize);
                for (int i = 0; i < biomeDatas[biomeMap[xPos, yPos]]
                    .resourceSpawnInfoInBiome
                    .Length; i++) {
                    if (rollValue < biomeDatas[biomeMap[xPos, yPos]]
                        .resourceSpawnInfoInBiome[i]
                        .chanceMinThreshold) { continue; }
                    chunkResourceMap[xPos, yPos] = biomeDatas[biomeMap[xPos, yPos]]
                        .resourceSpawnInfoInBiome[i].resourceData.resourceIndex;
                }
            }
        }
        return chunkResourceMap;
    }
}
[Serializable]
public struct ResourceSpawnInfo {
    public ResourceData resourceData;
    public float chanceMinThreshold;
}
