using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour {
    //Variables
    public enum DrawMode {
        NoiseMap,
        ColourMap,
        Mesh
    }
    public DrawMode drawMode;

    const int mapChunkSize = 241;
    [Range(0f, 6f)] [SerializeField] int levelOfDetail;
    public int
        octaves,
        seed;
    public bool autoUpdate;
    public float
        noiseScale,
        lacunarity,
        meshHeightMultiplier;
    [Range(0f, 1f)] public float persistance;
    public AnimationCurve meshHeightCurve;

    public TerrainType[] regions;


    void Start() {
        //Generate the map when the game starts
        GenerateMap();
    }

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity);

        //Loop through noiseMap and generate the regions
        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for(int y = 0; y < mapChunkSize; y++) {
            for(int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x, y];

                //Give it a colour
                for(int ind = 0; ind < regions.Length; ind++) {
                    if(currentHeight <= regions[ind].height) {
                        //Found the region so save the colour
                        colourMap[y * mapChunkSize + x] = regions[ind].colour;
                        break;
                    }
                }
            }
        }

        //Reference the MapDisplay script and call one of it's functions
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if(drawMode == DrawMode.ColourMap)
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        else if(drawMode == DrawMode.Mesh)
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
    }

    void OnValidate() {
        if(lacunarity < 1) lacunarity = 1;
        if(octaves < 0) octaves = 0;
    }

    [System.Serializable] //Make it show up in the inspector
    public struct TerrainType {
        public string name;
        public float height;
        public Color colour;
    }
}