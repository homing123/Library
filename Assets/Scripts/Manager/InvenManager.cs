using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;
public class InvenManager : Manager<InvenManager>
{
    private void Awake()
    {
        User_Inven.m_UserInven = new User_Inven();
        StreamingManager.LT_StrLoad.Add_Task(new Task(() =>
        {
            StreamingManager.Read_Data<J_ItemData>(StreamingManager.Get_StreamingPath(J_ItemData.Path), ItemData.Data_DicSet);
        }));
    }

    public async Task Add((int kind, int id, int count)[] add_info)
    {
        await User_Inven.m_UserInven.Add(add_info);
    }
    public async Task Remove_byKind((int kind, int id, int count)[] remove_info)
    {
        await User_Inven.m_UserInven.Remove_byKind(remove_info);
    }
    public async Task Remove_byKey((int key, int count)[] remove_info)
    {
        await User_Inven.m_UserInven.Remove_byKey(remove_info);
    }
    public bool CheckItemCount((int kind, int id, int count)[] price_info)
    {
        return User_Inven.m_UserInven.CheckItemCount(price_info);
    }


    public static (int kind, int id, int count)[] ToItemInfo(int[] kind, int[] id, int[] count)
    {
        List<(int, int, int)> l_info = new List<(int, int, int)>();
        for (int i = 0; i < count.Length; i++)
        {
            if (count[i] != 0)
            {
                l_info.Add((kind[i], id[i], count[i]));
            }
        }
        return l_info.ToArray();
    }
  
    #region Test

    [SerializeField] int Test_Kind;
    [SerializeField] int Test_ID;
    [SerializeField] int Test_Count;
    [ContextMenu("획득")]
    public void Test_Add()
    {
        Add((Test_Kind, Test_ID, Test_Count).ToArray());
    }
    [ContextMenu("제거")]
    public async void Test_Remove()
    {
        if (CheckItemCount((Test_Kind, Test_ID, Test_Count).ToArray()))
        {
            await Remove_byKind((Test_Kind, Test_ID, Test_Count).ToArray());
        }
        else
        {
            Debug.Log("제거 불가능");
        }
    }
    [ContextMenu("로그")]
    public void Test_Log()
    {
        foreach (int key in User_Inven.m_UserInven.D_Inven.Keys)
        {
            Debug.Log("키 : " + key + " kind : " + User_Inven.m_UserInven.D_Inven[key].Kind + " id : " + User_Inven.m_UserInven.D_Inven[key].Id + " count : " + User_Inven.m_UserInven.D_Inven[key].Count);
        }
    }
    #endregion

}
#region UD_Inven

public class User_Inven : UserData_Server
{
    public const string Path = "Inven";

    public static User_Inven m_UserInven;
    public static Action ac_InvenChanged = () =>
    {
        Debug.Log("InvenChanged Action");
    };

    public Dictionary<int, Inven> D_Inven = new Dictionary<int, Inven>();
    //[System.Serializable]
    public class Inven
    {
        public int Kind;
        public int Id;
        public int Count;
    }

