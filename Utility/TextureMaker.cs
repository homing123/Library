using System.IO;
using UnityEngine;
using System;
public enum E_Operator
{
    Add = 0,
    Subtraction = 1,
    Multiple = 2,
    //Division = 3,
    Devide_Add = 4,

}
public class TextureMaker : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public string fileName = "Test";

    private Texture2D m_Texture;
    [SerializeField] Color color;

    [ContextMenu("생성")]
    void Create_File()
    {
        m_Texture = new Texture2D(width, height);
        Texture_Info info = new Texture_Info(width, height);

        SetPixel(info.Noise(E_Operator.Add, 0.01f, 0.5f, 3, 2).ColorRamp(new ColorKey_Info[]{new ColorKey_Info(0.3f,0.5f,0.5f,0.5f,1)}).ColorRamp(new ColorKey_Info[] { new ColorKey_Info(0, 0f, 0.95f, 1, 1), new ColorKey_Info(0.5f,0f,0.95f,1,1), new ColorKey_Info(1, 0.98f, 0.98f, 1, 1) }));
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
public class ColorKey_Info
{
    public float Key;
    public Color Color;

    public ColorKey_Info(float key, float r, float g, float b, float a)
    {
        Key = key;
        Color.r = r;
        Color.g = g;
        Color.b = b;
        Color.a = a;
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
                        //float random_value = UnityEngine.Random.Range(-value, value) * info.Colors[x, y].r;
                        //info.Colors[x, y].r += random_value;
                        //info.Colors[x, y].g += random_value;
                        //info.Colors[x, y].b += random_value;
                        //info.Colors[x, y].a += random_value;
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
            case E_Operator.Devide_Add:
                for (int y = 0; y < info.Height; y++)
                {
                    for (int x = 0; x < info.Width; x++)
                    {
                        info.Colors[x, y].r = info.Colors[x, y].r * 0.5f + value * 0.5f;
                        info.Colors[x, y].g = info.Colors[x, y].g * 0.5f + value * 0.5f;
                        info.Colors[x, y].b = info.Colors[x, y].b * 0.5f + value * 0.5f;
                        info.Colors[x, y].a = info.Colors[x, y].a * 0.5f + value * 0.5f;
                    }
                }
                break;
        }
        return info;
    }
    static Action<Color[,]> Get_Ac_Operator(Texture_Info info, E_Operator oper)
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
            case E_Operator.Devide_Add:
                Ac_Operator = (colors) =>
                {
                    for (int y = 0; y < info.Height; y++)
                    {
                        for (int x = 0; x < info.Width; x++)
                        {
                            info.Colors[x, y] = info.Colors[x, y] * 0.5f + colors[x, y] * 0.5f;
                        }
                    }
                };
                break;
        }
        return Ac_Operator;
    }
    #endregion
    #region Color
    public static Texture_Info ColorRamp(this Texture_Info info, ColorKey_Info[] key_info)
    {
        
        for (int x = 0; x < info.Width; x++)
        {
            for (int y = 0; y < info.Height; y++)
            {
                info.Colors[x, y] = ColorRamp_GetColor(key_info, info.Colors[x, y].r);
            }
        }
        Debug.Log(ColorRamp_GetColor(key_info, 0.31f));
        return info;
    }
    static Color ColorRamp_GetColor(ColorKey_Info[] key_info, float key)
    {
        ColorKey_Info UpKey = null;
        ColorKey_Info DownKey = null;

        Color Color_Return;
      
        if (key_info == null || key_info.Length == 0)
        {
            Color_Return.r = key;
            Color_Return.g = key;
            Color_Return.b = key;
            Color_Return.a = 1;
            return Color_Return;
        }
        else
        {
            for (int i = 0; i < key_info.Length; i++)
            {
                //키값이 같을땐 기준키보다 크면은 idx순서가 작은게 사용되고 기준키보다 작으면 idx순서가 큰게 사용된다.
                if (key <= key_info[i].Key)
                {
                    if (UpKey == null)
                    {
                        UpKey = key_info[i];
                    }
                    else
                    {
                        if (UpKey.Key > key_info[i].Key)
                        {
                            UpKey = key_info[i];
                        }
                    }
                }

                if (key >= key_info[i].Key)
                {
                    if (DownKey == null)
                    {
                        DownKey = key_info[i];
                    }
                    else
                    {
                        if (DownKey.Key <= key_info[i].Key)
                        {
                            DownKey = key_info[i];
                        }
                    }
                }
            }
            if (key == 1)
            {
                if (UpKey == null)
                {
                    Color_Return.r = 1;
                    Color_Return.g = 1;
                    Color_Return.b = 1;
                    Color_Return.a = 1;
                }
                else
                {
                    Color_Return = UpKey.Color;
                }
            }
            else
            {
                float up_key;
                float down_key;
                Color up_color;
                Color down_color;
                if (UpKey == null)
                {
                    up_key = 1;
                    up_color = new Color(1, 1, 1, 1);
                }
                else
                {
                    up_key = UpKey.Key;
                    up_color = UpKey.Color;
                }
                if(DownKey == null)
                {
                    down_key = 0;
                    down_color = new Color(0, 0, 0, 1);
                }
                else
                {
                    down_key = DownKey.Key;
                    down_color = DownKey.Color;
                }

                Color_Return = (key - down_key) / (up_key - down_key) * (up_color - down_color) + down_color;
            }
            return Color_Return;
        }
        
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
    public static Texture_Info Noise(this Texture_Info info, E_Operator oper,float scale ,float persistence, int octaves, float lacunarity)
    {
        Color[,] Arr_Color = new Color[info.Width, info.Height];

        for (int y = 0; y < info.Height; y++)
        {
            for (int x = 0; x < info.Width; x++)
            {
                float total = 0;
                float frequency = 1;
                float amplitude = 1;
                float maxValue = 0;
                for (int i = 0; i < octaves; i++)
                {
                    total += Mathf.PerlinNoise(x * scale * frequency, y * scale * frequency) * amplitude;
                    maxValue += amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                float value = total / maxValue;
                Arr_Color[x, y].r = value;
                Arr_Color[x, y].g = value;
                Arr_Color[x, y].b = value;
                Arr_Color[x, y].a = 1;
            }
        }
        Get_Ac_Operator(info,oper)(Arr_Color);
        return info;
    }

    public static Texture_Info Wave(this Texture_Info info, E_Operator oper)
    {
        Color[,] Arr_Color = new Color[info.Width, info.Height];
        for (int y = 0; y < info.Height; y++)
        {
            for (int x = 0; x < info.Width; x++)
            {
                // WaveTexture의 픽셀값 계산
                float value = Mathf.Sin(x * 0.1f) * 0.5f + 0.5f;
                Arr_Color[x, y].r = value;
                Arr_Color[x, y].g = value;
                Arr_Color[x, y].b = value;
                Arr_Color[x, y].a = 1;
            }
        }
        Get_Ac_Operator(info, oper)(Arr_Color);
        return info;
    }

    public static Texture_Info Voronoi(this Texture_Info info, E_Operator oper, int point_count)
    {
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
        Get_Ac_Operator(info, oper)(Arr_Color);
        return info;
    }
    #endregion

}