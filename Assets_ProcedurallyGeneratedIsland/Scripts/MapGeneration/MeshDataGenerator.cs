using UnityEngine;
using System;

public static class MeshDataGenerator {

    public static MeshData GenerateTerrainMesh(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshGenerator meshData = new(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 0, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return new MeshData(meshData.CreateMesh(), new float[width, height]);

    }
    public static MeshData GenerateTerrainMesh(float[,] noiseMap, float globalHeightMultiplier, AnimationCurve heighMultiplier, int levelOfDetail) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        float[,] heightMap = new float[width, height];

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshGenerator meshData = new(width, height);
        int vertexIndex = 0;

        int gapBetweenVerticies = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
        int verticiesPerLine = width == height ? (width - 1) / gapBetweenVerticies : throw new Exception("Width not equal to height!");
        for (int y = 0; y < height; y += gapBetweenVerticies) {
            for (int x = 0; x < width; x += gapBetweenVerticies) {

                heightMap[x, y] = heighMultiplier.Evaluate(noiseMap[x, y]) * globalHeightMultiplier;
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heighMultiplier.Evaluate(noiseMap[x, y]) * globalHeightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticiesPerLine + 1, vertexIndex + verticiesPerLine);
                    meshData.AddTriangle(vertexIndex + verticiesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return new MeshData(meshData.CreateMesh(), heightMap);

    }
}
public struct MeshData {
    public Mesh mesh;
    public float[,] meshHeightMap;
    public MeshData(Mesh mesh, float[,] meshHeight) { this.mesh = mesh; this.meshHeightMap = meshHeight; }
}
public class MeshGenerator {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshGenerator(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new() {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }

}
