using UnityEngine;
public class TerrainRenderer : MonoBehaviour {
    public MeshCollider meshCollider;
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public void DrawTexture(Mesh mesh, Texture2D texture) {
        meshFilter.sharedMesh = mesh;
        textureRenderer.sharedMaterial.mainTexture = texture;
    }
    public void DrawMesh(Mesh mesh, Texture2D texture) {
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void DrawCollider(Mesh mesh) {
        meshCollider.sharedMesh = mesh;
    }
}
