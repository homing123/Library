using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
public class UserManager
{
    public const bool Use_Local = true;
    public static Action ac_LoadLocal;
    public static Func<Task> fc_LoadServer;
    public static async Task Load_Server()
    {
        await fc_LoadServer.Invoke();
    }

    public static void Delete_Local()
    {
        File.Delete(Path.Combine(Application.persistentDataPath, User_Inven.Path));
        File.Delete(Path.Combine(Application.persistentDataPath, User_Quest.Path));
        File.Delete(Path.Combine(Application.persistentDataPath, User_Store.Path));
        File.Delete(Path.Combine(Application.persistentDataPath, User_Time.Path));
        File.Delete(Path.Combine(Application.persistentDataPath, User_Sound.Path));
        File.Delete(Path.Combine(Application.persistentDataPath, User_Language.Path));
        File.Delete(Path.Combine(Application.persistentDataPath, User_Randombox.Path));
        File.Delete(Path.Combine(Application.persistentDataPath, User_Login.Path));
    }

    public static bool Exist_LocalUD(string path)
    {
        path = Path.Combine(Application.persistentDataPath, path);
        return File.Exists(path);
    }
    public static T Load_LocalUD<T>(string path)
    {
        path = Path.Combine(Application.persistentDataPath, path);
        string data = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(data);
    }
    public static void Save_LocalUD(string path, object obj)
    {
        path = Path.Combine(Application.persistentDataPath, path);
        Debug.Log("저장 : " + path);

        File.WriteAllText(path, JsonConvert.SerializeObject(obj));
    }
    public static void UD_Change<T>(Dictionary<int, T> server_dic, Dictionary<int, T> user_dic)
    {
        Debug.Log("변한거 갯수 : " + server_dic.Count);
        foreach (int key in server_dic.Keys)
        {
            if (server_dic[key] == null)
            {
                Debug.Log("UD 제거 : " + key);
                user_dic.Remove(key);
            }
            else
            {
                Debug.Log("UD 변경or추가 : " + key);
                user_dic[key] = server_dic[key];
            }
        }
    }
}
