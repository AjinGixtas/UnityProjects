using UnityEngine;
public static class ChunkBiomeMapGenerator
{
    public static int[,] GenerateChunkBiomeMap(int width, int height, ColoredWorleyNoise biomeGenerator, ColoredWorleyNoise.DistanceCalculationMethod distanceCalculationMethod, Vector2 offsetFromCenter) {
        int[,] chunkBiomeMap = new int[width,height];
        
        for(int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                chunkBiomeMap[x, y] = biomeGenerator.GetSampleColoredWorleyNoise(x + offsetFromCenter.x - width / 2, y + offsetFromCenter.y - height / 2, distanceCalculationMethod);
            }
        }
        return chunkBiomeMap;
    }
}
