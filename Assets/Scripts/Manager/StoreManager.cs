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
        StreamingManager.lt_StrLoad.Add(async () =>
        {
            var data = await StreamingManager.ReadDataAsync<J_StoreData>(StreamingManager.Get_StreamingPath(J_StoreData.Path));
            StoreData.Data_DicSet(data);
        });
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
    
    public async void Buy(int id, int count = 1)
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
            await AdManager.Instance.ShowAsync(AdManager.E_Adkind.Reward_Video);
        }
      
        var buy_result = await User_Store.m_UserStore.Buy(id, count);


        Debug.Log("구매 보상 : ");
        for (int i = 0; i < buy_result.Reward_Info.Length; i++)
        {
            Debug.Log(" kind : " + buy_result.Reward_Info[i].kind + " id : " + buy_result.Reward_Info[i].id + " count : " + buy_result.Reward_Info[i].count);
        }

        Debug.Log("대체된 보상 : ");
        for (int i = 0; i < buy_result.Replacement_Info.Length; i++)
        {
            Debug.Log(" kind : " + buy_result.Replacement_Info[i].kind + " id : " + buy_result.Replacement_Info[i].id + " count : " + buy_result.Replacement_Info[i].count);
        }
        //연출
    }

    #region Test
    [SerializeField] int Test_StoreID;
    [SerializeField] int Test_Count;
    [ContextMenu("구매")]
    public void Test_Buy()
    {
        Buy(Test_StoreID, Test_Count);
    }

    [ContextMenu("로그")]
    public void Test_Log()
    {
        foreach (int key in User_Store.m_UserStore.D_Store.Keys)
        {
            User_Store.Store store = User_Store.m_UserStore.D_Store[key];
            Debug.Log($"Time : {store.Time}, Count : {store.Count}");
        }
    }
    #endregion
}
#region UD_Store
public class User_Store : UserData_Server
{
    public const string Path = "Store";
    public static User_Store m_UserStore;

    public Dictionary<int, Store> D_Store = new Dictionary<int, Store>(); //구매 제한시간
    public static Action ac_StoreChanged = () => { Debug.Log("StoreChanged Action"); };

