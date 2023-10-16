using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using System;
using Newtonsoft.Json;
using Unity.VisualScripting;
public enum E_SortDir
{
    Up = 0,
    Down = 1,
}
public enum E_Layer
{
    BG = 64,
    Bio = 128,
    Map = 256,
    Effect = 512,
    Bullet = 1024,
    BioEnemy = 2048,
}
namespace Define
{
    public class Extension
    {
        public const int Layer_Map = 8;
        public static WaitForEndOfFrame waitframe = new WaitForEndOfFrame();
        public static void Init_Canvas(Canvas canvas)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
        }
        public static Color GetMedian(Color color_1, Color color_2, float value = 0.5f)
        {
            return new Color(GetMedian(color_1.r, color_2.r, value), GetMedian(color_1.g, color_2.g, value), GetMedian(color_1.b, color_2.b, value), 1);
        }
        public static float GetMedian(float f1, float f2, float value = 0.5f)
        {
            return f1 + (f2 - f1) * value;
        }

        public static int GetLayerMask(E_Layer layer)
        {
            return (int)layer;
        }
        public static int GetLayerMask(E_Layer layer, E_Layer layer_1)
        {
            return (int)(layer | layer_1);
        }
        public static int GetLayerMask(E_Layer layer, E_Layer layer_1, E_Layer layer_2)
        {
            return (int)(layer | layer_1 | layer_2);
        }

        //public static int GetEnemyLayerMask(E_Bio biokind)
        //{
        //    switch (biokind)
        //    {
        //        case E_Bio.Character:
        //            return (int)E_Layer.BioEnemy;
        //        case E_Bio.Monster:
        //        case E_Bio.Boss:
        //            return (int)E_Layer.Bio;
        //    }
        //    return 0;
        //}
        public static int GetLayer(E_Layer layer)
        {
            switch (layer)
            {
                case E_Layer.BG:
                    return 6;
                case E_Layer.Bio:
                    return 7;
                case E_Layer.Map:
                    return 8;
                case E_Layer.Effect:
                    return 9;
                case E_Layer.Bullet:
                    return 10;
                case E_Layer.BioEnemy:
                    return 11;
            }
            return 0;
        }

    }

}
public class Single<T> : MonoBehaviour where T : MonoBehaviour
{
    static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
                if (m_instance == null)
                {
                    m_instance = new GameObject().AddComponent<T>();
                }
                DontDestroyOnLoad(m_instance.gameObject);
            }
            return m_instance;
        }
    }

}
public class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
    static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
                if (m_instance == null)
                {
                    m_instance = GameManager.Instance.gameObject.AddComponent<T>();
                }
            }
            return m_instance;
        }
    }
}
public abstract class PlayManager : MonoBehaviour
{
    public PlayManager()
    {
        Instance = this;
    }
    public static PlayManager Instance;
    public abstract void WaitStart();
    public abstract void GameRestart();
}
public abstract class UserData
{

}
public abstract class UserData_Server : UserData
{
    public UserData_Server()
    {
        if (GameManager.LoadindStart == false)
        {
            UserManager.fc_LoadServer += Load;
        }
    }

    public abstract Task Load();
}
public abstract class UserData_Local : UserData
{
    public UserData_Local()
    {
        if (GameManager.LoadindStart == false)
        {
            UserManager.ac_LoadLocal += Load;
        }
    }

    public abstract void Load();
}



