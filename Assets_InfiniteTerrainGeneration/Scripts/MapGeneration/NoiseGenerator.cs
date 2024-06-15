using System;
using System.Drawing;
using UnityEngine;

public static class NoiseGenerator {
    public static float[,] GeneratePerlinNoiseMap(int width, int height, float scale, int octave, float persistance, float lacunarity, int seed, Vector2 offset) {


        if (width < 1) { Debug.LogError("Width is smaller than 1"); }
        if (height < 1) { Debug.LogError("Height is smaller than 1"); }
        if (scale <= 0) { Debug.LogError("Scale must be bigger than 0"); }
        if (octave < 1) { Debug.LogError("There must be at least 1 layer of octave"); }
        if (persistance <= 0) { Debug.LogError("Persistance must be bigger than 0"); }
        if (lacunarity < 0) { Debug.LogError("Lacunarity must be bigger or equal to 0"); }
        if (width * height * octave > 655360) { Debug.LogWarning("The total amount of compute iteration (width x height x octave) is exceed 256x256x10, this may lead to performance issue"); }

        try {
            float _0 = (width - 1) / scale * (float)Math.Pow(persistance, octave) + short.MaxValue + offset.x;
            float _1 = (height - 1) / scale * (float)Math.Pow(persistance, octave) + short.MaxValue + offset.y;
            float _2 = (width - 1) / scale * (float)Math.Pow(persistance, octave) + short.MinValue + offset.x;
            float _3 = (height - 1) / scale * (float)Math.Pow(persistance, octave) + short.MinValue + offset.y;
        } catch (OverflowException) { Debug.LogError("The offset is far too big, please consider lowering it to a smaller value"); }

        float halfHeight = (height - 1) / 2f, halfWidth = (width - 1) / 2f;

        float[,] noiseMap = new float[width, height];
        Vector2[] octaveOffsets = new Vector2[octave];

        System.Random randomNumberGenerator = new(seed);
        float maxPossibleValue = 0;
        float amplitude = 1, frequency;
        for (int i = 0; i < octave; i++) {
            int offsetX = randomNumberGenerator.Next(short.MinValue, short.MaxValue);
            int offsetY = randomNumberGenerator.Next(short.MinValue, short.MaxValue);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            maxPossibleValue += amplitude;
            amplitude *= persistance;
        }
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                amplitude = 1;
                frequency = 1;
                float noiseResult = 0;

                for (int i = 0; i < octave; i++) {

                    float xPos = (x - halfWidth + offset.x) / scale * frequency + octaveOffsets[i].x;
                    float yPos = (y - halfHeight + offset.y) / scale * frequency + octaveOffsets[i].y;
                    float noiseValue = Mathf.PerlinNoise(xPos, yPos);
                    noiseValue *= amplitude;
                    noiseResult += noiseValue;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                noiseMap[x, y] = noiseResult;
            }
        }
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(0, maxPossibleValue, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
    public static float GenerateSamplePerlinNoise(int octave, float persistance, float lacunarity, int seed, Vector2 offset) {
        if (octave < 1) { Debug.LogError("There must be at least 1 layer of octave"); }
        if (persistance <= 0) { Debug.LogError("Persistance must be bigger than 0"); }
        if (lacunarity < 0) { Debug.LogError("Lacunarity must be bigger or equal to 0"); }

        System.Random randomNumberGenerator = new(seed);
        Vector2[] octaveOffsets = new Vector2[octave];
        for (int i = 0; i < octave; i++) {
            int offsetX = randomNumberGenerator.Next(short.MinValue, short.MaxValue);
            int offsetY = randomNumberGenerator.Next(short.MinValue, short.MaxValue);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        float amplitude = 1, frequency = 1;
        float noiseResult = 0;

        for (int i = 0; i < octave; i++) {

            float xPos = octaveOffsets[i].x + offset.x;
            float yPos = octaveOffsets[i].y + offset.y;
            float noiseValue = Mathf.PerlinNoise(xPos, yPos) * 2 - 1;

            noiseResult += noiseValue;

            amplitude *= persistance;
            frequency *= lacunarity;
        }
        return (noiseResult + 1) / 2;
    }
    public static int[,] GenerateColoredWorleyNoiseMap(
        int width, int height,
        float cellWidth, float cellHeight,
        float scale, int colorCount,
        Vector2 offset) {

        int[,] coloredWorleyNoiseMap = new int[width, height];

        float gapBetweenSample = 1 / scale;
        float halfWidth = (width - 1) / 2f, halfHeight = (height - 1) / 2f;
        float halfCellWidth = (cellWidth - 1) / 2f, halfCellHeight = (cellHeight - 1) / 2f;

        Vector2 topRightSampleStart = new((-halfWidth + offset.x) / scale, (-halfHeight + offset.y) / scale);
        Vector2 bottomLeftSampleLimit = new((halfWidth + offset.x) / scale, (halfHeight + offset.y) / scale);

        Vector2Int topRightChunkStart = new(
            CalculateChunkPos(topRightSampleStart.x, halfCellWidth) - 1,
            CalculateChunkPos(topRightSampleStart.y, halfCellHeight) - 1);
        Vector2Int bottomLeftChunkLimit = new(
            CalculateChunkPos(bottomLeftSampleLimit.x, halfCellWidth) + 3,
            CalculateChunkPos(bottomLeftSampleLimit.y, halfCellHeight) + 3);

        WorleyPointPositionDataInGrid[,] worleyPointPositionDataInGrids =
            new WorleyPointPositionDataInGrid[
            bottomLeftChunkLimit.x - topRightChunkStart.x,
            bottomLeftChunkLimit.y - topRightChunkStart.y];

        for (int x = 0; x < worleyPointPositionDataInGrids.GetLength(0); x++) {
            for (int y = 0; y < worleyPointPositionDataInGrids.GetLength(1); y++) {

                worleyPointPositionDataInGrids[x, y] =
                    new(topRightChunkStart.x + x, topRightChunkStart.y + y,
                    cellWidth, cellHeight,
                    halfCellWidth, halfCellHeight,
                    colorCount);

            }
        }

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float xPosSample = x * gapBetweenSample,
                    yPosSample = y * gapBetweenSample;
                int xPosChunk = CalculateChunkPos(xPosSample, halfCellWidth) + 1,
                    yPosChunk = CalculateChunkPos(yPosSample, halfCellHeight) + 1;
                xPosSample += topRightSampleStart.x;
                yPosSample += topRightSampleStart.y;
                float shortestDistance = float.PositiveInfinity;
                float distance;
                int candidateValue = -1;
                for (int xOffset = -1; xOffset <= 1; xOffset++) {
                    for (int yOffset = -1; yOffset <= 1; yOffset++) {
                        WorleyPointPositionDataInGrid point =
                            worleyPointPositionDataInGrids[xPosChunk + xOffset, yPosChunk + yOffset];
                        distance = Mathf.Pow(point.x - xPosSample, 2) + Mathf.Pow(point.y - yPosSample, 2);
                        if (shortestDistance < distance) { continue; }
                        candidateValue = point.colorIndex;
                        shortestDistance = distance;
                    }
                }
                coloredWorleyNoiseMap[x, y] = candidateValue;
            }
        }
        return coloredWorleyNoiseMap;
        static int CalculateChunkPos(float pos, float halfChunkSize) {
            int result;
            result = RoundAwayZero(pos / halfChunkSize);
            result = RoundTowardZero(result / 2);
            return result;
        }
    }
    static int RoundAwayZero(float f) {
        if (f > 0) return Mathf.CeilToInt(f);
        return Mathf.FloorToInt(f);
    }
    static int RoundTowardZero(float f) {
        if (f < 0) return Mathf.CeilToInt(f);
        return Mathf.FloorToInt(f);
    }
}
[Serializable]
public struct SamplePerlinNoiseData {
    public int octave;
    public float lacunarity, persistance;
    public int seed;
    public Vector2 offset;
    public SamplePerlinNoiseData(int octave, float lacunarity, float persistance, int seed, Vector2 offset) {
        this.octave = octave;
        this.lacunarity = lacunarity;
        this.persistance = persistance;
        this.seed = seed;
        this.offset = offset;
    }
}
public struct WorleyPointPositionDataInGrid {
    public float x, y;
    public int colorIndex;
    public WorleyPointPositionDataInGrid(int gridPosX, int gridPosY,
        float width, float height,
        float halfWidth, float halfHeight,
        int colorCount) {

        System.Random rng = new((gridPosX << 16) | (gridPosY & 0xFFFF));
        x = (float)(rng.NextDouble() + gridPosX) * width - halfWidth;
        y = (float)(rng.NextDouble() + gridPosY) * height - halfHeight;
        colorIndex = rng.Next(0, colorCount);
    }
}