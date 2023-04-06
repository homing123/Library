using UnityEngine;
using System.Collections.Generic;
using System.IO;
using static Data_User;

public class Data_User
{
    public static Data_User UserData;
    public int Daily_Update_Year; //일일갱신 년도
    public int Daily_Update_Month; //일일갱신 월
    public int Daily_Update_Day; //일일갱신 일

    public int Attendance_Count; //출석일수
    public int Access_Time; //접속시간 (초)

    public E_Language_Kind Language_Kind; //언어 종류
    public bool Sound_On;
    public bool BGM_On;
    public bool Vibration_On;

    public Inventory[] Arr_Inventory;
    public class Inventory 
    {
        public E_ItemKind Item_Kind;
        public int Item_ID;
        public int Count;

    }
    public class Quest
    {
        public int Quest_ID;
        public int Progress;
    }

    public List<Quest> L_Quest;
    public List<int> L_Clear_Quest;

}

public class UD_Func
{
    #region Time
    public static System.DateTime Get_Daily_Update()
    {
        return new System.DateTime(UserData.Daily_Update_Year, UserData.Daily_Update_Month, UserData.Daily_Update_Day);
    }
    #endregion
    #region Inventory
    // 인벤 아이템클래스 가져오는 함수 같은아이템 없을경우 null 리턴
    public static Data_User.Inventory Inven_Get_Item(E_ItemKind kind, int item_id)
    {
        for(int i = 0; i < Data_User.UserData.Arr_Inventory.Length; i++)
        {
            if(Data_User.UserData.Arr_Inventory[i].Item_Kind==kind && Data_User.UserData.Arr_Inventory[i].Item_ID == item_id)
            {
                return Data_User.UserData.Arr_Inventory[i];
            }
        }
        return null;
    }
    //인벤 빈 배열 아이템클래스 가져오는 함수 배열 빈칸 없을경우에 null 리턴
    public static Data_User.Inventory Inven_Get_Empty()
    {
        for(int i = 0; i < Data_User.UserData.Arr_Inventory.Length; i++)
        {
            if (Data_User.UserData.Arr_Inventory[i].Item_Kind == E_ItemKind.None)
            {
                return Data_User.UserData.Arr_Inventory[i];
            }
        }
        return null;
    }
    #endregion
    #region Quest

    //현재 진행중인 퀘스트 클래스 가져오는 함수 진행중이지 않을땐 null리턴
    public static Quest Quest_Get_Progress_Quest(int quest_id)
    {
        for(int i = 0; i < UserData.L_Quest.Count; i++)
        {
            if(UserData.L_Quest[i].Quest_ID== quest_id)
            {
                return UserData.L_Quest[i];
            }
        }
        return null;
    }
    //퀘스트 클리어한건지 확인하는 함수
    public static bool Quest_isClear(int quest_id)
    {
        return Data_User.UserData.L_Clear_Quest.Contains(quest_id);
    }
    #endregion
    #region User
    readonly public static string UserData_FileName = "UserData";

    public static void UserData_Save()
    {
        if (UserData != null)
        {
            File.WriteAllText(Path.Combine(Application.persistentDataPath, UserData_FileName), JsonUtility.ToJson(UserData));
        }
    }
    public static bool UserData_Load(out Data_User userdata)
    {
        string path = Path.Combine(Application.persistentDataPath, UserData_FileName);
        if (File.Exists(path))
        {
            userdata = JsonUtility.FromJson<Data_User>(File.ReadAllText(path));
            return true;
        }
        else
        {
            userdata = null;
            return false;
        }
    }
    public static void UserData_Reset()
    {
        UserData = new Data_User();
    }
    #endregion
}