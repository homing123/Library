using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
public enum E_Language 
{ 
    Eng = 0,
    Kor = 1,
}

public class LanguageManager : Manager<LanguageManager>
{
    public static event EventHandler<E_Language> ev_LanguageChanged;

    private void Awake()
    {
        User_Language.m_UserLanguage = new User_Language();
        StreamingManager.LT_StrLoad.Add_Task(new Task(() =>
        {
            StreamingManager.Read_Data<J_LanguageData>(StreamingManager.Get_StreamingPath(J_LanguageData.Path), LanguageData.Data_DicSet);
        }));
    }

    public void ChangeLanguage(E_Language e_lang)
    {
        User_Language.m_UserLanguage.ChangeLanguage(e_lang);
        ev_LanguageChanged?.Invoke(this, e_lang);
    }
  
   

    #region Test

    [SerializeField] E_Language Test_Language;
    [ContextMenu("언어 변경")]
    public void Test_Kor()
    {
        ChangeLanguage(Test_Language);
    }
    [ContextMenu("로그")]
    public void Test_Log()
    {
        Debug.Log("현재 언어 : " + User_Language.m_UserLanguage.Language);
    }
    #endregion
}
#region UD Language
public class User_Language : UserData_Local
{
    public const string Path = "Language";
    public static User_Language m_UserLanguage;
    public E_Language Language;

  

    public override void Load()
    {
        if (UserManager.Exist_LocalUD(Path))
        {
            var data = UserManager.Load_LocalUD<User_Language>(Path);
            Language = data.Language;
        }
        else
        {
            Debug.Log("Language_Init");

            switch (Application.systemLanguage)
            {
                case SystemLanguage.Korean:
                    Language = E_Language.Kor;
                    break;
                default:
                    Language = E_Language.Eng;
                    break;
            }
            UserManager.Save_LocalUD(Path, this);
        }
    }
    public void ChangeLanguage(E_Language language)
    {
        Language = language;
        UserManager.Save_LocalUD(Path, this);
    }
}
#endregion
#region Streaming Language
[Serializable]
public class J_LanguageData
{
    public const string Path = "Language";

    public int[] ID;
    public string[] Kor;
    public string[] Eng;
    
}

public class LanguageData
{
    static Dictionary<E_Language, Dictionary<int, LanguageData>> D_Data = new Dictionary<E_Language, Dictionary<int, LanguageData>>();

    public int ID;
    public string Text;

    public static void Data_DicSet(J_LanguageData j_obj)
    {
        D_Data.Add(E_Language.Kor, new Dictionary<int, LanguageData>());
        D_Data.Add(E_Language.Eng, new Dictionary<int, LanguageData>());
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            LanguageData kor = new LanguageData()
            {
                ID = j_obj.ID[i],
                Text = j_obj.Kor[i],
            };
            D_Data[E_Language.Kor].Add(kor.ID, kor);

            LanguageData eng = new LanguageData()
            {
                ID = j_obj.ID[i],
                Text = j_obj.Eng[i],
            };
            D_Data[E_Language.Eng].Add(eng.ID, eng);
        }
    }
    public static string Get(int key)
    {
        if (D_Data[User_Language.m_UserLanguage.Language].ContainsKey(key))
        {
            return D_Data[User_Language.m_UserLanguage.Language][key].Text;
        }
        else
        {
            if(D_Data.Count == 0)
            {
                throw new Exception("LanguageData count is 0");
            }
            else
            {
                return key.ToString() + " is null";
                //throw new Exception("LanguageData key is null : " + key);
            }
        }
    }
}
#endregion