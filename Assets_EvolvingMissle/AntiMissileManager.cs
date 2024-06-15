using System;
using System.Collections.Generic;
using UnityEngine;

public class AntiMissileManager : MonoBehaviour
{
    public static readonly System.Random rng = new();
    List<GameObject> missilesTarget = new();
    DNA[] poolDNA = new DNA[20];
    public GameObject basicAntiMissile;
    [SerializeField] private float antiMissileSpawnRate, repopulateRate, startingGround;
    private void Start() {
        AntiMissileBehaviour.antiMissileManager = this;
        MissileBehaviour.antiMissileManager = this;
        for(int i = 0; i < poolDNA.Length; i++) {
            poolDNA[i] = new();
        }
        InvokeRepeating(nameof(SpawnAntiMissile), 1, antiMissileSpawnRate);
        InvokeRepeating(nameof(RepopulatePoolDNA), repopulateRate, repopulateRate);
    }
    public void NotifyDestruction(DNA dna, AntiMissileBehaviour.DestructReason destructReason) {
        switch(destructReason) {
            case AntiMissileBehaviour.DestructReason.TARGET_ELIMINATED:
                dna.fitnessScore += 1;
                break;
            case AntiMissileBehaviour.DestructReason.FUEL_DEPLETED:
                dna.fitnessScore -= 1;
                break;
        }
    }
    GameObject missile;
    [SerializeField] private int antiMissilePerWave;
    public void SpawnAntiMissile() {
        for (int i = 0; i < antiMissilePerWave; i++) {
            missile = Instantiate(basicAntiMissile, new Vector2((float)rng.NextDouble() * 90 - 45f, startingGround), new Quaternion());
            missile.GetComponent<AntiMissileBehaviour>().SetStat(poolDNA[i]);
        }
    }
    public int topCountDNA = 20;
    void RepopulatePoolDNA() {
        if (topCountDNA != 1) {
            topCountDNA--;
        }
        Array.Sort<DNA>(poolDNA, (a, b) => b.fitnessScore.CompareTo(a.fitnessScore));
        for(int i = 0; i < topCountDNA; i++) {
            poolDNA[i].fitnessScore = 0;
        }
        for(int i = topCountDNA; i < poolDNA.Length; i++) {
            poolDNA[i] = MutateDNA(poolDNA[i % topCountDNA]);
        }
    }
    public DNA MutateDNA(DNA dna) {
        return new(
            dna.fuelCapacity + GetPointMutation(), 
            dna.centeringFactor + GetPointMutation(), 
            dna.acceleration + GetPointMutation(),
            dna.minSpeed + GetPointMutation(),
            dna.maxSpeed + GetPointMutation(),
            dna.accuracy + GetPointMutation());
    }
    public float GetPointMutation() {
        return UnityEngine.Random.Range(-0.25f, 0.25f);
    }
    public void RemoveTargetMissile(GameObject gameObject) {
        missilesTarget.Remove(gameObject);
    }
    public void AddTargetMissile(GameObject gameObject) {
        missilesTarget.Add(gameObject);
    }
    public GameObject GetTargetMissile() {
        if(missilesTarget.Count == 0) { return null; }
        return missilesTarget[rng.Next(0, missilesTarget.Count)];
    }
}
public class DNA {
    public DNA(float fuelCapacity, float centeringFactor, float acceleration, float minSpeed, float maxSpeed, float accuracy) {
        float costOverflow = (fuelCapacity + centeringFactor + acceleration + minSpeed + maxSpeed + accuracy - 7) / 6;

        this.fuelCapacity = fuelCapacity - costOverflow;
        this.centeringFactor = centeringFactor - costOverflow;
        this.acceleration = acceleration - costOverflow;
        this.minSpeed = minSpeed - costOverflow;
        this.maxSpeed = maxSpeed - costOverflow;
        this.accuracy = accuracy - costOverflow;
    }
    public DNA() {
        fuelCapacity = (float)AntiMissileManager.rng.NextDouble();
        centeringFactor = (float)AntiMissileManager.rng.NextDouble();
        acceleration = (float)AntiMissileManager.rng.NextDouble();
        minSpeed = (float)AntiMissileManager.rng.NextDouble();
        maxSpeed = (float)AntiMissileManager.rng.NextDouble();
        accuracy = (float)AntiMissileManager.rng.NextDouble();
        float costOverflow = (fuelCapacity + centeringFactor + acceleration + minSpeed + maxSpeed + accuracy - 6) / 6;

        this.fuelCapacity = fuelCapacity - costOverflow;
        this.centeringFactor = centeringFactor - costOverflow;
        this.acceleration = acceleration - costOverflow;
        this.minSpeed = minSpeed - costOverflow;
        this.maxSpeed = maxSpeed - costOverflow;
        this.accuracy = accuracy - costOverflow;
    }
    public float fuelCapacity, centeringFactor, acceleration, minSpeed, maxSpeed, accuracy;
    public float fitnessScore = 0;
}
