using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;
public class StoreManager : Manager<StoreManager>
{
    private void Awake()
    {
        StreamingManager.Read_Data<J_StoreData>(StreamingManager.Get_StreamingPath(J_StoreData.Path), StoreData.Data_DicSet);
    }

    public void Buy((int kind, int id, int count) price_info, Action ac_buy)
    {
        if (price_info.count == 0)
        {
            throw new Exception("Buy Count is 0 ( Kind : " + price_info.kind + " ID : " + price_info.id + " Count : " + price_info.count + " )");
        }
        if (await InvenManager.Instance.CheckItemCount(price_info))
        {
            InvenManager.Instance.Remove_byKind(price_info);
            ac_buy();
        }
    }
    public void Buy(int id, int count = 1)
    {
        if (count == 0)
        {
            throw new Exception("Buy Count is 0. ID : " + id);
        }
        StoreData cur_store = StoreData.Get(id);
        if (cur_store.Purchase)
        {
            //결제완료 (User_Store.m_UserStore.Buy(id, count));
        }
        else if (cur_store.Advertisement)
        {
            //광고완료 (User_Store.m_UserStore.Buy(id, count));
        }
        else
        {
            User_Store.m_UserStore.Buy(id, count);
        }
    }

    #region Test
    [SerializeField] int Test_StoreID;
    [SerializeField] int Test_Count;
    [ContextMenu("구매")]
    public void Test_Buy()
    {
        Buy(Test_StoreID, Test_Count);
    }
    #endregion
}
#region UD_Store
public class User_Store : UserData
{
    public const string Path = "Store";
    public static User_Store m_UserStore;

    public List<Record> L_Purchase_Record = new List<Record>();
    public Dictionary<int, StoreLock> D_StoreLock = new Dictionary<int, StoreLock>();
    public class Record
    {
        public string Time;
        public int ID;
    }
    public class StoreLock 
    {
        public string Time;
    }
    
    public override void Init_UD()
    {

    }

