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
    
    public async void Buy(int id, int count = 1)
    {
        if (count == 0)
        {
            throw new Exception("Buy Count is 0. ID : " + id);
        }
        StoreData cur_store = StoreData.Get(id);
        if (cur_store.Purchase.IsNullOrEmptyOrN())
        {
            //�����Ϸ� (User_Store.m_UserStore.Buy(id, count));
        }
        else if (cur_store.Advertisement)
        {
            await AdManager.Instance.ShowAsync(AdManager.E_Adkind.Reward_Video);
        }
      
        var buy_result = await User_Store.m_UserStore.Buy(id, count);


        Debug.Log("���� ���� : ");
        for (int i = 0; i < buy_result.Reward_Info.Length; i++)
        {
            Debug.Log(" kind : " + buy_result.Reward_Info[i].kind + " id : " + buy_result.Reward_Info[i].id + " count : " + buy_result.Reward_Info[i].count);
        }

        Debug.Log("��ü�� ���� : ");
        for (int i = 0; i < buy_result.Replacement_Info.Length; i++)
        {
            Debug.Log(" kind : " + buy_result.Replacement_Info[i].kind + " id : " + buy_result.Replacement_Info[i].id + " count : " + buy_result.Replacement_Info[i].count);
        }
        //����
    }

    #region Test
    [SerializeField] int Test_StoreID;
    [SerializeField] int Test_Count;
    [ContextMenu("����")]
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

    public List<Record> L_Purchase_Record = new List<Record>(); //������ǰ ���ű��
    public Dictionary<int, StoreLock> D_StoreLock = new Dictionary<int, StoreLock>(); //���� ���ѽð�
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
                var data = UserManager.Load_LocalUD<User_Store>(Path);
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
            //�������� �ִ��������� Ȯ�� �� ������ �����ؼ� ������ �װ� �������
        }
    }

    /// <summary>
    /// �����Լ�
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public async Task<((int kind, int id, int count)[] Reward_Info, (int kind, int id, int count)[] Replacement_Info)> Buy(int id, int count = 1)
    {
        //���Ž� �κ�������, �����ڽ�������, �������� ����
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Store.Buy(id, count);
            D_StoreLock = data.D_StoreLock;
            L_Purchase_Record = data.L_Purchase_Record;
            User_Inven.m_UserInven.D_Inven = data.D_Inven;
            ac_StoreChanged?.Invoke();
            User_Inven.ac_InvenChanged?.Invoke();
            User_Randombox.m_UserRandombox.D_Randombox = data.D_Randombox;
            User_Randombox.ac_RandomboxChanged?.Invoke();
            return (data.Reward_Info, data.Replacement_Info);
        }
        else
        {
            //server
        }
    }

    /// <summary>
    /// ���� ���� �� �Լ�����
    /// </summary>
    /// <param name="price_info"></param>
    /// <param name="ac_buy"></param>
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
    /// <summary>
    /// �����Լ�
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static async Task<(Dictionary<int, User_Store.StoreLock> D_StoreLock, List<User_Store.Record> L_Purchase_Record, Dictionary<int,User_Inven.Inven> D_Inven, Dictionary<int,int> D_Randombox, (int kind, int id, int count)[] Reward_Info, (int kind, int id, int count)[] Replacement_Info)> Buy(int id, int count = 1)
    {
        Debug.Log("����");
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Store m_userstore = UserManager.Load_LocalUD<User_Store>(User_Store.Path);
        StoreData cur_store = StoreData.Get(id);

        //�������� ��������
        var price_info = cur_store.L_PriceInfo.ToArray().ItemMulCount(count);

        if (await LocalUser_Inven.CheckItemCount(price_info)) //���� ���� �������� Ȯ��
        {
            DateTime cur_time = await TimeManager.Instance.Cur_TimeAsync(); //����ð� ��������

            //���� �ð����� Ȯ��
            if (cur_store.Lock_Time != 0)
            {
                if (m_userstore.isLock(id, cur_time))
                {
                    throw new Exception("This Product has TimeLock ID : " + id + " TimeLock : " + m_userstore.D_StoreLock[id].Time + " Cur Time : " + cur_time.ToString());
                }
                else
                {
                    m_userstore.AddLockTime(id, cur_time.AddSeconds(cur_store.Lock_Time));
                }
            }

            //��ǰ��� ���� �� �����ڽ� ����
            var data = await LocalUser_Randombox.Open_Randombox(cur_store.L_RewardInfo.ToArray().ItemMulCount(count));

            //���� ���� �� ȹ��
            var InvenChangeData = await LocalUser_Inven.Add_Remove_byKind(data.reward_info, price_info);

            //������ǰ���� Ȯ��
            if (cur_store.Purchase.IsNullOrEmptyOrN())
            {
                m_userstore.L_Purchase_Record.Add(new User_Store.Record()
                {
                    Time = cur_time.ToString(),
                    ID = id
                });
            } 

            UserManager.Save_LocalUD(User_Store.Path, m_userstore);
            Debug.Log("���ſϷ�");
            return (m_userstore.D_StoreLock, m_userstore.L_Purchase_Record, InvenChangeData.D_Inven, data.d_randombox, data.reward_info, InvenChangeData.Replacement_Info);
        }
        else
        {
            throw new Exception("So Expensive! ID : " + id + " Count : " + count);
        }
    }

    /// <summary>
    /// ���� �� �Լ�����
    /// </summary>
    /// <param name="price_info"></param>
    /// <returns></returns>
    public static async Task<Dictionary<int, User_Inven.Inven>> Buy((int kind, int id, int count) price_info)
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (await LocalUser_Inven.CheckItemCount(price_info.ToArray()))
        {
            //��������
            var InvenChangeData = await LocalUser_Inven.Add_Remove_byKind(null, price_info.ToArray());
            return InvenChangeData.D_Inven;
        }
        else
        {
            throw new Exception("So Expensive! Kind : " + price_info.kind + " ID : " + price_info.id + " Count : " + price_info.count);
        }
    }
}
public static class Ex_Store 
{
    public static bool isLock(this User_Store m_userstore, int id, DateTime curtime)
    {
        if (m_userstore.D_StoreLock.ContainsKey(id))
        {
            if (DateTime.Parse(m_userstore.D_StoreLock[id].Time).CompareTo(curtime) > 0) // a.compareto(b) => 0���������� a���� b������, 0�� a==b, 0���� ũ�� a���� b�� ��
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