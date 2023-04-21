using System.IO;
using UnityEngine;

public class TextureMaker : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public string fileName = "noise.png";

    public float Noise_Value;
    private Texture2D noiseTexture;

    [ContextMenu("»ý¼º")]
    void Create_File()
    {
        // Create a new Texture2D with the specified width and height
        noiseTexture = new Texture2D(width, height);

        // Generate the noise
        GenerateNoise();

        // Save the noise texture to a file
        SaveTextureToFile();
    }

    void GenerateNoise()
    {
        // Loop through each pixel in the texture and set the color based on the noise value
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x / Noise_Value, y / Noise_Value);
                noiseTexture.SetPixel(x, y, new Color(noiseValue, noiseValue, noiseValue));
            }
        }

        // Apply the changes to the texture
        noiseTexture.Apply();
    }

    void SaveTextureToFile()
    {
        // Convert the texture to a byte array
        byte[] bytes = noiseTexture.EncodeToPNG();

        // Create a new file in the specified path
        string path = Path.Combine(Application.dataPath, fileName);
        Debug.Log(path);
        File.WriteAllBytes(path, bytes);
    }
}