using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum E_Language 
{ 
    Eng = 0,
    Kor = 1,
}

public class LanguageManager : Manager<LanguageManager>
{
    public User_Language m_UserLanguage;
    public static event EventHandler<E_Language> ev_LanguageChanged;

    private void Awake()
    {
        UserManager.Add_Local(User_Language.LocalPath, Init_UD, () => UserManager.Save_LocalUD(User_Language.LocalPath, m_UserLanguage), () => m_UserLanguage = UserManager.Load_LocalUD<User_Language>(User_Language.LocalPath));

        StreamingManager.Read_Data<J_LanguageData>(StreamingManager.Get_StreamingPath(J_LanguageData.Path), LanguageData.Data_DicSet);
    }

    public void ChangeLanguage(E_Language e_lang)
    {
        m_UserLanguage.ChangeLanguage(e_lang);
        ev_LanguageChanged?.Invoke(this, e_lang);
    }
  
    void Init_UD()
    {
        Debug.Log("ÀÌ´Ö ·©±ÍÁö");
        m_UserLanguage = new User_Language();
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean:
                m_UserLanguage.Language = E_Language.Kor;
                break;
            default:
                m_UserLanguage.Language = E_Language.Eng;
                break;
        }
    }


    #region Test

    [SerializeField] E_Language Test_Language;
    [ContextMenu("¾ð¾î º¯°æ")]
    public void Test_Kor()
    {
        ChangeLanguage(Test_Language);
    }
    [ContextMenu("·Î±×")]
    public void Test_Log()
    {
        Debug.Log("ÇöÀç ¾ð¾î : " + m_UserLanguage.Language);
    }
    #endregion
}
public class User_Language
{
    static string m_localpath;
    public static string LocalPath
    {
        get
        {
            if (m_localpath == null)
            {
                m_localpath = Application.persistentDataPath + "/Language.txt";
            }
            return m_localpath;
        }
    }
    public E_Language Language;

    public void ChangeLanguage(E_Language language)
    {
        Language = language;
        SaveData();
    }
    void SaveData()
    {
        UserManager.Save_LocalUD(LocalPath, this);
    }
}
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
        if (D_Data[LanguageManager.Instance.m_UserLanguage.Language].ContainsKey(key))
        {
            return D_Data[LanguageManager.Instance.m_UserLanguage.Language][key].Text;
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