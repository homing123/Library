using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class StreamingManager
{
    public enum E_Type
    {
        FollowPlatform,
        Text,
        Binary
    }

    public static L_Task lt_StrLoad = new L_Task();
    static readonly string StreamingText_Path = Application.streamingAssetsPath + "/Text";
    static readonly string StreamingBinary_Path = Application.streamingAssetsPath + "/Binary";

    public const E_Type Cur_StreamingType = E_Type.FollowPlatform;

    public static string Get_StreamingPath(string filename)
    {
        switch (Cur_StreamingType)
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

        string[] TextFiles_Path = Directory.GetFiles(StreamingText_Path, "*.txt");

        foreach (string textFile in TextFiles_Path)
        {
            // ���� �̸����� Ȯ���� �� ��θ� �����ϰ� J_�� �ٿ��� Ŭ���� �̸� ����
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(textFile);
            string className = "J_" + fileNameWithoutExtension+"Data";
            
            // �ؽ�Ʈ ������ �о�� JSON ���ڿ��� ��ȯ
            string jsonText = File.ReadAllText(textFile);

            // JSON ���ڿ��� ������ȭ�Ͽ� �ش� Ŭ������ ��ȯ
            object deserializedObject = JsonConvert.DeserializeObject(jsonText, Type.GetType(className));
            if (Type.GetType(className) == null)
            {
                Debug.Log("Ŭ���� ���� : " + className);
            }

            // ����ȭ�� �����͸� ���̳ʸ� ���Ϸ� ����
            string binaryFilePath = Path.Combine(StreamingBinary_Path, fileNameWithoutExtension + ".bin");

            using (FileStream binaryFileStream = new FileStream(binaryFilePath, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(binaryFileStream, deserializedObject);
            }

            Debug.Log($"{textFile} �� ����ȭ �� {binaryFilePath}�� ���� �Ϸ�.");
        }
        Debug.Log("Conversion complete.");
    }
    public static async Task Load_StreamingData()
    {
        await lt_StrLoad.Invoke_Serial();
        Debug.Log("�ε� ��Ʈ���� ������ �Ϸ�");
    }

    public static T ReadData<T>(string path)
    {
        switch (Cur_StreamingType)
        {
            case E_Type.FollowPlatform:
#if UNITY_EDITOR
                var obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return obj;
#elif UNITY_ANDROID
                UnityWebRequest webrequest = UnityWebRequest.Get(path);
                webrequest.SendWebRequest();
                while (!webrequest.isDone)
                {

                }
                MemoryStream ms = new MemoryStream(webrequest.downloadHandler.data);
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
#elif UNITY_IOS
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
#endif
            case E_Type.Text:
                obj = JsonUtility.FromJson<T>(File.ReadAllText(path));
                return obj;
            case E_Type.Binary:
                MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
        }
    }

    public static async Task<T> ReadDataAsync<T>(string path)
    {
       
        switch (Cur_StreamingType)
        {
            case E_Type.FollowPlatform:
#if UNITY_EDITOR
                var obj = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
                return obj;
#elif UNITY_ANDROID
                UnityWebRequest webrequest = UnityWebRequest.Get(path);
                webrequest.SendWebRequest();
                while (!webrequest.isDone)
                {
                    await Task.Delay(1);
                }
                MemoryStream ms = new MemoryStream(webrequest.downloadHandler.data);
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
#elif UNITY_IOS
                MemoryStream ms = new MemoryStream(await File.ReadAllBytesAsync(path));
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
#endif
            case E_Type.Text:
                obj = JsonUtility.FromJson<T>(await File.ReadAllTextAsync(path));
                return obj;
            case E_Type.Binary:
                MemoryStream ms = new MemoryStream(await File.ReadAllBytesAsync(path));
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
        }
    }
}