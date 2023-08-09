using System;
using UnityEngine;
using System.IO;
public class StreamingManager
{
    public static L_Task LT_StrLoad = new L_Task();

    public static string Get_StreamingPath(string filename)
    {
        return Application.streamingAssetsPath + "/" + filename + ".txt";
    }

    public static async void Load_StreamingData(Action ac_successed)
    {
        await LT_StrLoad.Invoke();
        ac_successed?.Invoke();
    }

    public static void Read_Data<T>(string path, Action<T> ac_dicset)
    {
        var obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
        ac_dicset.Invoke(obj);
    }
}