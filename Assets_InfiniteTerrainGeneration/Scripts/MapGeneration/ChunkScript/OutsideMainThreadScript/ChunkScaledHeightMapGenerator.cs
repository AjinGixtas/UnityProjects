using UnityEngine;

public static class ChunkScaledHeightMapGenerator {
    public static float[,] GetScaledHeightMap(float[,] elevationMap, Keyframe[] heightConversionMapKeys, float globalHeightMultiplier) {
        AnimationCurve heightConversionMap = new(heightConversionMapKeys);
        int width = elevationMap.GetLength(0), height = elevationMap.GetLength(1);
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                elevationMap[x, y] = heightConversionMap.Evaluate(elevationMap[x, y]) * globalHeightMultiplier;
            }
        }
        return elevationMap;
    }
}