    public override async Task Load()
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (UserManager.Use_Local)
        {
            if (UserManager.Exist_LocalUD(Path))
            {
                var data = await UserManager.Load_LocalUDAsync<User_Inven>(Path);
                D_Inven = data.D_Inven;
            }
            else
            {
                Debug.Log("Inven_Init");
                D_Inven.Add(0, new Inven()
                {
                    Kind = 1,
                    Id = 0,
                    Count = 1000
                });
                UserManager.Save_LocalUD(Path, this);
            }
        }
        else
        {
            //서버에서 있는지없는지 확인 후 없으면 생성해서 보내면 그거 받으면됨
        }
    }

    public async Task Add((int kind, int id, int count)[] add_info)
    {
        if (UserManager.Use_Local)
        {
            m_UserInven.D_Inven = await LocalUser_Inven.Add(add_info);
            ac_InvenChanged?.Invoke();
        }
        else
        {
            //server
        }
    }

    public async Task Remove_byKind((int kind, int id, int count)[] remove_info)
    {
        if (UserManager.Use_Local)
        {
            m_UserInven.D_Inven = await LocalUser_Inven.Remove_byKind(remove_info);
            ac_InvenChanged?.Invoke();
        }
        else
        {
            //server
        }
    }
    public async Task Remove_byKey((int key, int count)[] remove_info)
    {
        if (UserManager.Use_Local)
        {
            m_UserInven.D_Inven = await LocalUser_Inven.Remove_byKey(remove_info);
            ac_InvenChanged?.Invoke();
        }
        else
        {
            //server
        }
    }
    public bool CheckItemCount((int kind, int id, int count)[] item_info)
    {
        int[] keys;
        for (int i = 0; i < item_info.Length; i++)
        {
            if (m_UserInven.Get(item_info[i].kind, item_info[i].id, out keys) == false)
            {
                return false;
            }
            else
            {
                int count = 0;
                for (int j = 0; j < keys.Length; j++)
                {
                    count += D_Inven[keys[j]].Count;
                }
                if (count < item_info[i].count)
                {
                    return false;
                }
            }
        }
        return true;
    }

}
public class LocalUser_Inven
{
    public static async Task<bool> CheckItemCount((int kind, int id, int count)[] item_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Inven m_userinven = UserManager.Load_LocalUD<User_Inven>(User_Inven.Path);
        int[] keys;
        for (int i = 0; i < item_info.Length; i++)
        {
            if (m_userinven.Get(item_info[i].kind, item_info[i].id, out keys) == false)
            {
                return false;
            }
            else
            {
                int count = 0;
                for (int j = 0; j < keys.Length; j++)
                {
                    count += m_userinven.D_Inven[keys[j]].Count;
                }
                if (count < item_info[i].count)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public static async Task<Dictionary<int, User_Inven.Inven>> Add((int kind, int id, int count)[] add_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Inven m_userinven = UserManager.Load_LocalUD<User_Inven>(User_Inven.Path);
        User_Inven.Inven inven;
        for (int i = 0; i < add_info.Length; i++)
        {
            ItemData item = ItemData.Get((add_info[i].kind, add_info[i].id));
            if (item.Overlap_Size == 0)
            {
                if (m_userinven.Find_Addable(add_info[i].kind, add_info[i].id, out inven))
                {
                    inven.Count += add_info[i].count;
                }
                else
                {
                    m_userinven.D_Inven.Add(m_userinven.D_Inven.Get_Idx(), new User_Inven.Inven()
                    {
                        Kind = add_info[i].kind,
                        Id = add_info[i].id,
                        Count = add_info[i].count
                    });
                }
            }
            else
            {
                while (true)
                {
                    bool Find_Addable_is_None = false; //추가가능한게 더이상 없는경우
                    if (Find_Addable_is_None == false && m_userinven.Find_Addable(add_info[i].kind, add_info[i].id, out inven))
                    {
                        if (inven.Count + add_info[i].count > item.Overlap_Size)
                        {
                            add_info[i].count -= item.Overlap_Size - inven.Count;
                            inven.Count = item.Overlap_Size;
                        }
                        else
                        {
                            inven.Count = inven.Count + add_info[i].count;
                            break;
                        }
                    }
                    else
                    {
                        Find_Addable_is_None = true;
                        if (add_info[i].count > item.Overlap_Size)
                        {
                            add_info[i].count -= item.Overlap_Size;
                            m_userinven.D_Inven.Add(m_userinven.D_Inven.Get_Idx(), new User_Inven.Inven()
                            {
                                Kind = add_info[i].kind,
                                Id = add_info[i].id,
                                Count = item.Overlap_Size
                            });

                        }
                        else
                        {
                            m_userinven.D_Inven.Add(m_userinven.D_Inven.Get_Idx(), new User_Inven.Inven()
                            {
                                Kind = add_info[i].kind,
                                Id = add_info[i].id,
                                Count = add_info[i].count
                            });
                            break;
                        }
                    }
                }
            }
        }
        UserManager.Save_LocalUD(User_Inven.Path, m_userinven);
        return m_userinven.D_Inven;
    }

    public static async Task<Dictionary<int, User_Inven.Inven>> Remove_byKey((int key, int count)[] remove_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Inven m_userinven = UserManager.Load_LocalUD<User_Inven>(User_Inven.Path);
        User_Inven.Inven inven;
        for (int i = 0; i < remove_info.Length; i++)
        {
            if (m_userinven.Get(remove_info[i].key, out inven))
            {
                if (inven.Count < remove_info[i].count)
                {
                    throw new Exception("Inven Remove Count Error :  ( key : " + remove_info[i].key + " count : " + remove_info[i].count + " Inven Count : " + inven.Count + " )");
                }
                else
                {
                    inven.Count -= remove_info[i].count;
                    if (inven.Count == 0)
                    {
                        m_userinven.D_Inven.Remove(remove_info[i].key);
                    }
                }
            }
            else
            {
                throw new Exception("Inven Remove Key is Null :  ( key : " + remove_info[i].key + " )");
            }
        }
        UserManager.Save_LocalUD(User_Inven.Path, m_userinven);
        return m_userinven.D_Inven;
    }
    public static async Task<Dictionary<int, User_Inven.Inven>> Remove_byKind((int kind, int id, int count)[] remove_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Inven m_userinven = UserManager.Load_LocalUD<User_Inven>(User_Inven.Path);
        int[] keys;

        for (int i = 0; i < remove_info.Length; i++)
        {
            if (m_userinven.Get(remove_info[i].kind, remove_info[i].id, out keys))
            {
                int cur_count = remove_info[i].count;
                for (int j = 0; j < keys.Length; j++)
                {
                    User_Inven.Inven cur_inven = m_userinven.D_Inven[keys[j]];
                    if (cur_inven.Count > cur_count)
                    {
                        cur_inven.Count -= cur_count;
                        cur_count = 0;
                        break;
                    }
                    else if (cur_inven.Count == cur_count)
                    {
                        m_userinven.D_Inven.Remove(keys[j]);
                        cur_count = 0;
                        break;
                    }
                    else
                    {
                        cur_count -= cur_inven.Count;
                        m_userinven.D_Inven.Remove(keys[j]);
                    }
                }
                if (cur_count > 0)
                {
                    throw new Exception("Inven Remove Count Error :  ( kind : " + remove_info[i].kind + " id : " + remove_info[i].id + " count : " + remove_info[i].count + " )");
                }
            }
            else
            {
                throw new Exception("Inven Remove Key is Null :  ( kind : " + remove_info[i].kind + " id : " + remove_info[i].id + " count : " + remove_info[i].count + " )");
            }
        }
        UserManager.Save_LocalUD(User_Inven.Path, m_userinven);
        return m_userinven.D_Inven;
    }
}
public static class Ex_Inven
{
    public static bool Get(this User_Inven m_userinven, int key, out User_Inven.Inven inven)
    {
        if (m_userinven.D_Inven.ContainsKey(key))
        {
            inven = m_userinven.D_Inven[key];
            return true;
        }
        else
        {
            inven = null;
            return false;
        }
    }
    public static bool Get(this User_Inven m_userinven, int kind, int id, out int[] keys)
    {
        List<User_Inven.Inven> l_inven = new List<User_Inven.Inven>();
        List<int> l_key = new List<int>();
        foreach (int key in m_userinven.D_Inven.Keys)
        {
            if (m_userinven.D_Inven[key].Kind == kind && m_userinven.D_Inven[key].Id == id)
            {
                if (l_inven.Count == 0)
                {
                    l_inven.Add(m_userinven.D_Inven[key]);
                    l_key.Add(key);
                }
                else
                {
                    for (int i = 0; i < l_inven.Count; i++)
                    {
                        if (l_inven[i].Count > m_userinven.D_Inven[key].Count)
                        {
                            l_inven.Insert(i, m_userinven.D_Inven[key]);
                            l_key.Insert(i, key);
                            break;
                        }
                        if (i == l_inven.Count - 1)
                        {
                            l_inven.Add(m_userinven.D_Inven[key]);
                            l_key.Add(key);
                            break;
                        }
                    }
                }
            }
        }
        keys = l_key.ToArray();
        if (l_inven.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool Find_Addable(this User_Inven m_userinven, int kind, int id, out User_Inven.Inven out_inven)
    {
        foreach (User_Inven.Inven inven in m_userinven.D_Inven.Values)
        {
            if (inven.Kind == kind && inven.Id == id)
            {
                if (ItemData.Get((kind, id)).Overlap_Size == 0 || ItemData.Get((kind, id)).Overlap_Size > inven.Count)
                {
                    out_inven = inven;
                    return true;
                }
            }
        }
        out_inven = null;
        return false;
    }
}

#endregion
#region Streaming Item
public class J_ItemData
{
    public const string Path = "Item";

    public int[] Kind;
    public int[] ID;
    public bool[] GachaLock;
    public int[] Overlap_Size;
    public int[] Name;


}
public class ItemData
{
    public const int MoneyKind = 1;
    public static readonly (int kind, int id) AttendanceQuest_Count = (2, 2);
    public const int RandomboxKind = 100;

    static Dictionary<(int kind, int id), ItemData> D_Data = new Dictionary<(int kind, int id), ItemData>();

    public int Kind;
    public int ID;
    public bool GachaLock;
    public int Overlap_Size;
    public int Name;

    public static void Data_DicSet(J_ItemData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            ItemData obj = new ItemData()
            {
                Kind = j_obj.Kind[i],
                ID = j_obj.ID[i],
                GachaLock = j_obj.GachaLock[i],
                Overlap_Size = j_obj.Overlap_Size[i],
                Name = j_obj.Name[i]
            };
            D_Data.Add((obj.Kind, obj.ID), obj);
        }
    }

    public static ItemData Get((int kind, int id) key)
    {
        if (D_Data.ContainsKey(key))
        {
            return D_Data[key];
        }
        else
        {
            if (D_Data.Count == 0)
            {
                throw new Exception("ItemData count is 0");
            }
            else
            {
                throw new Exception("ItemData key is null : " + key.kind + " " + key.id);
            }
        }
    }
    public static bool Get((int kind, int id) key, out ItemData itemdata)
    {
        if (D_Data.Count == 0)
        {
            throw new Exception("ItemData count is 0");
        }
        else
        {
            if (D_Data.ContainsKey(key))
            {
                itemdata = D_Data[key];
                return true;
            }
            else
            {
                itemdata = null;
                return false;
            }
        }
    }
    public static (int kind, int id)[] GetItems_byKind(int kind, bool israndombox = false)
    {
        List<(int kind, int id)> l_items = new List<(int kind, int id)>();

        foreach (ItemData itemdata in D_Data.Values)
        {
            if (itemdata.Kind == kind)
            {
                if (israndombox)
                {
                    if (itemdata.GachaLock == false)
                    {
                        l_items.Add((itemdata.Kind, itemdata.ID));
                    }
                }
            }
        }
        return l_items.ToArray();
    }

    public static (int kind, int id, int count)[] GetItems_byKind_WithCount(int kind, bool israndombox = false)
    {
        List<(int kind, int id, int count)> l_items = new List<(int kind, int id, int count)>();

        foreach (ItemData itemdata in D_Data.Values)
        {
            if (itemdata.Kind == kind)
            {
                if (israndombox)
                {
                    if (itemdata.GachaLock == false)
                    {
                        l_items.Add((itemdata.Kind, itemdata.ID, 1));
                    }
                }
            }
        }
        return l_items.ToArray();
    }
}

#endregion