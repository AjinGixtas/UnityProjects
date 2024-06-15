using UnityEngine;

public class ChunkResourceRenderer : MonoBehaviour {
    public ResourceData[] resourceDatas;
    public void RenderChunkResource(int[,] resourceMap, float[,] heightMap, float minHeight, Vector2Int offsetFromCenter, Transform chunk) {
        int width = resourceMap.GetLength(0), height = resourceMap.GetLength(1);
        Vector3 offsetToTopCorner = new(width / 2f, 0, height / 2f);
        System.Random rng = new(offsetFromCenter.x << 32 | offsetFromCenter.y);
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (resourceMap[x, y] == 0) { continue; }
                if(minHeight > heightMap[x,y]) { continue; }
                GameObject _ = GameObject.Instantiate(resourceDatas[resourceMap[x, y] - 1].fullResModel, new Vector3(x, heightMap[x, y], height - y - 1) + new Vector3(offsetFromCenter.x, 0, offsetFromCenter.y) * 16 - offsetToTopCorner, new Quaternion(), chunk);
                _.transform.Rotate(new(rng.Next(-100, -80), 0, rng.Next(-180, 180)));
            }
        }
    }
    void OnValidate() {
        for (int i = 0; i < resourceDatas.Length; i++) {
            resourceDatas[i].resourceIndex = i + 1;
        }
    }
}
