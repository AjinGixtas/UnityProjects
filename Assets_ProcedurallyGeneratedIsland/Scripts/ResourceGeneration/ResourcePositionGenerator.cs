using UnityEngine;

public static class ResourcePositionGenerator {
    // For-loop hell, this single function probaly do at least 6-13 for-loop on average, depend on how many I want it to.
    // Let's just hope I never have to come back and refactor this shit XP
    public static int[,] GenerateResourcePositionMap(float[,] noiseMap, ref ResourceType[] resourceTypes) {
        int width = noiseMap.GetLength(0), height = noiseMap.GetLength(1);
        int[,] resourceMap = new int[width, height];

        for (int i = 0; i < resourceTypes.Length; i++) {
            resourceTypes[i].amountOfResourceOnMap = 0;
            if (resourceTypes[i].randomSeed) { Debug.Log("Here!"); resourceTypes[i].seed = Random.Range(-32768, 32767); }
            System.Random randomNumberGenerator = new(resourceTypes[i].seed);

            float minHeight = resourceTypes[i].minHeight, maxHeight = resourceTypes[i].maxHeight;
            float[,] resourceChanceToSpawnMapThreshold = NoiseGenerator.GenerateNoise(width, height, resourceTypes[i].scale, resourceTypes[i].octave, resourceTypes[i].persistance, resourceTypes[i].lacunarity, randomNumberGenerator.Next(-32768, 32767), Vector2.zero);
            

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    // This tile is too high/too low for this resource type to spawn
                    if (noiseMap[x, y] > maxHeight || minHeight > noiseMap[x, y]) { continue; }
                    // This tile is already occupied by another resource type
                    if (resourceMap[x, y] != 0) { continue; }
                    // This resource type is not worthy enough to occupy this tile (?)
                    if (resourceChanceToSpawnMapThreshold[x, y] > resourceTypes[i].resourceSpawnRange) { continue; }
                    // This resource type is not lucky enough to occupy this tile T_T
                    // Because the System.Random class don't have a built in method to generate floaty number, I have to devide the number by 
                    if (randomNumberGenerator.Next(0,100_000) / 100_000f >= resourceTypes[i].resourceDensity) { continue; }
                    // Congratualtion! This resource type (whatever the fuck it is) has proven to be worthy of occupying this tile :D
                    resourceMap[x, y] = i + 1;
                    resourceTypes[i].amountOfResourceOnMap++;
                }
            }

        }
        return resourceMap;
    }
}

