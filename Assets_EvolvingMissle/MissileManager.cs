using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class MissileManager : MonoBehaviour
{
    static readonly System.Random rng = new();
    public AntiMissileManager antiMissileManager;
    public GameObject B, C;
    public Vector2 B_pos, C_pos;
    public GameObject missilePrefab_0;
    public Transform missileContainer;
    public int missilePerWave;
    [SerializeField] float spawnRange, spawnRate;
    void Start()
    {
        B_pos = B.transform.position;
        C_pos = C.transform.position;
        InvokeRepeating(nameof(SpawnMissile), 0, spawnRate);
    }
    // Pre-allocare memory
    float x, y;
    float rotationRelativeToPivot;
    float[] rotationRange;
    GameObject _;
    void SpawnMissile() {
        for (int i = 0; i < missilePerWave; i++) {
            // Since get random y is easier than random x, we gonna do it first
            y = (float)rng.NextDouble() * spawnRange;
            x = rng.Next(2) == 0 ? Mathf.Sqrt(Mathf.Pow(spawnRange, 2) - Mathf.Pow(y, 2)) : -Mathf.Sqrt(Mathf.Pow(spawnRange, 2) - Mathf.Pow(y, 2));
            rotationRange = CalculateAngleRanges(new(x, y), B_pos, C_pos);
            // Custom pivot is not required but I firgured it help the code be more flexible.
            rotationRelativeToPivot = CalculateRotationAngleRelativeToAxisWithCustomPivot(0, 1, 0, 0, x, y);
            if (x > 0) { rotationRange[0] -= rotationRange[2] / 180 * rotationRelativeToPivot; }
            if (x < 0) { rotationRange[1] += rotationRange[2] / 180 * rotationRelativeToPivot; }
            _ = Instantiate(missilePrefab_0, new(x, y, 0), new Quaternion(), missileContainer);
            _.transform.Rotate(new(0, 0, UnityEngine.Random.Range(rotationRange[1], rotationRange[0])));
            antiMissileManager.AddTargetMissile(_);
        }
    }
    // The in put format as such:
    // A is the point we want to draw vector from, B and C is two point indicate the line A want to go through.
    // The result format as such:
    // 0 - min rotation range, 1 - max rotation range, 2 - absolute difference between min and max rotation range
    public static float[] CalculateAngleRanges(Vector2 A, Vector2 B, Vector2 C) {
        // Calculate the vectors AB and AC
        float vectorABX = B.x - A.x;
        float vectorABY = B.y - A.y;
        float vectorACX = C.x - A.x;
        float vectorACY = C.y - A.y;

        // Calculate the dot product of vectors AB and AC
        float dotProduct = (vectorABX * vectorACX) + (vectorABY * vectorACY);

        // Calculate the magnitudes of vectors AB and AC
        float magnitudeAB = Mathf.Sqrt((vectorABX * vectorABX) + (vectorABY * vectorABY));
        float magnitudeAC = Mathf.Sqrt((vectorACX * vectorACX) + (vectorACY * vectorACY));

        // Calculate the angle between vectors AB and AC
        float angle = Mathf.Acos(dotProduct / (magnitudeAB * magnitudeAC));

        // Calculate the angle between AB and the positive x-axis
        float startAngle = Mathf.Atan2(vectorABY, vectorABX);
        float endAngle = startAngle - angle;

        // Convert angles from radians to degrees
        startAngle = ToDegrees(startAngle);
        endAngle = ToDegrees(endAngle);
        return new float[3] {startAngle, endAngle, ToDegrees(angle)};
        static float ToDegrees(float radians) {
            return radians * 180f / Mathf.PI;
        }
    }
    public static float CalculateRotationAngleRelativeToAxisWithCustomPivot(float i, float j, float x_pivot, float y_pivot, float x_vector, float y_vector) {
        // Calculate the dot product of the vectors
        float dotProduct = x_vector * i + y_vector * j;

        // Calculate the magnitudes of the vectors
        float vectorMagnitude = Mathf.Sqrt(x_vector * x_vector + y_vector * y_vector);
        float referenceMagnitude = 1; // Magnitude of the reference vector (positive x-axis)

        // Calculate the rotation angle in radians
        float rotationAngle = Mathf.Acos(dotProduct / (vectorMagnitude * referenceMagnitude));

        // Convert the rotation angle from radians to degrees
        float rotationAngleDegrees = rotationAngle * 180 / Mathf.PI;

        return rotationAngleDegrees;
    }
}