public static class Ex_Define
{
    public static void AddRandomInfo(this List<(int, int, int, float)> list, int kind, int id, int count, float per)
    {
        if (count != 0)
        {
            list.Add((kind, id, count, per));
        }
    }
    public static void AddItemInfo(this List<(int, int, int)> list, int kind, int id, int count)
    {
        if (count != 0)
        {
            list.Add((kind, id, count));
        }
    }
    public static (int kind, int id, int count)[] ItemMulCount(this (int kind, int id, int count)[] iteminfo, int count)
    {
        var result = new (int kind, int id, int count)[iteminfo.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = iteminfo[i];
            result[i].count = iteminfo[i].count * count;
        }
        return result;
    }
    public static (int kind, int id, int count, float per)[] RandomItemMulCount(this (int kind, int id, int count, float per)[] randomiteminfo, int count)
    {
        var result = new (int kind, int id, int count, float per)[randomiteminfo.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = randomiteminfo[i];
            result[i].count = randomiteminfo[i].count * count;
        }
        return result;
    }
    public static int Get_Idx<TValue>(this Dictionary<int, TValue> dic)
    {
        int idx = 0;
        var l_key = new List<int>(dic.Keys);
        for (int i = 0; i < l_key.Count; i++)
        {
            if (l_key.Contains(idx) == true)
            {
                idx++;
            }
            else
            {
                break;
            }
        }
        return idx;
    }
    public static T[] ToArray<T>(this T value)
    {
        return new T[] { value };
    }
    public static List<T> ToList<T>(this T[] arr)
    {
        List<T> list = new List<T>(arr.Length);
        for (int i = 0; i < arr.Length; i++)
        {
            list.Add(arr[i]);
        }
        return list;
    }
    public static bool IsNullOrEmptyOrN(this string str)
    {
        if (string.IsNullOrEmpty(str) || str == "N")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Empty<T>(this T[] arr)
    {
        if (arr == null || arr.Length == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool Empty<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void Shuffle<T>(this List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    public static void Shuffle<T>(this T[] array)
    {
        System.Random random = new System.Random();
        int n = array.Length;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
    public static void Overlap<TKey, TValue>(this Dictionary<TKey, TValue> d_origin, Dictionary<TKey, TValue> d_overdic)
    {
        Debug.Log("덮기 갯수 : " + d_overdic.Count);
        foreach (TKey key in d_overdic.Keys)
        {
            if (d_overdic[key] == null)
            {
                Debug.Log("제거 : " + key);
                d_origin.Remove(key);
            }
            else
            {
                Debug.Log("변경 : " + key);
                d_origin[key] = d_overdic[key];
            }
        }
    }

    public static Vector3 Vt3(this Vector2 vt2, float z = 0)
    {
        return new Vector3(vt2.x, vt2.y, z);
    }
    public static Vector2 Vt2(this Vector3 vt3)
    {
        return new Vector2(vt3.x, vt3.y);
    }
    public static float Angle(this Vector2 vt2)
    {
        float angle = 0;
        if (vt2 == Vector2.zero)
        {
            angle = 0;
        }
        if (vt2.y == 0)
        {
            if (vt2.x > 0)
            {
                angle = 0;
            }
            else
            {
                angle = 180;
            }
        }
        else
        {
            float dis = Vector2.Distance(Vector2.zero, vt2);
            if (vt2.y > 0)
            {
                angle = Mathf.Acos(vt2.x / dis) * Mathf.Rad2Deg;
            }
            else
            {
                angle = Mathf.Acos(-vt2.x / dis) * Mathf.Rad2Deg + 180;
            }
        }

        if (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }

    /// <summary>
    /// 0~360
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static float ToAngleValue(this float value)
    {
        if (value < 0)
        {
            while (true)
            {
                value += 360;
                if (value >= 0)
                {
                    return value;
                }
            }
        }
        else if (value >= 360)
        {
            while (true)
            {
                value -= 360;
                if (value <= 360)
                {
                    return value;
                }
            }
        }
        else return value;
    }

    public static Vector2 AngleToVt2(this float angle)
    {
        angle = angle.ToAngleValue();
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

}

/// <summary>
/// 직렬 병렬 선택가능
/// 진행도 가져오기 가능
/// </summary>
public class L_Task
{
    public List<Func<Task>> l_fc_task = new List<Func<Task>>();
    public float Success_Per; //직렬에서만 사용가능

    /// <summary>
    /// 직렬
    /// </summary>
    /// <returns></returns>
    public async Task Invoke_Serial()
    {
        Success_Per = 0;
        for (int i = 0; i < l_fc_task.Count; i++)
        {
            await l_fc_task[i]();
            Success_Per = (i + 1) / l_fc_task.Count;
        }
        Success_Per = 100;
    }

    /// <summary>
    /// 병렬
    /// </summary>
    /// <returns></returns>
    public async Task Invoke_Parallel()
    {
        Task[] arr_task = new Task[l_fc_task.Count];
        for (int i = 0; i < l_fc_task.Count; i++)
        {
            arr_task[i] = l_fc_task[i]();
        }
        await Task.WhenAll(arr_task);
    }

    public void Add(Func<Task> fc)
    {
        if (fc != null)
        {
            l_fc_task.Add(fc);
        }
    }
    public void Remove(Func<Task> fc)
    {
        if (fc != null)
        {
            l_fc_task.Remove(fc);
        }
    }
}


