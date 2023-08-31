using System;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Converters;
public class StreamingManager
{
    public enum E_Type
    {
        FollowPlatform,
        Text,
        Binary
    }

    public static L_Task LT_StrLoad = new L_Task();
    static readonly string StreamingText_Path = Application.streamingAssetsPath + "/Text";
    static readonly string StreamingBinary_Path = Application.streamingAssetsPath + "/Binary";

    public static string Get_StreamingPath(string filename, E_Type type = E_Type.FollowPlatform)
    {
        switch (type)
        {
            case E_Type.FollowPlatform:
#if UNITY_EDITOR
                return StreamingText_Path + "/" + filename + ".txt";
#else 
                return StreamingBinary_Path + "/" + filename + ".bin";
#endif
            case E_Type.Text:
                return StreamingText_Path + "/" + filename + ".txt";
            case E_Type.Binary:
                return StreamingBinary_Path + "/" + filename + ".bin";
            default:
                return null;
        }
    }

    public static void ConvertTextFilesToBinary()
    {
        if (!Directory.Exists(StreamingText_Path) || !Directory.Exists(StreamingBinary_Path))
        {
            Debug.LogError("Source or target folder does not exist.");
            return;
        }

        object temp = null;
        Read_Data<object>(Get_StreamingPath(J_ItemData.Path), (obj) =>
        {
            temp = obj;
        });

        J_ItemData itemdata = temp as J_ItemData;
        if (itemdata != null)
        {

            Debug.Log(itemdata.ID[0] + " " + itemdata.ID.Length);
        }
        else
        {
            Debug.Log("아이템데이터 널");
        }

        Debug.Log(JsonUtility.ToJson(temp));

        J_ItemData temp_2 = null;
        Read_Data<J_ItemData>(Get_StreamingPath(J_ItemData.Path), (obj) =>
        {
            temp_2 = obj;
        });

        itemdata = temp_2 as J_ItemData;
        if (itemdata != null)
        {

            Debug.Log(itemdata.ID[0] + " " + itemdata.ID.Length);
        }
        else
        {
            Debug.Log("아이템데이터 널");
        }

        Debug.Log(JsonUtility.ToJson(temp_2));

        string[] TextFiles_Path = Directory.GetFiles(StreamingText_Path, "*.txt");

        foreach (string textfile_path in TextFiles_Path)
        {
            BinaryConverter b_converter = new BinaryConverter();
            

            //string fileName = Path.GetFileNameWithoutExtension(textfile_path);
            //string binaryfile_path = Get_StreamingPath(fileName, E_Type.Binary);
            //DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(binaryfile_path));

            //if (!directoryInfo.Exists)
            //{
            //    directoryInfo.Create();
            //}

            //BinaryFormatter binaryFormatter = new BinaryFormatter();
            //FileStream fileStream = File.Open(binaryfile_path, FileMode.OpenOrCreate);
            //binaryFormatter.Serialize(fileStream, ReadData<T>(filename));
            //fileStream.Close();
        }

        Debug.Log("Conversion complete.");
    }
    public static async void Load_StreamingData(Action ac_successed)
    {
        await LT_StrLoad.Invoke();
        ac_successed?.Invoke();
    }

    public static void Read_Data<T>(string path, Action<T> ac_dicset, E_Type type = E_Type.FollowPlatform)
    {

        switch (type)
        {
            case E_Type.FollowPlatform:
#if UNITY_EDITOR
                var obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
                ac_dicset.Invoke(obj);
#else
#endif
                break;
            case E_Type.Text:
                obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
                ac_dicset.Invoke(obj);
                break;
            case E_Type.Binary:
                
                break;
        }
    }
}