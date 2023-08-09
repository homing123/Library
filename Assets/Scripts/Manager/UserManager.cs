using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
public class UserManager
{
    public static List<(string path, Action ac_init, Action ac_save, Action ac_load)> L_Local = new List<(string path, Action ac_init, Action ac_save, Action ac_load)>();
    
    public static void Add_Local(string path, Action ac_init, Action ac_save, Action ac_load)
    {
        L_Local.Add((path, ac_init, ac_save, ac_load));
    }
    public static void Reset_Data()
    {
        for (int i = 0; i < L_Local.Count; i++)
        {
            L_Local[i].ac_init();
            L_Local[i].ac_save();
        }
    }
    public static void Get_LocalData()
    {
        for(int i = 0; i < L_Local.Count; i++)
        {
            if (File.Exists(L_Local[i].path))
            {
                L_Local[i].ac_load();
            }
            else
            {
                L_Local[i].ac_init();
                L_Local[i].ac_save();
            }
        }
    }

    public static void Delete_Local()
    {
        File.Delete(User_Inven.LocalPath);
        File.Delete(User_Language.LocalPath);
        File.Delete(User_Time.LocalPath);
    }

    public static T Load_LocalUD<T>(string localpath)
    {
        string data = File.ReadAllText(localpath);
        return JsonConvert.DeserializeObject<T>(data);
    }

    public static void Save_LocalUD(string localpath, object obj)
    {
        File.WriteAllText(localpath, JsonConvert.SerializeObject(obj));
    }

}
