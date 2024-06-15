using System;

public static class RandomNumberGenerator
{
    private static Random rng = new();
    public static void AssignSeed(int seed) {
        rng = new(seed);
    }
    public static int GetRandomNumber(int minValue, int maxValue) {
        return rng.Next(minValue, maxValue);
    }
}
