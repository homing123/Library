using System.Collections.Generic;
using UnityEngine;
using System;

public class InvenManager : Manager<InvenManager>
{

    public User_Inven m_UserInven;

    private void Awake()
    {

        UserManager.Add_Local(User_Inven.LocalPath, Init_UD, () => UserManager.Save_LocalUD(User_Inven.LocalPath, m_UserInven), () => m_UserInven = UserManager.Load_LocalUD<User_Inven>(User_Inven.LocalPath));

        StreamingManager.Read_Data<J_ItemData>(StreamingManager.Get_StreamingPath(J_ItemData.Path), ItemData.Data_DicSet);
    }

    void Init_UD()
    {
        m_UserInven = new User_Inven();
    }

    #region Add override
    public void Add((int kind, int id, int count)[] add_info)
    {
        m_UserInven.Add(add_info);
    }
    public void Add(List<(int kind, int id, int count)> add_info)
    {
        m_UserInven.Add(add_info.ToArray());
    }

    public void Add((int kind, int id, int count) add_info)
    {
        m_UserInven.Add(new (int kind, int id, int count)[] { add_info });
    }

    
    #endregion
    #region Remove override
    public void Remove_bykind((int kind, int id, int count)[] remove_info )
    {
        var _remove_info = new (int key, int count)[remove_info.Length];
        for (int i = 0; i < remove_info.Length; i++)
        {
            _remove_info[i].key = m_UserInven.Get_Key(remove_info[i].kind, remove_info[i].id);
            _remove_info[i].count = remove_info[i].count;
        }

        m_UserInven.Remove(_remove_info);
    }
    public void Remove_bykind((int kind, int id, int count) remove_info)
    {
        var _remove_info = new (int key, int count)[1] { (m_UserInven.Get_Key(remove_info.kind, remove_info.id), remove_info.count) };
        m_UserInven.Remove(_remove_info);
    }
    public void Remove_bykind(List<(int kind, int id, int count)> remove_info)
    {
        var _remove_info = new (int key, int count)[remove_info.Count];
        for (int i = 0; i < remove_info.Count; i++)
        {
            _remove_info[i].key = m_UserInven.Get_Key(remove_info[i].kind, remove_info[i].id);
            _remove_info[i].count = remove_info[i].count;
        }

        m_UserInven.Remove(_remove_info);
    }
    public void Remove_byKey((int key, int count)[] remove_info )
    {
        m_UserInven.Remove(remove_info);
    }
    public void Remove_byKey((int key, int count) remove_info)
    {
        m_UserInven.Remove(new (int key, int count)[] { remove_info });
    }
    public void Remove_byKey(List<(int key, int count)> remove_info)
    {
        m_UserInven.Remove(remove_info.ToArray());
    }
   
    #endregion
    #region AvailablePurchase override
    public bool AvailablePurchase((int kind, int id, int count)[] price_info)
    {
        return m_UserInven.AvailablePurchase(price_info);
    }
    public bool AvailablePurchase((int kind, int id, int count) price_info)
    {
        return m_UserInven.AvailablePurchase(new (int kind, int id, int count)[] { price_info });
    }
    public bool AvailablePurchase(List<(int kind, int id, int count)> price_info)
    {
        return m_UserInven.AvailablePurchase(price_info.ToArray());
    }
   
    #endregion
   
    

}
//[System.Serializable]
public class User_Inven
{
    static string m_localpath;
    public static string LocalPath
    {
        get
        {
            if (m_localpath == null)
            {
                m_localpath = Application.persistentDataPath + "/Inven.txt";
            }
            return m_localpath;
        }
    }
    public Dictionary<int, Inven> D_Inven = new Dictionary<int, Inven>();
    //[System.Serializable]
    public class Inven
    {
        public int Kind;
        public int Id;
        public int Count;
    }

    bool Find_Addable(int kind, int id, out Inven out_inven)
    {
        foreach(Inven inven in D_Inven.Values)
        {
            if (inven.Kind == kind && inven.Id == id)
            {
                if(ItemData.Get((kind, id)).Overlap_Size == 0  || ItemData.Get((kind,id)).Overlap_Size > inven.Count)
                {
                    out_inven = inven;
                    return true;
                }
            }
        }
        out_inven = null;
        return false;
    }
    public int Get_Key(int kind, int id = 0)
    {
        foreach(int key in D_Inven.Keys)
        {
            if(D_Inven[key].Kind == kind && D_Inven[key].Id == id)
            {
                return key;
            }
        }
        return -1;
    }
    bool Get(int key, out Inven inven)
    {
        if (D_Inven.ContainsKey(key))
        {
            inven = D_Inven[key];
            return true;
        }
        else
        {
            inven = null;
            return false;
        }
    }

    public void Add((int kind, int id, int count)[] add_info)
    {
        User_Inven.Inven inven;
        for (int i = 0; i < add_info.Length; i++)
        {
            ItemData item = ItemData.Get((add_info[i].kind, add_info[i].id));
            if (item.Overlap_Size == 0)
            {
                if (Find_Addable(add_info[i].kind, add_info[i].id, out inven))
                {
                    inven.Count += add_info[i].count;
                }
                else
                {
                    D_Inven.Add(D_Inven.Get_Idx(), new User_Inven.Inven()
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
                    if (Find_Addable_is_None == false && Find_Addable(add_info[i].kind, add_info[i].id, out inven))
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
                            D_Inven.Add(D_Inven.Get_Idx(), new User_Inven.Inven()
                            {
                                Kind = add_info[i].kind,
                                Id = add_info[i].id,
                                Count = item.Overlap_Size
                            });

                        }
                        else
                        {
                            D_Inven.Add(D_Inven.Get_Idx(), new User_Inven.Inven()
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
        SaveData();
    }
    public void Remove((int key, int count)[] remove_info)
    {
        User_Inven.Inven inven;
        for (int i = 0; i < remove_info.Length; i++)
        {
            if (Get(remove_info[i].key, out inven))
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
                        D_Inven.Remove(remove_info[i].key);
                    }
                }
            }
            else
            {
                throw new Exception("Inven Remove Key is Null :  ( key : " + remove_info[i].key + " )");
            }
        }
        SaveData();
    }
    public bool AvailablePurchase((int kind, int id, int count)[] price_info)
    {
        //overlapsize 가 무한인것만 가격에 올수있음
        User_Inven.Inven inven;
        for (int i = 0; i < price_info.Length; i++)
        {
            int key = Get_Key(price_info[i].kind, price_info[i].id);
            if (key == -1 || Get(key, out inven) == false || inven.Count < price_info[i].count)
            {
                return false;
            }
        }
        return true;
    }
    void SaveData()
    {
        UserManager.Save_LocalUD(LocalPath, this);
    }

}
public class J_ItemData
{
    public const string Path = "Inven";

    public int[] Kind;
    public int[] ID;
    public int[] Overlap_Size;
    public int[] Name;

   
}
public class ItemData
{
    static Dictionary<(int kind, int id), ItemData> D_Data = new Dictionary<(int kind, int id), ItemData>();

    public int Kind;
    public int ID;
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
}

