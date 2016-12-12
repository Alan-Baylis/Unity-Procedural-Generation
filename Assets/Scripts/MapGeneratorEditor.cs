using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor (typeof(MapGenerator))]

//Modify areas of the editor with custom settings
public class MapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        MapGenerator mapGen = (MapGenerator)target;

        if(DrawDefaultInspector()) if(mapGen.autoUpdate) mapGen.GenerateMap();
        if(GUILayout.Button("Generate")) {
            mapGen.GenerateMap();
        }
    }
}