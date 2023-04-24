using System.IO;
using UnityEngine;
using System;
public enum E_Operator
{
    Add = 0,
    Subtraction = 1,
    Multiple = 2,
    //Division = 3,

}
public class TextureMaker : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public string fileName = "Test";

    private Texture2D m_Texture;

    [ContextMenu("생성")]
    void Create_File()
    {
        m_Texture = new Texture2D(width, height);
        Texture_Info info = new Texture_Info(width, height);
        SetPixel(info.Gradient_Circle(E_Operator.Add, 200).ColorRamp(new Color(1, 1, 1, 1), new Color(0, 0, 0, 1)));
        SaveTextureToFile();
    }


    void SetPixel(Texture_Info info)
    {
        for(int x = 0; x < info.Width; x++)
        {
            for(int y = 0; y < info.Height; y++)
            {
                m_Texture.SetPixel(x, y, info.Colors[x,y]);
            }
        }

        m_Texture.Apply();
    }
   
    void SaveTextureToFile()
    {
        // Convert the texture to a byte array
        byte[] bytes = m_Texture.EncodeToPNG();

        // Create a new file in the specified path
        string path = Path.Combine(Application.dataPath, fileName+".png");
        Debug.Log(path);
        File.WriteAllBytes(path, bytes);

        
    }
}

public class Texture_Info
{
    public int Width;
    public int Height;
    public Color[,] Colors;
    public Texture_Info(int width, int height)
    {
        Width = width;
        Height = height;
        Colors = new Color[width, height];
    }
}
public static class Ex_Color
{
    #region Operator
    public static Texture_Info Operator(this Texture_Info info, E_Operator oper, float value)
    {
        switch (oper)
        {
            case E_Operator.Add:
                for (int y = 0; y < info.Height; y++)
                {
                    for (int x = 0; x < info.Width; x++)
                    {
                        info.Colors[x, y].r += value;
                        info.Colors[x, y].g += value;
                        info.Colors[x, y].b += value;
                        info.Colors[x, y].a += value;
                    }
                }
                break;
            case E_Operator.Subtraction:
                for (int y = 0; y < info.Height; y++)
                {
                    for (int x = 0; x < info.Width; x++)
                    {
                        info.Colors[x, y].r -= value;
                        info.Colors[x, y].g -= value;
                        info.Colors[x, y].b -= value;
                        info.Colors[x, y].a -= value;
                    }
                }
                break;
            case E_Operator.Multiple:
                for (int y = 0; y < info.Height; y++)
                {
                    for (int x = 0; x < info.Width; x++)
                    {
                        info.Colors[x, y].r *= value;
                        info.Colors[x, y].g *= value;
                        info.Colors[x, y].b *= value;
                        info.Colors[x, y].a *= value;
                    }
                }
                break;
        }
        return info;
    }
    #endregion
    #region Color
    public static Texture_Info ColorRamp(this Texture_Info info, Color max_color, Color min_color)
    {
        Color temp = max_color - min_color;
        for (int x = 0; x < info.Width; x++)
        {
            for (int y = 0; y < info.Height; y++)
            {
                info.Colors[x, y] = temp * info.Colors[x, y] + min_color;
            }
        }
        return info;
    }
    #endregion
    #region Gradient
    public static Texture_Info Gradient_Circle(this Texture_Info info, E_Operator oper, float radius)
    {
        Action<Color[,]> Ac_Operator = null;
        switch (oper)
        {
            case E_Operator.Add:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] += colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Subtraction:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] -= colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Multiple:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] *= colors[x, y];
                        }
                    }
                };
                break;
        }

        Color[,] Arr_Color = new Color[info.Width, info.Height];
        float Center_x = info.Width * 0.5f;
        float Center_y = info.Height * 0.5f;
        for (int y = 0; y < info.Height; y++)
        {
            for (int x = 0; x < info.Width; x++)
            {
                float value = 1 - (Mathf.Sqrt(((Center_x - x) * (Center_x - x) + (Center_y - y) * (Center_y - y))) / radius);
                Arr_Color[x, y].r = value;
                Arr_Color[x, y].g = value;
                Arr_Color[x, y].b = value;
                Arr_Color[x, y].a = 1;
            }
        }
        Ac_Operator(Arr_Color);
        return info;
    }
    #endregion
    #region Shape
    #endregion
    #region Texture
    public static Texture_Info Noise(this Texture_Info info, E_Operator oper, float noise_value)
    {
        Action<Color[,]> Ac_Operator = null;
        switch (oper)
        {
            case E_Operator.Add:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] += colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Subtraction:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] -= colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Multiple:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] *= colors[x, y];
                        }
                    }
                };
                break;
        }

        Color[,] Arr_Color = new Color[info.Width, info.Height];
        for (int y = 0; y < info.Height; y++)
        {
            for (int x = 0; x < info.Width; x++)
            {
                float value = Mathf.PerlinNoise(x / noise_value, y / noise_value);
                Arr_Color[x, y].r = value;
                Arr_Color[x, y].g = value;
                Arr_Color[x, y].b = value;
                Arr_Color[x, y].a = 1;
            }
        }
        Ac_Operator(Arr_Color);
        return info;
    }

    public static Texture_Info Wave(this Texture_Info info, E_Operator oper)
    {
        Action<Color[,]> Ac_Operator = null;
        switch (oper)
        {
            case E_Operator.Add:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] += colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Subtraction:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] -= colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Multiple:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] *= colors[x, y];
                        }
                    }
                };
                break;
        }

        Color[,] Arr_Color = new Color[info.Width, info.Height];
        for (int y = 0; y < info.Height; y++)
        {
            for (int x = 0; x < info.Width; x++)
            {
                // WaveTexture의 픽셀값 계산
                float value = Mathf.Sin(x * 0.1f) + Mathf.Sin(y * 0.1f);
                Arr_Color[x, y].r = value;
                Arr_Color[x, y].g = value;
                Arr_Color[x, y].b = value;
                Arr_Color[x, y].a = 1;
            }
        }
        Ac_Operator(Arr_Color);
        return info;
    }

    public static Texture_Info Voronio(this Texture_Info info, E_Operator oper, int point_count)
    {
        Action<Color[,]> Ac_Operator = null;
        switch (oper)
        {
            case E_Operator.Add:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] += colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Subtraction:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] -= colors[x, y];
                        }
                    }
                };
                break;
            case E_Operator.Multiple:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] *= colors[x, y];
                        }
                    }
                };
                break;
        }

        Vector2[] points = new Vector2[point_count];
        for (int i = 0; i < point_count; i++)
        {
            points[i] = new Vector2(UnityEngine.Random.value * info.Width, UnityEngine.Random.value * info.Height);
        }

        Color[,] Arr_Color = new Color[info.Width, info.Height];
        for (int y = 0; y < info.Height; y++)
        {
            for (int x = 0; x < info.Width; x++)
            {
                float minDistance = float.MaxValue;
                Vector2 closestPoint = Vector2.zero;

                // Find the closest point
                for (int i = 0; i < point_count; i++)
                {
                    float distance = Vector2.Distance(points[i], new Vector2(x, y));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint = points[i];
                    }
                }

                // Set the pixel color
                Arr_Color[x, y].r = closestPoint.x / info.Width;
                Arr_Color[x, y].g = closestPoint.y / info.Height;
                Arr_Color[x, y].b = 0;
                Arr_Color[x, y].a = 1;
            }
        }
        Ac_Operator(Arr_Color);
        return info;
    }
    #endregion

}