using UnityEngine;

public class ResourceRenderer : MonoBehaviour {
    public Transform[] resourceTypeContainer;
    public new Renderer renderer;
    public float xOffSet, yOffset;
    public float spread;
    public void RenderResource(int[,] resoureMap, ResourceType[] resourceTypes) {
        int width = resoureMap.GetLength(0), height = resoureMap.GetLength(1);

        renderer = GameObject.FindWithTag("Test").GetComponent<Renderer>();
        Color[] colors = new Color[width * height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                colors[width * y + x] = resoureMap[x, y] - 1 != -1 ? resourceTypes[resoureMap[x, y] - 1].resourceColor : Color.white;
            }
        }
        Texture2D texture = new(width, height) {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };
        texture.SetPixels(colors);
        texture.Apply();
        renderer.sharedMaterial.mainTexture = texture;
    }
    public void KillAllChild() {
        for (int i = 0; i < resourceTypeContainer.Length; i++) {
            while (resourceTypeContainer[i].childCount > 0) {
                DestroyImmediate(resourceTypeContainer[i].GetChild(0).gameObject);
            }
        }
    }
    public void RenderResourceObject(int[,] resoureMap, float[,] heightMap, ResourceType[] resourceTypes, int seed) {
        System.Random randomNumberGenerator = new(seed);

        KillAllChild();

        int width = resoureMap.GetLength(0), height = resoureMap.GetLength(1);

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                if (resoureMap[x, z] == 0 || resoureMap[x, z] == -1) { continue; }
                float xPos = (x - width / 2f) * spread + Random.Range(-1f, 1f), zPos = ((z - height / 2f) * spread) * -1 + Random.Range(-1f, 1f), yPos = heightMap[x, z];
                float angleX = randomNumberGenerator.Next(-5, 6), angleZ = randomNumberGenerator.Next(-5, 6), angleY = randomNumberGenerator.Next(-180, 180);

                GameObject resourcePrefab = resourceTypes[resoureMap[x, z] - 1].resourcePrefab;
                Vector3 rotationOffset = new(angleX, angleY, angleZ);
                GameObject resource = Instantiate(resourcePrefab, new Vector3(xPos, yPos, zPos), resourcePrefab.transform.rotation, resourceTypeContainer[resoureMap[x, z] - 1]);
                resource.transform.Rotate(rotationOffset);
            }
        }
    }
}