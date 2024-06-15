using UnityEngine;

public static class NoiseGenerator {
    public static float[,] GenerateNoise(int width, int height, float scale, int octave, float persistance, float lacunarity, int seed, Vector2 offset) {
        float[,] result = new float[width, height];
        // This array hold the coordinate on where to sample Perlin noise value for each layer
        Vector2[] octaveOffsets = new Vector2[octave];

        System.Random randomNumberGenerator = new(seed);
        // Generate offset coordinate for each layer
        for (int i = 0; i < octave; i++) {
            // This value range fir perfectly inside 64-bits! (probaly)
            int offsetX = randomNumberGenerator.Next(short.MinValue, short.MaxValue);
            int offsetY = randomNumberGenerator.Next(short.MinValue, short.MaxValue);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // Value for normalization purpose
        float maxNoiseValue = float.MinValue;
        float minNoiseValue = float.MaxValue;

        // Loop through all tile
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                // Intialize variable for that tile
                float amplitude = 1, frequency = 1;
                float noiseResult = 0;

                // Loop through all layer
                for (int i = 0; i < octave; i++) {

                    // Calculate coordinate to sample the noise
                    /* 
                     * Frequency will make the sample point further/closer exponentially
                     * Sample point closer will make the noise map generally less noisy
                     * and smoother while further point will make noise less smooth
                     * 
                     * We increment the coordinate by octaveOffsets to make sample point 
                     * between 2 layer further away and therefore make it less likely to
                     * look the same.
                     */
                    float xPos = x / scale * frequency + octaveOffsets[i].x + offset.x;
                    float yPos = y / scale * frequency + octaveOffsets[i].y + offset.y;
                    // Multiply by 2 and minus 1 make the value range from (0,1) to (-1,1)
                    float noiseValue = Mathf.PerlinNoise(xPos, yPos) * 2 - 1;



                    /* Amplitude decide how high and low the noise can be
                     * 
                     * We cound multiply the noiseResult by amplitude instead of this, but it's
                     * easier to read to it's here to stay >:)
                     */
                    noiseResult += noiseValue * amplitude;

                    // Increase amplitude and frequency exponentially, this make amplitude
                    // and frequency have bigger effect on subsequent layer
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                // This if-else block is for normalization latter on
                if (maxNoiseValue < noiseResult) {
                    maxNoiseValue = noiseResult;
                } else if (minNoiseValue > noiseResult) {
                    minNoiseValue = noiseResult;
                }
                result[x, y] = noiseResult;
            }
        }
        // Normalize all value
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                result[x, y] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, result[x, y]);
            }
        }
        return result;
    }
    public static float[] GenerateNoise1D(int length, float scale, int octave, float persistance, float lacunarity, int seed, float offset) {
        float[] result = new float[length];
        float[] octaveOffsets = new float[octave];
        System.Random randomNumberGenerator = new(seed);

        for (int i = 0; i < octave; i++) {
            octaveOffsets[i] = randomNumberGenerator.Next(short.MinValue, short.MaxValue);
        }
        float maxNoiseValue = float.MinValue, minNoiseValue = float.MaxValue;

        for (int x = 0; x < length; x++) {
            float amplitude = 1, frequency = 1;
            float noiseResult = 0;
            for (int i = 0; i < octave; i++) {
                float samplePos = x / scale * frequency + octaveOffsets[i] + offset;
                Debug.Log("Pos: " + samplePos);
                float noiseValue = Mathf.PerlinNoise(samplePos, 0) * 2 - 1;
                Debug.Log("Value: " + noiseValue);

                noiseResult += noiseValue * amplitude;

                amplitude *= persistance;
                frequency *= lacunarity;
            }
            if (maxNoiseValue < noiseResult) {
                maxNoiseValue = noiseResult;
            } else if (minNoiseValue > noiseResult) {
                minNoiseValue = noiseResult;
            }
            result[x] = noiseResult;
        }
        for (int x = 0; x < length; x++) {
            result[x] = Mathf.InverseLerp(minNoiseValue, maxNoiseValue, result[x]);
        }

        return result;
    }
    public static void Print2DArray(float[,] array) {
        string str = "";
        for (int x = 0; x < array.GetLength(0); x++) {
            for (int y = 0; y < array.GetLength(1); y++) {
                str += $"{array[x, y]} ";
            }
            str += "\n";
        }
        Debug.Log(str);
    }
}