    public async void Buy(int id, int count = 1)
    {
        if (UserManager.Use_Local)
        {
            LOCALUSER
        }
    }
    public async Task<User_Inven> Buy()
    {

    }
}
public class LocalUser_Store
{
    public static async Task<(Dictionary<int, User_Store.StoreLock>, List<User_Store.Record>, Dictionary<int,User_Inven.Inven>)> Buy(int id, int count = 1)
    {
        await Task.Delay(1);
        User_Store m_userstore = UserManager.Load_LocalUD<User_Store>(User_Store.Path);
        StoreData cur_store = StoreData.Get(id);

        var price_info = cur_store.Get_PriceInfo(count);
        List<(int kind, int id, int count)> reward_info = null;

        if (await LocalUser_Inven.CheckItemCount(price_info))
        {
            //가격 지불
            await LocalUser_Inven.Remove_byKind(price_info);

            //상품목록 생성
            reward_info = cur_store.Get_RewardInfo_List(count);

            for (int i = 0; i < reward_info.Count; i++)
            {
                switch (reward_info[i].kind)
                {
                    case InvenManager.RandomboxKind:
                        reward_info.Add(await RandomBoxData.Get(reward_info[i].id).Gacha());
                        reward_info.RemoveAt(i);
                        i--;
                        break;
                }
            }

            //획득
            Dictionary<int,User_Inven.Inven> d_inven = await LocalUser_Inven.Add(reward_info.ToArray());

            Debug.Log("구매 완료");

            if (cur_store.Lock_Time != 0)
            {
                if (m_userstore.isLock(id))
                {
                    throw new Exception("This Product has TimeLock ID : " + id + " TimeLock : " + m_userstore.D_StoreLock[id].Time + " Cur Time : " + TimeManager.Instance.Cur_Time.ToString());
                }
                else
                {
                    m_userstore.D_StoreLock.Add(id, new User_Store.StoreLock() { Time = TimeManager.Instance.Cur_Time.AddSeconds(cur_store.Lock_Time).ToString() });
                }
            }
            if (cur_store.Purchase)
            {
                m_userstore.L_Purchase_Record.Add(new User_Store.Record()
                {
                    Time = TimeManager.Instance.Cur_Time.ToString(),
                    ID = id
                });
            }

            UserManager.Save_LocalUD()
            Debug.Log("구매 처리 끝");
        }
        else
        {
            throw new Exception("So Expensice! ID : " + id + " Count : " + count);
        }
    }
}
public static class Ex_Store 
{
    public static bool isLock(this User_Store m_userstore, int id)
    {
        if (m_userstore.D_StoreLock.ContainsKey(id))
        {
            if (DateTime.Parse(m_userstore.D_StoreLock[id].Time).CompareTo(TimeManager.Instance.Cur_Time) > 0) // a.compareto(b) => 0보다작으면 a보다 b가이전, 0은 a==b, 0보다 크면 a보다 b가 후
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
}
#endregion
#region Streaming Store
public class J_StoreData
{
    public const string Path = "Store";

    public int[] ID;
    public int[] Type;
    public int[] Title;
    public int[] Description;
    public bool[] Purchase;
    public bool[] Advertisement;
    public int[] Lock_Time;
    public int[] Price_Kind_0;
    public int[] Price_ID_0;
    public int[] Price_Count_0;
    public int[] Price_Kind_1;
    public int[] Price_ID_1;
    public int[] Price_Count_1;
    public int[] Reward_Item_Kind_0;
    public int[] Reward_Item_ID_0;
    public int[] Reward_Item_Count_0;
    public int[] Reward_Item_Kind_1;
    public int[] Reward_Item_ID_1;
    public int[] Reward_Item_Count_1;
}
public class StoreData
{
    static Dictionary<int, StoreData> D_Data = new Dictionary<int, StoreData>();

    public int ID;
    public int Type;
    public int Title;
    public int Description;
    public bool Purchase;
    public bool Advertisement;
    public int Lock_Time;
    public (int kind, int id, int count)[] Price_Info;
    public (int kind, int id, int count)[] Reward_ItemInfo;

    public static void Data_DicSet(J_StoreData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            StoreData obj = new StoreData()
            {
                ID = j_obj.ID[i],
                Type = j_obj.Type[i],
                Title = j_obj.Title[i],
                Description = j_obj.Description[i],
                Purchase = j_obj.Purchase[i],
                Advertisement = j_obj.Advertisement[i],
                Lock_Time = j_obj.Lock_Time[i],

            };

            obj.Price_Info = InvenManager.ToItemInfo(new int[] { j_obj.Price_Kind_0[i], j_obj.Price_Kind_1[i] }, new int[] { j_obj.Price_ID_0[i], j_obj.Price_ID_1[i] }, new int[] { j_obj.Price_Count_0[i], j_obj.Price_Count_1[i] });
            obj.Reward_ItemInfo = InvenManager.ToItemInfo(new int[] { j_obj.Reward_Item_Kind_0[i], j_obj.Reward_Item_Kind_1[i] }, new int[] { j_obj.Reward_Item_ID_0[i], j_obj.Reward_Item_ID_1[i] }, new int[] { j_obj.Reward_Item_Count_0[i], j_obj.Reward_Item_Count_1[i] });

            D_Data.Add(obj.ID, obj);

        }
    }

    public static StoreData Get(int key)
    {
        if (D_Data.ContainsKey(key))
        {
            return D_Data[key];
        }
        else
        {
            if (D_Data.Count == 0)
            {
                throw new Exception("StoreData count is 0");
            }
            else
            {
                throw new Exception("StoreData key is null : " + key);
            }
        }
    }
    public static bool Get(int key, out StoreData storedata)
    {
        if (D_Data.Count == 0)
        {
            throw new Exception("StoreData count is 0");
        }
        else
        {
            if (D_Data.ContainsKey(key))
            {
                storedata = D_Data[key];
                return true;
            }
            else
            {
                storedata = null;
                return false;
            }
        }
    }

    public (int,int,int)[] Get_PriceInfo(int count = 1)
    {
        var price_info = new (int, int, int)[Price_Info.Length];
        for(int i = 0; i < Price_Info.Length; i++)
        {
            price_info[i] = (Price_Info[i].kind, Price_Info[i].id, Price_Info[i].count * count);
        }
        return price_info;
    }
    public (int,int,int)[] Get_RewardInfo(int count = 1)
    {
        var reward_info = new List<(int, int, int)>();
        for (int i = 0; i < Reward_ItemInfo.Length; i++)
        {
            reward_info.Add((Reward_ItemInfo[i].kind, Reward_ItemInfo[i].id, Reward_ItemInfo[i].count * count));
        }
        return reward_info.ToArray();
    }
    public List<(int, int, int)> Get_RewardInfo_List(int count = 1)
    {
        var reward_info = new List<(int, int, int)>();
        for (int i = 0; i < Reward_ItemInfo.Length; i++)
        {
            reward_info.Add((Reward_ItemInfo[i].kind, Reward_ItemInfo[i].id, Reward_ItemInfo[i].count * count));
        }
        return reward_info;
    }
}
#endregion