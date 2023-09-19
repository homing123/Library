using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;

//ItemKind
public enum E_ItemKind
{
    Money = 1,
    SystemData = 2,
    Randombox = 100
}
public class InvenManager : Manager<InvenManager>
{

    private void Awake()
    {
        User_Inven.m_UserInven = new User_Inven();
        StreamingManager.lt_StrLoad.Add(async () =>
        {
            var data = await StreamingManager.ReadDataAsync<J_ItemData>(StreamingManager.Get_StreamingPath(J_ItemData.Path));
            ItemData.Data_DicSet(data);
        });
    }
    public async Task Add_Remove_byKind((int kind, int id, int count)[] add_info, (int kind, int id, int count)[] remove_info)
    {
        await User_Inven.m_UserInven.Add_Remove_byKind(add_info, remove_info);
    }
    public async Task Add_Remove_byKey((int kind, int id, int count)[] add_info, (int key, int count)[] remove_info)
    {
        await User_Inven.m_UserInven.Add_Remove_byKey(add_info, remove_info);
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
        Add_Remove_byKind((Test_Kind, Test_ID, Test_Count).ToArray(), null);
    }
    [ContextMenu("제거")]
    public async void Test_Remove()
    {
        if (CheckItemCount((Test_Kind, Test_ID, Test_Count).ToArray()))
        {
            await Add_Remove_byKind(null, (Test_Kind, Test_ID, Test_Count).ToArray());
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
                var data = UserManager.Load_LocalUD<User_Inven>(Path);
                D_Inven = data.D_Inven;
            }
            else
            {
                Debug.Log("Inven_Init");
                D_Inven.Add(0, new Inven()
                {
                    Kind = 1,
                    Id = 1,
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

    public async Task<(int kind, int id, int count)[]> Add_Remove_byKind((int kind, int id, int count)[] add_info, (int kind, int id, int count)[] remove_info)
    {
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Inven.Add_Remove_byKind(add_info, remove_info);
            D_Inven.Overlap(data.D_InvenChanged);
            ac_InvenChanged?.Invoke();
            return data.Replacement_Info;
        }
        else
        {
            //server
        }
    }

   
    public async Task<(int kind, int id, int count)[]> Add_Remove_byKey((int kind, int id, int count)[] add_info, (int key, int count)[] remove_info)
    {
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Inven.Add_Remove_byKey(add_info, remove_info);
            D_Inven.Overlap(data.D_InvenChanged);
            ac_InvenChanged?.Invoke();
            return data.Replacement_Info;
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
    public static async Task<(Dictionary<int, User_Inven.Inven> D_InvenChanged, (int kind, int id, int count)[] Replacement_Info)> Add_Remove_byKind((int kind, int id, int count)[] add_info, (int kind, int id, int count)[] remove_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Inven m_userinven = UserManager.Load_LocalUD<User_Inven>(User_Inven.Path);
        User_Inven.Inven inven;
        Dictionary<int, User_Inven.Inven> d_InvenChanged = new Dictionary<int, User_Inven.Inven>();

        if (remove_info.Empty() == false)
        {
            //제거처리
            int[] keys;
            for (int i = 0; i < remove_info.Length; i++)
            {
                if (m_userinven.Get(remove_info[i].kind, remove_info[i].id, out keys))
                {
                    int cur_count = remove_info[i].count;
                    for (int j = 0; j < keys.Length; j++)
                    {
                        inven = m_userinven.D_Inven[keys[j]];
                        if (inven.Count > cur_count)
                        {
                            inven.Count -= cur_count;
                            cur_count = 0;

                            d_InvenChanged[keys[j]] = m_userinven.D_Inven[keys[j]];
                            break;
                        }
                        else if (inven.Count == cur_count)
                        {
                            m_userinven.D_Inven.Remove(keys[j]);
                            cur_count = 0;

                            d_InvenChanged[keys[j]] = null;
                            break;
                        }
                        else
                        {
                            cur_count -= inven.Count;
                            m_userinven.D_Inven.Remove(keys[j]);

                            d_InvenChanged[keys[j]] = null;
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
        }

        (int kind, int id, int count)[] replacement_info = null;
        if (add_info.Empty() == false)
        {
            var add_data = m_userinven.Add(add_info, out replacement_info);
            d_InvenChanged.Overlap(add_data);
        }

        UserManager.Save_LocalUD(User_Inven.Path, m_userinven);
        return (d_InvenChanged, replacement_info);
    }
    public static async Task<(Dictionary<int, User_Inven.Inven> D_InvenChanged, (int kind, int id, int count)[] Replacement_Info)> Add_Remove_byKey((int kind, int id, int count)[] add_info, (int key, int count)[] remove_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Inven m_userinven = UserManager.Load_LocalUD<User_Inven>(User_Inven.Path);
        User_Inven.Inven inven;
        Dictionary<int, User_Inven.Inven> d_InvenChanged = new Dictionary<int, User_Inven.Inven>();

        if (remove_info.Empty() == false)
        {
            //제거처리
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
                            d_InvenChanged[remove_info[i].key] = null;
                            m_userinven.D_Inven.Remove(remove_info[i].key);
                        }
                        else
                        {
                            d_InvenChanged[remove_info[i].key] = inven;
                        }
                    }
                }
                else
                {
                    throw new Exception("Inven Remove Key is Null :  ( key : " + remove_info[i].key + " )");
                }
            }
        }

        (int kind, int id, int count)[] replacement_info = null;
        if (add_info.Empty() == false)
        {
            var add_data = m_userinven.Add(add_info, out replacement_info);
            d_InvenChanged.Overlap(add_data);
        }

        UserManager.Save_LocalUD(User_Inven.Path, m_userinven);
        return (d_InvenChanged, replacement_info);
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

    public static bool Find_Addable(this User_Inven m_userinven, int kind, int id, out int out_key)
    {
        foreach (int key in m_userinven.D_Inven.Keys)
        {
            if (m_userinven.D_Inven[key].Kind == kind && m_userinven.D_Inven[key].Id == id)
            {
                if (ItemData.Get((kind, id)).Max_Size == 0 || ItemData.Get((kind, id)).Max_Size > m_userinven.D_Inven[key].Count)
                {
                    out_key = key;
                    return true;
                }
            }
        }
        out_key = -1;
        return false;
    }

    public static bool Find(this User_Inven m_userinven, int kind, int id, out int out_key)
    {
        foreach (int key in m_userinven.D_Inven.Keys)
        {
            if (m_userinven.D_Inven[key].Kind == kind && m_userinven.D_Inven[key].Id == id)
            {
                out_key = key;
                return true;
            }
        }
        out_key = -1;
        return false;
    }
    public static Dictionary<int, User_Inven.Inven> Add(this User_Inven m_userinven, (int kind, int id, int count)[] add_info, out (int kind, int id, int count)[] replacement_info)
    {
        //획득처리
        User_Inven.Inven inven;
        int key = 0;
        List<(int kind, int id, int count)> l_replacement_info = new List<(int kind, int id, int count)>(); //대체상품으로 바뀐 아이템들

        Dictionary<int, User_Inven.Inven> d_InvenChanged = new Dictionary<int, User_Inven.Inven>();

        for (int i = 0; i < add_info.Length; i++)
        {
            ItemData item = ItemData.Get((add_info[i].kind, add_info[i].id));

            switch ((E_ItemKind)add_info[i].kind)
            {
                //케이스별로 차이가 있을경우
                default:
                    //획득 제한이 없는 경우
                    if (item.Max_Size == 0)
                    {
                        //현재 인벤에 같은종류가 있는경우
                        if (m_userinven.Find_Addable(add_info[i].kind, add_info[i].id, out key))
                        {
                            inven = m_userinven.D_Inven[key];
                            inven.Count += add_info[i].count;
                        }
                        else
                        {
                            key = m_userinven.D_Inven.Get_Idx();
                            m_userinven.D_Inven.Add(key, new User_Inven.Inven()
                            {
                                Kind = add_info[i].kind,
                                Id = add_info[i].id,
                                Count = add_info[i].count
                            });
                        }

                        d_InvenChanged[key] = m_userinven.D_Inven[key];
                    }
                    else
                    {
                        int Cur_Count = add_info[i].count;
                        switch ((ItemData.E_MaxKind)item.Max_Kind)
                        {
                            case ItemData.E_MaxKind.NextSlot:
                                //추가가능한 슬롯 (빈슬롯 제외) 이 없는지 여부 (추가가능한게 없는데 계속 찾지않기위해서)

                                bool Find_Addable_isNone = false;
                                //전부 다 채워넣을때 까지 반복
                                while (true)
                                {
                                    if (Find_Addable_isNone == false)
                                    {
                                        if (m_userinven.Find_Addable(add_info[i].kind, add_info[i].id, out key))
                                        {
                                            inven = m_userinven.D_Inven[key];
                                            //추가 가능한 슬롯이 있는 경우
                                            if (inven.Count + Cur_Count > item.Max_Size)
                                            {
                                                Cur_Count -= item.Max_Size - inven.Count;
                                                inven.Count = item.Max_Size;
                                            }
                                            else
                                            {
                                                inven.Count = inven.Count + Cur_Count;
                                                Cur_Count = 0;
                                                break;
                                            }

                                            d_InvenChanged[key] = m_userinven.D_Inven[key];
                                        }
                                        else
                                        {
                                            Find_Addable_isNone = true;
                                        }
                                    }
                                    else
                                    {
                                        //추가 가능한 슬롯이 없는 경우 새 슬롯에 넣음
                                        key = m_userinven.D_Inven.Get_Idx();

                                        if (Cur_Count > item.Max_Size)
                                        {
                                            Cur_Count -= item.Max_Size;
                                            m_userinven.D_Inven.Add(key, new User_Inven.Inven()
                                            {
                                                Kind = add_info[i].kind,
                                                Id = add_info[i].id,
                                                Count = item.Max_Size
                                            }); 
                                            d_InvenChanged[key] = m_userinven.D_Inven[key];
                                        }
                                        else
                                        {
                                            m_userinven.D_Inven.Add(key, new User_Inven.Inven()
                                            {
                                                Kind = add_info[i].kind,
                                                Id = add_info[i].id,
                                                Count = Cur_Count
                                            });
                                            Cur_Count = 0;
                                            d_InvenChanged[key] = m_userinven.D_Inven[key];
                                            break;
                                        }
                                    }
                                }
                                break;
                            case ItemData.E_MaxKind.ReplacementItem: //대체 아이템으로 처리되는경우
                                if (m_userinven.Find(add_info[i].kind, add_info[i].id, out key))
                                {
                                    //현재 사용중인 슬롯이 있는경우 추가가능한 만큼 넣고 남으면 대체아이템
                                    inven = m_userinven.D_Inven[key];
                                    if (inven.Count == item.Max_Size)
                                    {

                                    }
                                    else
                                    {
                                        if (inven.Count + Cur_Count > item.Max_Size)
                                        {
                                            Cur_Count -= item.Max_Size - inven.Count;
                                            inven.Count = item.Max_Size;
                                        }
                                        else
                                        {
                                            inven.Count = inven.Count + Cur_Count;
                                            Cur_Count = 0;
                                        }
                                        d_InvenChanged[key] = m_userinven.D_Inven[key];
                                    }
                                }
                                else
                                {
                                    //현재 사용중인 슬롯이 없는경우 새 슬롯을 만들고 추가가능한 만큼 넣고 남으면 대체 아이템

                                    key = m_userinven.D_Inven.Get_Idx();
                                    if (Cur_Count > item.Max_Size)
                                    {
                                        Cur_Count -= item.Max_Size;
                                        m_userinven.D_Inven.Add(key, new User_Inven.Inven()
                                        {
                                            Kind = add_info[i].kind,
                                            Id = add_info[i].id,
                                            Count = item.Max_Size
                                        });
                                    }
                                    else
                                    {
                                        m_userinven.D_Inven.Add(key, new User_Inven.Inven()
                                        {
                                            Kind = add_info[i].kind,
                                            Id = add_info[i].id,
                                            Count = Cur_Count
                                        });
                                        Cur_Count = 0;
                                    }
                                    d_InvenChanged[key] = m_userinven.D_Inven[key];
                                }

                                //대체되어야할 아이템 추가
                                if (Cur_Count > 0)
                                {
                                    l_replacement_info.Add((add_info[i].kind, add_info[i].id, Cur_Count));
                                }
                                break;
                        }
                       
                    }
                    break;
            }   
        }

        //대체상품 처리
        replacement_info = l_replacement_info.ToArray();

        List<(int kind, int id, int count)> l_replacement_add_info = new List<(int kind, int id, int count)>(); //대체획득상품 아이템 (획득할 아이템)

        //대체획득상품 목록화 (대체상품 키가 없는값이거나, 대체상품이 획득갯수 제한이 있는경우 오류)
        for(int i = 0; i < l_replacement_info.Count; i++)
        {
            ItemData cur_item = ItemData.Get((l_replacement_info[i].kind, l_replacement_info[i].id));

            if (cur_item.L_ReplacementItem.Count == 0)
            {
                throw new Exception($"Cur Item Replacement Item is Null : Item Key ( kind : {cur_item.Kind}, id : {cur_item.ID} )");
            }
            var arr_ReplacementItem = cur_item.L_ReplacementItem.ToArray().ItemMulCount(l_replacement_info[i].count);

            for (int j = 0; j < arr_ReplacementItem.Length; j++) 
            {
                ItemData cur_replacement_item;
                if (ItemData.Get((cur_item.L_ReplacementItem[j].kind, cur_item.L_ReplacementItem[j].id), out cur_replacement_item))
                {
                    if (cur_replacement_item.Max_Size != 0)
                    {
                        throw new Exception($"Replacement Item's max size must be 0! Replacement Item Key ( kind : {cur_item.L_ReplacementItem[j].kind}, id : {cur_item.L_ReplacementItem[j].id} )");
                    }
                    else
                    {
                        l_replacement_add_info.Add(arr_ReplacementItem[j]);
                    }
                }
                else
                {
                    throw new Exception($"Replacement Item Key is Null : Item Key ( kind : {cur_item.Kind}, id : {cur_item.ID} ), Replacement Item Key ( kind : {cur_item.L_ReplacementItem[j].kind}, id : {cur_item.L_ReplacementItem[j].id} )");
                }
            }
        }

        //대체획득 상품 목록 획득
        for (int i = 0; i < l_replacement_add_info.Count; i++)
        {
            if (m_userinven.Find_Addable(l_replacement_add_info[i].kind, l_replacement_add_info[i].id, out key))
            {
                inven = m_userinven.D_Inven[key];
                inven.Count += l_replacement_add_info[i].count;
            }
            else
            {
                key = m_userinven.D_Inven.Get_Idx();
                m_userinven.D_Inven.Add(key, new User_Inven.Inven()
                {
                    Kind = l_replacement_add_info[i].kind,
                    Id = l_replacement_add_info[i].id,
                    Count = l_replacement_add_info[i].count
                });
            }
            d_InvenChanged[key] = m_userinven.D_Inven[key];
        }
        return d_InvenChanged;
    }
}

#endregion
#region Streaming Item
[Serializable]
public class J_ItemData
{
    public const string Path = "Item";

    public int[] Kind;
    public int[] ID;
    public int[] Name;
    public int[] Description;
    public bool[] GachaLock;

    public int[] Max_Size;
    public int[] Max_Kind;

    public int[] Replacement_Kind;
    public int[] Replacement_ID;
    public int[] Replacement_Count;


}
public class ItemData
{
    //MaxKind
    public enum E_MaxKind
    {
        NextSlot = 1,
        ReplacementItem = 2,
    }

   

    public const int AttendanceQuest_Count_ID = 2;

    static Dictionary<(int kind, int id), ItemData> D_Data = new Dictionary<(int kind, int id), ItemData>();

    public int Kind;
    public int ID;
    public int Name;
    public int Description;
    public bool GachaLock;

    public int Max_Size;
    public int Max_Kind;

    public List<(int kind, int id, int count)> L_ReplacementItem = new List<(int kind, int id, int count)>();

    public static void Data_DicSet(J_ItemData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            var data_key = (j_obj.Kind[i], j_obj.ID[i]);
            if(D_Data.ContainsKey(data_key) == false)
            {
                ItemData obj = new ItemData()
                {
                    Kind = j_obj.Kind[i],
                    ID = j_obj.ID[i],
                    Name = j_obj.Name[i],
                    Description = j_obj.Description[i],
                    GachaLock = j_obj.GachaLock[i],
                    Max_Size = j_obj.Max_Size[i],
                    Max_Kind = j_obj.Max_Kind[i]
                }; 
                D_Data.Add(data_key, obj);
            }

            ItemData cur_item = D_Data[data_key];
            cur_item.L_ReplacementItem.AddItemInfo(j_obj.Replacement_Kind[i], j_obj.Replacement_ID[i], j_obj.Replacement_Count[i]);
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