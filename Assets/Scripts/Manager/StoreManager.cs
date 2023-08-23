using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
public class StoreManager : Manager<StoreManager>
{
    private void Awake()
    {
        User_Store.m_UserStore = new User_Store();
        StreamingManager.LT_StrLoad.Add_Task(new Task(() =>
        {
            StreamingManager.Read_Data<J_StoreData>(StreamingManager.Get_StreamingPath(J_StoreData.Path), StoreData.Data_DicSet);
        }));
    }

    public void Buy((int kind, int id, int count) price_info, Action ac_buy)
    {
        if (price_info.count == 0)
        {
            throw new Exception("Buy Count is 0 ( Kind : " + price_info.kind + " ID : " + price_info.id + " Count : " + price_info.count + " )");
        }
        else
        {
            User_Store.m_UserStore.Buy_Action(price_info, ac_buy);
        }
    }
    public void Buy_Advertisement(int advertisement, Action ac_buy)
    {
        //광고 완료시 ac_buy 호출
    }
    public void Buy(int id, int count = 1)
    {
        if (count == 0)
        {
            throw new Exception("Buy Count is 0. ID : " + id);
        }
        StoreData cur_store = StoreData.Get(id);
        if (cur_store.Purchase.IsNullOrEmptyOrN())
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
public class User_Store : UserData_Server
{
    public const string Path = "Store";
    public static User_Store m_UserStore;

    public List<Record> L_Purchase_Record = new List<Record>();
    public Dictionary<int, StoreLock> D_StoreLock = new Dictionary<int, StoreLock>();
    public static Action ac_StoreChanged = () => { Debug.Log("StoreChanged Action"); };
    public class Record
    {
        public string Time;
        public int ID;
    }
    public class StoreLock 
    {
        public string Time;
    }
    public override async Task Load()
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (UserManager.Use_Local)
        {
            if (UserManager.Exist_LocalUD(Path))
            {
                var data = await UserManager.Load_LocalUDAsync<User_Store>(Path);
                L_Purchase_Record = data.L_Purchase_Record;
                D_StoreLock = data.D_StoreLock;
            }
            else
            {
                Debug.Log("Store_Init");

                UserManager.Save_LocalUD(Path, this);
            }
        }
        else
        {
            //서버에서 있는지없는지 확인 후 없으면 생성해서 보내면 그거 받으면됨
        }
    }
    public async void Buy(int id, int count = 1)
    {
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Store.Buy(id, count);
            D_StoreLock = data.D_StoreLock;
            L_Purchase_Record = data.L_Purchase_Record;
            User_Inven.m_UserInven.D_Inven = data.D_Inven;
            ac_StoreChanged?.Invoke();
            User_Inven.ac_InvenChanged?.Invoke();

            if (data.D_Randombox != null)
            {
                User_Randombox.m_UserRandombox.D_Randombox = data.D_Randombox;
                User_Randombox.ac_RandomboxChanged?.Invoke();
            }
        }
        else
        {
            //server
        }
    }
    public async void Buy_Action((int kind, int id, int count) price_info, Action ac_buy)
    {
        if (UserManager.Use_Local)
        {
            User_Inven.m_UserInven.D_Inven = await LocalUser_Store.Buy(price_info);
            User_Inven.ac_InvenChanged?.Invoke();
            ac_buy();
        }
        else
        {
            //server
        }
    }
}
public class LocalUser_Store
{
    public static async Task<(Dictionary<int, User_Store.StoreLock> D_StoreLock, List<User_Store.Record> L_Purchase_Record, Dictionary<int,User_Inven.Inven> D_Inven, Dictionary<int,int> D_Randombox)> Buy(int id, int count = 1)
    {
        Debug.Log("구매");
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Store m_userstore = UserManager.Load_LocalUD<User_Store>(User_Store.Path);
        StoreData cur_store = StoreData.Get(id);

        var price_info = cur_store.Get_PriceInfo(count);
        List<(int kind, int id, int count)> reward_info = null;

        if (await LocalUser_Inven.CheckItemCount(price_info))
        {
            if (cur_store.Lock_Time != 0)
            {
                if (m_userstore.isLock(id))
                {
                    throw new Exception("This Product has TimeLock ID : " + id + " TimeLock : " + m_userstore.D_StoreLock[id].Time + " Cur Time : " + TimeManager.Instance.Cur_Time.ToString());
                }
                else
                {
                    m_userstore.AddLockTime(id, TimeManager.Instance.Cur_Time.AddSeconds(cur_store.Lock_Time));
                }
            }
            if (cur_store.Purchase.IsNullOrEmptyOrN())
            {
                m_userstore.L_Purchase_Record.Add(new User_Store.Record()
                {
                    Time = TimeManager.Instance.Cur_Time.ToString(),
                    ID = id
                });
            }


            //가격 지불
            await LocalUser_Inven.Remove_byKind(price_info);

            //상품목록 생성
            reward_info = cur_store.Get_RewardInfo_List(count);

            int test = 0;

            Dictionary<int, int> d_randombox = null;
            for (int i = 0; i < reward_info.Count; i++)
            {
                test++;
                if (test > 1000)
                {
                    throw new Exception("Buy Loop is Infinity! ID : " + id + " Count : " + count);
                }
                switch (reward_info[i].kind)
                {
                    case ItemData.RandomboxKind:
                        var data = await LocalUser_Randombox.Gacha_byBuy(reward_info[i].id, reward_info[i].count);
                        d_randombox = data.d_randombox;
                        reward_info.Add(data.item_info);
                        reward_info.RemoveAt(i);
                        i--;
                        break;
                }
            }

            //획득
            Dictionary<int,User_Inven.Inven> d_inven = await LocalUser_Inven.Add(reward_info.ToArray());

            Debug.Log("구매 완료");

            UserManager.Save_LocalUD(User_Store.Path, m_userstore);
            Debug.Log("구매 처리 끝");
            return (m_userstore.D_StoreLock, m_userstore.L_Purchase_Record, d_inven, d_randombox);
        }
        else
        {
            throw new Exception("So Expensive! ID : " + id + " Count : " + count);
        }
    }

    public static async Task<Dictionary<int, User_Inven.Inven>> Buy((int kind, int id, int count) price_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (await LocalUser_Inven.CheckItemCount(price_info.ToArray()))
        {
            //가격지불
            Dictionary<int, User_Inven.Inven> d_inven = await LocalUser_Inven.Remove_byKind(price_info.ToArray());
            return d_inven;
        }
        else
        {
            throw new Exception("So Expensive! Kind : " + price_info.kind + " ID : " + price_info.id + " Count : " + price_info.count);
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
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public static void AddLockTime(this User_Store m_userstore, int id, DateTime datetime)
    {
        if (m_userstore.D_StoreLock.ContainsKey(id))
        {
            m_userstore.D_StoreLock[id].Time = datetime.ToString();
        }
        else
        {
            m_userstore.D_StoreLock.Add(id, new User_Store.StoreLock() { Time = datetime.ToString() });
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
    public string[] Purchase;
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
    public string Purchase;
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