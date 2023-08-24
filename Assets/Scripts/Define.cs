using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using System;
using Newtonsoft.Json;
public class Define
{
    public static void Init_Canvas(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
    }
}
public class Math_Define
{
    static int _Get_RandomResult(float[] per)
    {
        float total_value = 0;
        for (int i = 0; i < per.Length; i++)
        {
            if (per[i] > 0)
            {
                total_value += per[i];
            }
        }

        float random_value = UnityEngine.Random.Range(0, total_value);
        for (int i = 0; i < per.Length; i++)
        {
            random_value -= per[i];
            if (random_value < 0)
            {
                return i;
            }
        }
        throw new Exception("RandomResult Error : " + total_value + " " + random_value + " " + per.Length);
    }
    public static int Get_RandomResult(float[] per)
    {
        return _Get_RandomResult(per);
    }
    public static int Get_RandomResult(List<float> per)
    {
        return _Get_RandomResult(per.ToArray());
    }

    public static float Get_TimeValue(float curtime, float maxtime, float startvalue = 0, float endvalue = 1)
    {
        if (maxtime == 0)
        {
            return startvalue;
        }
        return startvalue + (curtime / maxtime * (endvalue - startvalue));
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
    public static void Add<T>(this List<T> list, T[] array)
    {
        if (array == null || array.Length == 0)
        {
            throw new Exception("Add array to List Error : array is null or array's length is 0");
        }
        for (int i = 0; i < array.Length; i++)
        {
            list.Add(array[i]);
        }
    }
    public static void Add<T>(this List<T> list, List<T> array)
    {
        if (array == null || array.Count == 0)
        {
            throw new Exception("Add List to List Error : List is null or List's Count is 0");
        }
        for (int i = 0; i < array.Count; i++)
        {
            list.Add(array[i]);
        }
    }

    public static async void Fc_Async(this Func<Task> func, Action ac_complete)
    {
        await func?.Invoke();
        ac_complete?.Invoke();
    }
    public static bool Empty<T>(this T[] arr)
    {
        if(arr == null || arr.Length == 0)
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
}

//동기를 병렬비동기로 바꿀때 사용
public class L_Task
{
    public List<Task> l_task = new List<Task>();
    public async Task Invoke()
    {
        for (int i = 0; i < l_task.Count; i++)
        {
            l_task[i].Start();
        }
        await Task.WhenAll(l_task);
    }

    public void Add_Task(Task task)
    {
        l_task.Add(task);
    }
    public void Remove_Task(Task task)
    {
        if (task != null)
        {
            l_task.Remove(task);
        }
    }
}

