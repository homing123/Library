using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using System;

public class Define
{
    public static void Init_Canvas(Canvas canvas)
    {
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
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

public class Manager<T> : MonoBehaviour where T : MonoBehaviour
{
    static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameManager.Instance.gameObject.AddComponent<T>();
            }
            return m_instance;
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
}
