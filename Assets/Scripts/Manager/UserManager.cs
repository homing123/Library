using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
public class UserManager
{
    public const bool Use_Local = true;
    public static Action ac_LoadLocal;
    public static Func<Task> fc_LoadServer;
    public static async void Load_Server(Action ac)
    {
        await fc_LoadServer.Invoke();
        ac?.Invoke();
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
    }

    public static bool Exist_LocalUD(string path)
    {
        path = Path.Combine(Application.persistentDataPath, path);
        return File.Exists(path);
    }
    public static T Load_LocalUD<T>(string path)
    {
        bool log = false;
        if (path == "Time")
        {
            log = true;
        }
        path = Path.Combine(Application.persistentDataPath, path);
        string data = File.ReadAllText(path);
        if (log)
        {
            Debug.Log(data);
        }
        return JsonConvert.DeserializeObject<T>(data);
    }
    public static async Task<T> Load_LocalUDAsync<T>(string path)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        path = Path.Combine(Application.persistentDataPath, path);
        string data = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(data);
    }
    public static void Save_LocalUD(string path, object obj)
    {
        path = Path.Combine(Application.persistentDataPath, path);
        Debug.Log("¿˙¿Â : " + path);

        File.WriteAllText(path, JsonConvert.SerializeObject(obj));
    }

}
