using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor {

    // Bare-bone implementation of GUI
    public override void OnInspectorGUI() {
        MapGenerator mapGen = (MapGenerator)target;
        if (DrawDefaultInspector()) {
            if (mapGen.autoUpdate) {
                mapGen.GenerateMapInEditor();
            }
        }
        if (GUILayout.Button("Generate noise map")) {
            Debug.Log("Clicked!");
            mapGen.GenerateMapInEditor();
        }
    }
}