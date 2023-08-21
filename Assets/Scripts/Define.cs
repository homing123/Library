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
        for(int i = 0; i < per.Length; i++)
        {
            if (per[i] > 0)
            {
                total_value += per[i];
            }
        }

        float random_value = UnityEngine.Random.Range(0, total_value);
        for(int i = 0; i < per.Length; i++)
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
        if(maxtime == 0)
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

public abstract class UserData
{
    public UserData()
    {
        UserManager.ac_Init += Init_UD;
    }
    public abstract void Init_UD();
}

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


public static class Ex_Define
{
    public static int Get_Idx<TValue>(this Dictionary<int, TValue> dic)
    {
        int idx = 0;
        var l_key = new List<int>(dic.Keys);
        for(int i = 0; i < l_key.Count; i++)
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

}
