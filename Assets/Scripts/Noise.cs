using UnityEngine;
using System.Collections;

//This script won't be applied to any objects in the scene so there is no reason to inherit from MonoBehaviour
//And since there won't be multiple instances of this script, it will be static
public static class Noise {
    //Generate noise map and return a grid of values between 0 and 1
    //float[,] = Two dimensional array of float values
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int numOctaves, float persistance, float lacunarity) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //Seed
        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[numOctaves];
        for(int ind = 0; ind < numOctaves; ind++) {
            float offsetX = rng.Next(-100000, 100000);
            float offsetY = rng.Next(-100000, 100000);

            octaveOffsets[ind] = new Vector2(offsetX, offsetY);
        }

        //Check if scale is 0, avoid division by 0 error
        if(scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //Loop through the noiseMap
        for(int y = 0; y < mapHeight; y++) {
            for(int x = 0; x < mapWidth; x++) {
                //Frequency and amplitude
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int ind = 0; ind < numOctaves; ind++) {
                    //Divide by a scale to get non-integer values
                    float sampleX = x / scale * frequency + octaveOffsets[ind].x;
                    float sampleY = y / scale * frequency + octaveOffsets[ind].y;

                    //Allow for negative noise so we multiply by 2 and subtract 1
                    float perlinVal = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinVal * amplitude;

                    //Amplitude decreases while the frequency increases
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                //Normalize the noiseMap so the values are back within 0 to 1
                if(noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if(noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }
        //Loop through the noiseMap again
        for(int y = 0; y < mapHeight; y++) {
            for(int x = 0; x < mapWidth; x++) {
                //InverseLerp returns value between 0 and 1, effectively normalizing it
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}