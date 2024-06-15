using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
public class ColoredWorleyNoise {
    public enum DistanceCalculationMethod { EuclideanDistance = 0, ManhattanDistance = 1, ChebyshevDistance = 2, MinkowskiDistance = 3 }
    readonly double gridSize;
    readonly double halfGridSize;
    readonly int amountOfColor;
    readonly ConcurrentDictionary<Vector2Int, ColoredWorleyNoisePoint> pointHash = new();
    readonly double[] euclideanDistanceThresholds, manhattanDistanceThresholds, chebyshevDistanceThresholds;
    readonly Vector2Int[][] allOffset = new Vector2Int[][] {
        //Side
        new Vector2Int[] {
            new Vector2Int(x: 1, y: 0),
            new Vector2Int(x: -1, y: 0),
            new Vector2Int(x: 0, y: 1),
            new Vector2Int(x: 0, y: -1)
        },
        //Corner
        new Vector2Int[] {
            new Vector2Int(x: 1, y: 1),
            new Vector2Int(x: -1, y: -1),
            new Vector2Int(x: -1, y: 1),
            new Vector2Int(x: 1, y: -1)
        },

    };


    public ColoredWorleyNoise(double gridSize, int amountOfColor) {
        this.gridSize = gridSize;
        halfGridSize = gridSize / 2;
        this.amountOfColor = amountOfColor;
        euclideanDistanceThresholds = new double[] { halfGridSize, Math.Pow(halfGridSize, 2) * 2 };
        manhattanDistanceThresholds = new double[] { halfGridSize, gridSize };
        chebyshevDistanceThresholds = new double[] { halfGridSize, halfGridSize };
    }
    public void ClearHash() {
        pointHash.Clear();
    }
    public int GetSampleColoredWorleyNoise(double x, double y,
        DistanceCalculationMethod distanceCalculationMethod,
        uint p = 0) {
        if (p == 0 && distanceCalculationMethod == DistanceCalculationMethod.MinkowskiDistance) {
            throw new Exception("p must be bigger than 0");
        }

        int gridX = CalculateChunkPos(x, halfGridSize),
            gridY = CalculateChunkPos(y, halfGridSize);

        Vector2Int centerKey = new(gridX, gridY);
        x -= gridX * gridSize; y -= gridY * gridSize;
        if (distanceCalculationMethod == DistanceCalculationMethod.EuclideanDistance) {
            return EuclideanDistance(centerKey, x, y);
        } else if (distanceCalculationMethod == DistanceCalculationMethod.ManhattanDistance || (distanceCalculationMethod == DistanceCalculationMethod.MinkowskiDistance && p == 1)) {
            return ManhattanDistance(centerKey, x, y);
        } else if (distanceCalculationMethod == DistanceCalculationMethod.ChebyshevDistance || (distanceCalculationMethod == DistanceCalculationMethod.MinkowskiDistance && p == 2)) {
            return ChebyshevDistance(centerKey, x, y);
            // Fuck the Minkowski distance.
        } else return MinkowskiDistance(centerKey, x, y, p);
    }
    int EuclideanDistance(Vector2Int centerKey, double x, double y) {
        ColoredWorleyNoisePoint centerPoint = pointHash.GetOrAdd(centerKey, (centerKey) => new(centerKey.x, centerKey.y, gridSize, gridSize, amountOfColor));

        List<int> colorContender = new() { centerPoint.colorIndex };
        double shortestDistance = Math.Pow(centerPoint.GetX(0) - x, 2) + Math.Pow(centerPoint.GetY(0) - y, 2);
        double distance;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < allOffset[i].Length; j++) {
                Vector2Int key = centerKey + allOffset[i][j];
                int xOffset = allOffset[i][j].x, yOffset = allOffset[i][j].y;
                ColoredWorleyNoisePoint point = pointHash.GetOrAdd(key, (key) => new(key.x, key.y, gridSize, gridSize, amountOfColor));

                distance = Math.Pow(point.GetX(xOffset) - x, 2)
                    + Math.Pow(point.GetY(yOffset) - y, 2);

                if (distance > shortestDistance) { continue; }
                if (distance == shortestDistance) { colorContender.Add(point.colorIndex); continue; }
                colorContender = new List<int>() { point.colorIndex };
                shortestDistance = distance;
            }
            if (euclideanDistanceThresholds[i] > shortestDistance) {
                break;
            }
        }
        return colorContender[new System.Random((centerKey.x << 16) | (centerKey.x & 0xFFFF)).Next(0, colorContender.Count)];
    }
    int ManhattanDistance(Vector2Int centerKey, double x, double y) {
        ColoredWorleyNoisePoint centerPoint = pointHash.GetOrAdd(centerKey, (centerKey) => new(centerKey.x, centerKey.y, gridSize, gridSize, amountOfColor));

        List<int> colorContender = new() { centerPoint.colorIndex };
        double shortestDistance = Math.Abs(centerPoint.GetX(0) - x)
                    + Math.Abs(centerPoint.GetY(0) - y);
        double distance;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < allOffset[i].Length; j++) {
                Vector2Int key = centerKey + allOffset[i][j];
                int xOffset = allOffset[i][j].x, yOffset = allOffset[i][j].y;
                ColoredWorleyNoisePoint point = pointHash.GetOrAdd(key, (key) => new(key.x, key.y, gridSize, gridSize, amountOfColor));


                distance = Math.Abs(point.GetX(xOffset) - x)
                    + Math.Abs(point.GetY(yOffset) - y);

                if (distance > shortestDistance) { continue; }
                if (distance == shortestDistance) { colorContender.Add(point.colorIndex); continue; }
                colorContender = new List<int>() { point.colorIndex };
                shortestDistance = distance;
            }
            if (manhattanDistanceThresholds[i] > shortestDistance) {
                break;
            }
        }
        return colorContender[new System.Random((centerKey.x << 16) | (centerKey.x & 0xFFFF)).Next(0, colorContender.Count)];
    }
    int ChebyshevDistance(Vector2Int centerKey, double x, double y) {
        ColoredWorleyNoisePoint centerPoint = pointHash.GetOrAdd(centerKey, (centerKey) => new(centerKey.x, centerKey.y, gridSize, gridSize, amountOfColor));

        List<int> colorContender = new() { centerPoint.colorIndex };
        double shortestDistance = Math.Max(
                    Math.Abs(centerPoint.GetX(0) - x),
                    Math.Abs(centerPoint.GetY(0) - y));
        double distance;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < allOffset[i].Length; j++) {
                Vector2Int key = centerKey + allOffset[i][j];
                int xOffset = allOffset[i][j].x, yOffset = allOffset[i][j].y;

                ColoredWorleyNoisePoint point = pointHash.GetOrAdd(key, (key) => new(key.x, key.y, gridSize, gridSize, amountOfColor));

                distance = Math.Max(
                    Math.Abs(point.GetX(xOffset) - x),
                    Math.Abs(point.GetY(yOffset) - y));

                if (distance > shortestDistance) { continue; }
                if (distance == shortestDistance) { colorContender.Add(point.colorIndex); continue; }
                colorContender = new List<int>() { point.colorIndex };
                shortestDistance = distance;
            }
            if (chebyshevDistanceThresholds[i] > shortestDistance) {
                break;
            }
        }
        return colorContender[new System.Random((centerKey.x << 16) | (centerKey.x & 0xFFFF)).Next(0, colorContender.Count)];
    }
    int MinkowskiDistance(Vector2Int centerKey, double x, double y, uint p) {
        ColoredWorleyNoisePoint centerPoint = pointHash.GetOrAdd(centerKey, (centerKey) => new(centerKey.x, centerKey.y, gridSize, gridSize, amountOfColor));

        List<int> colorContender = new() { centerPoint.colorIndex };
        double shortestDistance = Math.Pow(Math.Pow(Math.Abs(centerPoint.GetX(0) - x), p) + Math.Pow(Math.Abs(centerPoint.GetY(0) - y), p), 1 / p);
        double distance;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < allOffset[i].Length; j++) {
                Vector2Int key = centerKey + allOffset[i][j];
                int xOffset = allOffset[i][j].x, yOffset = allOffset[i][j].y;

                ColoredWorleyNoisePoint point = pointHash.GetOrAdd(key, (key) => new(key.x, key.y, gridSize, gridSize, amountOfColor));

                distance = Math.Pow(
                    Math.Pow(
                        Math.Abs(point.GetX(xOffset) - x)
                        , p)
                    + Math.Pow(
                        Math.Abs(point.GetY(yOffset) - y)
                        , p)
                    , 1 / p);

                if (distance > shortestDistance) { continue; }
                if (distance == shortestDistance) { colorContender.Add(point.colorIndex); continue; }
                colorContender = new List<int>() { point.colorIndex };
                shortestDistance = distance;
            }
            if (chebyshevDistanceThresholds[i] > shortestDistance) {
                break;
            }
        }
        return colorContender[new System.Random((centerKey.x << 16) | (centerKey.x & 0xFFFF)).Next(0, colorContender.Count)];
    }



    static int RoundAwayZero(double f) {
        if (f > 0) return (int)Math.Ceiling(f);
        return (int)Math.Floor(f);
    }
    static int RoundTowardZero(double f) {
        if (f < 0) return (int)Math.Ceiling(f);
        return (int)Math.Floor(f);
    }
    static int CalculateChunkPos(double pos, double halfGridSize) {
        int layer = RoundAwayZero(pos / halfGridSize);
        int chunk = RoundTowardZero(layer / 2);
        return chunk;
    }
    public struct ColoredWorleyNoisePoint {
        public double localX, localY;
        readonly double width, height;
        public int colorIndex;
        public ColoredWorleyNoisePoint(
            int globalGridPosX, int globalGridPosY,
            double width, double height,
            int colorCount) {

            this.width = width;
            this.height = height;

            System.Random rng = new((globalGridPosX << 16) | (globalGridPosY & 0xFFFF));
            localX = (rng.NextDouble() - 0.5f) * width;
            localY = (rng.NextDouble() - 0.5f) * height;
            colorIndex = rng.Next(0, colorCount);
        }
        public double GetX(int offsetX) {
            return offsetX * width + localX;
        }
        public double GetY(int offsetY) {
            return offsetY * height + localY;
        }
    }
    public struct Vector2Int {
        public readonly int x, y;
        public Vector2Int(int x, int y) {
            this.x = x;
            this.y = y;
        }
        public static Vector2Int operator +(Vector2Int a, Vector2Int b) {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }
    }
}
