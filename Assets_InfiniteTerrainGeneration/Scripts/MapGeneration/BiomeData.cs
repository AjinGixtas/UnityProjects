using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData")]
public class BiomeData : ScriptableObject {
    public string biomeName;
    public Color biomeGroundColor;

    public float minHumidity, maxHumidity;
    public float minTemperature, maxTemperature;
    public float minElevation, maxElevation;
    public float minErosion, maxErosion;

    public ResourceSpawnInfo[] resourceSpawnInfoInBiome;
    public BiomeData(string biomeName, Color biomeGroundColor, float minHumidity, float maxHumidity, float minTemperature, float maxTemperature, float minElevation, float maxElevation, float minErosion, float maxErosion, ResourceSpawnInfo[] resourceSpawnInfoInBiome) {
        this.biomeName = biomeName;
        this.biomeGroundColor = biomeGroundColor;
        this.minHumidity = minHumidity;
        this.maxHumidity = maxHumidity;
        this.minTemperature = minTemperature;
        this.maxTemperature = maxTemperature;
        this.minElevation = minElevation;
        this.maxElevation = maxElevation;
        this.minErosion = minErosion;
        this.maxErosion = maxErosion;
        this.resourceSpawnInfoInBiome = resourceSpawnInfoInBiome;
        Array.Sort(resourceSpawnInfoInBiome, (a, b) => a.resourceData.resourceIndex.CompareTo(b.resourceData.resourceIndex));
    }
}