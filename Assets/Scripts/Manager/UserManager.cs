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
    }

    public static T Load_LocalUD<T>(string localpath)
    {
        localpath = Path.Combine(Application.persistentDataPath, localpath);
        string data = File.ReadAllText(localpath);
        return JsonConvert.DeserializeObject<T>(data);
    }
    public static async Task<T> Load_LocalUDAsync<T>(string localpath)
    {
        await Task.Delay(1);
        localpath = Path.Combine(Application.persistentDataPath, localpath);
        string data = File.ReadAllText(localpath);
        return JsonConvert.DeserializeObject<T>(data);
    }
    public static void Save_LocalUD(string localpath, object obj)
    {
        localpath = Path.Combine(Application.persistentDataPath, localpath);
        File.WriteAllText(localpath, JsonConvert.SerializeObject(obj));
    }

}
