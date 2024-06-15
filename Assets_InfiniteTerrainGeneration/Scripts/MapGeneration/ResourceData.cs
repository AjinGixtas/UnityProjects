using System;
using System.IO;
using UnityEngine;
[CreateAssetMenu(fileName = "ResourceData")]
public class ResourceData : ScriptableObject {
    public string resourceName;
    public int resourceIndex;
    public GameObject lowPolyModel;
    public GameObject fullResModel;
    public ResourceData(string resourceName, int resourceIndex, GameObject lowPolyModel, GameObject fullResModel) {
        this.resourceName = resourceName;
        this.resourceIndex = resourceIndex;
        this.lowPolyModel = lowPolyModel;
        this.fullResModel = fullResModel;
        if (resourceIndex < 0) { Debug.LogError("Index must be bigger or equal to 0"); }
    }
}