    public class Store 
    {
        public string Time;
        public int Count;
    }
    public override async Task Load()
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (UserManager.Use_Local)
        {
            if (UserManager.Exist_LocalUD(Path))
            {
                var data = UserManager.Load_LocalUD<User_Store>(Path);
                D_Store = data.D_Store;
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

    /// <summary>
    /// 구매함수
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<((int kind, int id, int count)[] Reward_Info, (int kind, int id, int count)[] Replacement_Info)> Buy(int id, int count = 1)
    {
        //구매시 인벤데이터, 랜덤박스데이터, 스토어데이터 변함
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Store.Buy(id, count);
            D_Store.Overlap(data.D_StoreChanged);
            User_Inven.m_UserInven.D_Inven.Overlap(data.D_InvenChanged);
            User_Randombox.m_UserRandombox.D_Randombox.Overlap(data.D_RandomboxChanged);
            ac_StoreChanged?.Invoke();
            User_Inven.ac_InvenChanged?.Invoke();
            User_Randombox.ac_RandomboxChanged?.Invoke();
            return (data.Reward_Info, data.Replacement_Info);
        }
        else
        {
            //server
        }
    }

    /// <summary>
    /// 가격 지불 후 함수실행
    /// </summary>
    /// <param name="price_info"></param>
    /// <param name="ac_buy"></param>
    public async void Buy_Action((int kind, int id, int count) price_info, Action ac_buy)
    {
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Store.Buy(price_info);
            User_Inven.m_UserInven.D_Inven.Overlap(data);
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
    /// <summary>
    /// 구매함수
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static async Task<(Dictionary<int, User_Store.Store> D_StoreChanged, Dictionary<int,User_Inven.Inven> D_InvenChanged, Dictionary<int,int> D_RandomboxChanged, (int kind, int id, int count)[] Reward_Info, (int kind, int id, int count)[] Replacement_Info)> Buy(int id, int count = 1)
    {
        Debug.Log("구매");
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Store m_userstore = UserManager.Load_LocalUD<User_Store>(User_Store.Path);
        StoreData cur_store = StoreData.Get(id);
        Dictionary<int, User_Store.Store> d_StoreChanged = new Dictionary<int, User_Store.Store>();

        //가격정보 가져오기
        var price_info = cur_store.L_PriceInfo.ToArray().ItemMulCount(count);

        if (await LocalUser_Inven.CheckItemCount(price_info)) //가격 지불 가능한지 확인
        {
            DateTime cur_time = await TimeManager.Instance.Cur_TimeAsync(); //현재시간 가져오기

            //구매 홧수제한 확인
            if (cur_store.Limit > 0)
            {
                if (m_userstore.isLimit(id, cur_store.Limit))
                {
                    throw new Exception($"This Product has Limit. ID : {id} UserLimit Count : {m_userstore.D_Store[id].Count} ProductLimit Count : {cur_store.Limit}");
                }
                else
                {
                    m_userstore.AddLimitCount(id);
                    d_StoreChanged[id] = m_userstore.D_Store[id];
                }
            }

            //구매 시간제한 확인
            if (cur_store.Lock_Time != 0)
            {
                if (m_userstore.isLock(id, cur_time))
                {
                    throw new Exception("This Product has TimeLock ID : " + id + " TimeLock : " + m_userstore.D_Store[id].Time + " Cur Time : " + cur_time.ToString());
                }
                else
                {
                    m_userstore.AddLockTime(id, cur_time.AddSeconds(cur_store.Lock_Time));
                    d_StoreChanged[id] = m_userstore.D_Store[id];
                }
            }
            

            //상품목록 생성 및 랜덤박스 오픈
            var RandomboxOpenData = await LocalUser_Randombox.Open_Randombox(cur_store.L_RewardInfo.ToArray().ItemMulCount(count));

            //가격 지불 및 획득
            var InvenChangeData = await LocalUser_Inven.Add_Remove_byKind(RandomboxOpenData.reward_info, price_info);

            UserManager.Save_LocalUD(User_Store.Path, m_userstore);
            Debug.Log("구매완료");
            return (d_StoreChanged, InvenChangeData.D_InvenChanged, RandomboxOpenData.D_RandomboxChanged, RandomboxOpenData.reward_info, InvenChangeData.Replacement_Info);
        }
        else
        {
            throw new Exception("So Expensive! ID : " + id + " Count : " + count);
        }
    }

    /// <summary>
    /// 구매 후 함수실행
    /// </summary>
    /// <param name="price_info"></param>
    /// <returns></returns>
    public static async Task<Dictionary<int, User_Inven.Inven>> Buy((int kind, int id, int count) price_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (await LocalUser_Inven.CheckItemCount(price_info.ToArray()))
        {
            //가격지불
            var InvenChangeData = await LocalUser_Inven.Add_Remove_byKind(null, price_info.ToArray());
            return InvenChangeData.D_InvenChanged;
        }
        else
        {
            throw new Exception("So Expensive! Kind : " + price_info.kind + " ID : " + price_info.id + " Count : " + price_info.count);
        }
    }
}
public static class Ex_Store 
{
    public static bool isLimit(this User_Store m_userstore, int id, int limit_count)
    {
        if (m_userstore.D_Store.ContainsKey(id))
        {
            if(m_userstore.D_Store[id].Count < limit_count)
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
    public static void AddLimitCount(this User_Store m_userstore, int id)
    {
        if (m_userstore.D_Store.ContainsKey(id))
        {
            m_userstore.D_Store[id].Count++;
        }
        else
        {
            User_Store.Store user_store = new User_Store.Store();
            user_store.Count = 1;
            m_userstore.D_Store.Add(id, user_store);
        }
    }
    public static bool isLock(this User_Store m_userstore, int id, DateTime curtime)
    {
        if (m_userstore.D_Store.ContainsKey(id))
        {
            if (DateTime.Parse(m_userstore.D_Store[id].Time).CompareTo(curtime) > 0) // a.compareto(b) => 0보다작으면 a보다 b가이전, 0은 a==b, 0보다 크면 a보다 b가 후
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
        if (m_userstore.D_Store.ContainsKey(id))
        {
            m_userstore.D_Store[id].Time = datetime.ToString();
        }
        else
        {
            User_Store.Store user_store = new User_Store.Store();
            user_store.Time = datetime.ToString();
            m_userstore.D_Store.Add(id, user_store);
        }
    }
}
#endregion
#region Streaming Store
[Serializable]
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
    public int[] Limit;
    public int[] Price_Kind;
    public int[] Price_ID;
    public int[] Price_Count;
    public int[] Reward_Kind;
    public int[] Reward_ID;
    public int[] Reward_Count;
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
    public int Limit;
    public List<(int kind, int id, int count)> L_PriceInfo = new List<(int kind, int id, int count)>();
    public List<(int kind, int id, int count)> L_RewardInfo = new List<(int kind, int id, int count)>();

    public static void Data_DicSet(J_StoreData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            if (D_Data.ContainsKey(j_obj.ID[i]) == false)
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
                    Limit = j_obj.Limit[i],
                };
                D_Data.Add(obj.ID, obj);
            }

            StoreData cur_store = D_Data[j_obj.ID[i]];
            cur_store.L_PriceInfo.AddItemInfo(j_obj.Price_Kind[i], j_obj.Price_ID[i], j_obj.Price_Count[i]);
            cur_store.L_RewardInfo.AddItemInfo(j_obj.Reward_Kind[i], j_obj.Reward_ID[i], j_obj.Reward_Count[i]);
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
}
#endregion