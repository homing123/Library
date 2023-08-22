using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;
public class RandomBoxManager : Manager<RandomBoxManager>
{
    private void Awake()
    {
        User_Randombox.m_UserRandombox = new User_Randombox();
        StreamingManager.LT_StrLoad.Add_Task(new Task(() =>
        {
            StreamingManager.Read_Data<J_RandomBoxData>(StreamingManager.Get_StreamingPath(J_RandomBoxData.Path), RandomBoxData.Data_DicSet);
        }));
    }


    public void Gacha_byInven(int inven_key, int count)
    {
        if(count == 0)
        {
            throw new Exception("Gacha_byInven Coutn is 0. inven_key : " + inven_key);
        }

        User_Randombox.m_UserRandombox.Gacha_byInven(inven_key, count);
    }

    [SerializeField] int Test_RandomBoxID;
    [ContextMenu("확률 로그")]
    public void Test_Log()
    {
        RandomBoxData cur_box = RandomBoxData.Get(Test_RandomBoxID);
        for(int i = 0; i < cur_box.Random_Info.Length; i++)
        {
            Debug.Log("RandomBox Info. Kind : " + cur_box.Random_Info[i].kind + " ID : " + cur_box.Random_Info[i].id + " Count : " + cur_box.Random_Info[i].count + " Per : " + cur_box.Random_Info[i].per);
        }
    }
}
#region UD_Randombox
public class User_Randombox : UserData_Server
{
    public const string Path = "Randombox";
    public static User_Randombox m_UserRandombox;
    public static Action ac_RandomboxChanged;

    public Dictionary<int, int> D_Randombox = new Dictionary<int, int>();

    public override async Task Load()
    {
        await Task.Delay(1);
        if (UserManager.Use_Local)
        {
            if (File.Exists(Path))
            {
                var data = await UserManager.Load_LocalUDAsync<User_Randombox>(Path);
                D_Randombox = data.D_Randombox;
            }
            else
            {
                UserManager.Save_LocalUD(Path, this);
            }
        }
        else
        {
            //서버에서 있는지없는지 확인 후 없으면 생성해서 보내면 그거 받으면됨
        }
    }
    public async void Gacha_byInven(int inven_key, int count)
    {
        if (UserManager.Use_Local)
        {
            var data = await LocalUser_Randombox.Gacha_byInven(inven_key, count);
            D_Randombox = data.d_randombox;
            User_Inven.m_UserInven.D_Inven = data.d_inven;
            ac_RandomboxChanged?.Invoke();
            User_Inven.ac_InvenChanged?.Invoke();
        }
        else
        {
            //server
        }
    }
    public int Get_Count(int id)
    {
        if (D_Randombox.ContainsKey(id))
        {
            return D_Randombox[id];
        }
        else
        {
            return 0;
        }
    }
}
public class LocalUser_Randombox
{
    /// <summary>
    /// 상점에서 뽑기시 뽑기결과만 전달
    /// </summary>
    /// <param name="id"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static async Task<((int kind, int id, int count)[] item_info, Dictionary<int,int> d_randombox)> Gacha_byBuy(int id, int count)
    {
        await Task.Delay(1);
        User_Randombox m_userrandombox = UserManager.Load_LocalUD<User_Randombox>(User_Randombox.Path);

        List<(int kind, int id, int count)> l_iteminfo = new List<(int kind, int id, int count)>();

        for(int i = 0; i < count; i++)
        {
            m_userrandombox.Add(id);
            l_iteminfo.Add(m_userrandombox.Get_RandomboxResult(id));
        }

        int test = 0;
        for(int i = 0; i < l_iteminfo.Count; i++)
        {
            if (l_iteminfo[i].kind == ItemData.RandomboxKind)
            {
                for (int j = 0; j < l_iteminfo[i].count; i++) 
                {
                    test++;
                    if(test > 1000)
                    {
                        throw new Exception("Randombox Gacha has Infinity Loop ID : " + id + " Count : " + count);
                    }
                    l_iteminfo.Add(m_userrandombox.Get_RandomboxResult(l_iteminfo[i].id));
                }
                l_iteminfo.RemoveAt(i);
                i--;
            }
        }

        UserManager.Save_LocalUD(User_Randombox.Path, m_userrandombox);
        return (l_iteminfo.ToArray(), m_userrandombox.D_Randombox);
    }

    /// <summary>
    /// 인벤에서 랜덤상자 사용시 상자 제거 후 상자결과 획득
    /// </summary>
    /// <returns></returns>
    public static async Task<(Dictionary<int ,User_Inven.Inven> d_inven, Dictionary<int,int> d_randombox)> Gacha_byInven(int inven_key, int count)
    {
        await Task.Delay(1);

        User_Randombox m_userrandombox = UserManager.Load_LocalUD<User_Randombox>(User_Randombox.Path);
        User_Inven m_userinven = UserManager.Load_LocalUD<User_Inven>(User_Inven.Path);

        List<(int kind, int id, int count)> l_iteminfo = new List<(int kind, int id, int count)>();

        if(m_userinven.D_Inven.ContainsKey(inven_key)== false)
        {
            throw new Exception("Gacha_byInven Inven null : Inven_Key : " + inven_key);
        }
        else if (m_userinven.D_Inven[inven_key].Count < count || m_userinven.D_Inven[inven_key].Kind != ItemData.RandomboxKind)
        {
            User_Inven.Inven inven = m_userinven.D_Inven[inven_key];
            throw new Exception("Gacha_byInven Inven Error : Inven_Key : " + inven_key + " Count : " + count + " ( Inven_kind : " + inven.Kind + " Inven_id : " + inven.Id + " Inven_Count : " + inven.Count + " )");
        }

        int box_id = m_userinven.D_Inven[inven_key].Id;
        await LocalUser_Inven.Remove_byKey((inven_key, count).ToArray());

        for (int i = 0; i < count; i++)
        {
            m_userrandombox.Add(box_id);
            l_iteminfo.Add(m_userrandombox.Get_RandomboxResult(box_id));
        }

        int test = 0;
        for (int i = 0; i < l_iteminfo.Count; i++)
        {
            if (l_iteminfo[i].kind == ItemData.RandomboxKind)
            {
                for (int j = 0; j < l_iteminfo[i].count; i++)
                {
                    test++;
                    if (test > 1000)
                    {
                        throw new Exception("Randombox Gacha has Infinity Loop ID : " + box_id + " Count : " + count);
                    }
                    l_iteminfo.Add(m_userrandombox.Get_RandomboxResult(l_iteminfo[i].id));
                }
                l_iteminfo.RemoveAt(i);
                i--;
            }
        }

        Dictionary<int, User_Inven.Inven> d_inven = await LocalUser_Inven.Add(l_iteminfo.ToArray());

        return (d_inven, m_userrandombox.D_Randombox);
    }
}
public static class Ex_Randombox
{
    public static (int kind, int id, int count)[] Get_RandomboxResult(this User_Randombox user_Randombox, int id)
    {
        RandomBoxData cur_box = RandomBoxData.Get(id);
        List<(int kind, int id, int count)> l_iteminfo = new List<(int kind, int id, int count)>();
        if (user_Randombox.isFirst(id))
        {
            l_iteminfo.Add(cur_box.First_Reward_Info);
        }
        else if (user_Randombox.isMax(id, cur_box.Max_Count))
        {
            l_iteminfo.Add(cur_box.Max_Reward_Info);
        }
        else
        {
            l_iteminfo.Add(cur_box.Gacha());
        }
        return l_iteminfo.ToArray();
    }
    public static bool isFirst(this User_Randombox m_userrandombox, int id)
    {
        if (m_userrandombox.D_Randombox.ContainsKey(id) == false || m_userrandombox.D_Randombox[id] == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool isMax(this User_Randombox m_userrandombox, int id, int max_count)
    {
        if (m_userrandombox.D_Randombox.ContainsKey(id) == true && m_userrandombox.D_Randombox[id] % max_count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void Add(this User_Randombox m_userrandombox, int id)
    {
        if (m_userrandombox.D_Randombox.ContainsKey(id))
        {
            m_userrandombox.D_Randombox[id]++;
        }
        else
        {
            m_userrandombox.D_Randombox.Add(id, 1);
        }
    }
}
#endregion
#region Streaming Randombox
public class J_RandomBoxData
{
    //MaxCount를 가득 채우면 MaxReward 아이템 획득
    //MaxReward 아이템 획득시 카운트 초기화

    public const string Path = "RandomBox";

    public int[] ID;
    public int[] Range_Kind;
    public int[] Range_Min;
    public int[] Range_Max;
    public int[] Item_Kind_0, Item_Kind_1, Item_Kind_2, Item_Kind_3, Item_Kind_4, Item_Kind_5, Item_Kind_6, Item_Kind_7, Item_Kind_8, Item_Kind_9;
    public int[] Item_ID_0, Item_ID_1, Item_ID_2, Item_ID_3, Item_ID_4, Item_ID_5, Item_ID_6, Item_ID_7, Item_ID_8, Item_ID_9;
    public int[] Item_Count_0, Item_Count_1, Item_Count_2, Item_Count_3, Item_Count_4, Item_Count_5, Item_Count_6, Item_Count_7, Item_Count_8, Item_Count_9;
    public float[] Item_Per_0, Item_Per_1, Item_Per_2, Item_Per_3, Item_Per_4, Item_Per_5, Item_Per_6, Item_Per_7, Item_Per_8, Item_Per_9;
    public int[] Max_Count;
    public int[] Max_Reward_Kind_0;
    public int[] Max_Reward_ID_0;
    public int[] Max_Reward_Count_0;
    public int[] First_Reward_Kind_0;
    public int[] First_Reward_ID_0;
    public int[] First_Reward_Count_0;

}
public class RandomBoxData
{ 
    static int loop_count = 0;
    static int loop_ID;
    static Dictionary<int, RandomBoxData> D_Data = new Dictionary<int, RandomBoxData>();

    public int ID;

    public int Range_Kind;
    public int Range_Min;
    public int Range_Max;
    public int[] Item_Kind;
    public int[] Item_ID;
    public int[] Item_Count;
    public float[] Item_Per;

    public (int kind, int id, int count, float per)[] Random_Info;
    public int Max_Count;
    public (int kind, int id, int count)[] Max_Reward_Info;
    public (int kind, int id, int count)[] First_Reward_Info;

    public static void Data_DicSet(J_RandomBoxData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            RandomBoxData obj = new RandomBoxData()
            {
                ID = j_obj.ID[i],
                Range_Kind = j_obj.Range_Kind[i],
                Range_Min = j_obj.Range_Min[i],
                Range_Max = j_obj.Range_Max[i],
                Item_Kind = new int[10] { j_obj.Item_Kind_0[i], j_obj.Item_Kind_1[i], j_obj.Item_Kind_2[i], j_obj.Item_Kind_3[i], j_obj.Item_Kind_4[i], j_obj.Item_Kind_5[i], j_obj.Item_Kind_6[i], j_obj.Item_Kind_7[i], j_obj.Item_Kind_8[i], j_obj.Item_Kind_9[i], },
                Item_ID = new int[10] { j_obj.Item_ID_0[i], j_obj.Item_ID_1[i], j_obj.Item_ID_2[i], j_obj.Item_ID_3[i], j_obj.Item_ID_4[i], j_obj.Item_ID_5[i], j_obj.Item_ID_6[i], j_obj.Item_ID_7[i], j_obj.Item_ID_8[i], j_obj.Item_ID_9[i], },
                Item_Count = new int[10] { j_obj.Item_Count_0[i], j_obj.Item_Count_1[i], j_obj.Item_Count_2[i], j_obj.Item_Count_3[i], j_obj.Item_Count_4[i], j_obj.Item_Count_5[i], j_obj.Item_Count_6[i], j_obj.Item_Count_7[i], j_obj.Item_Count_8[i], j_obj.Item_Count_9[i], },
                Item_Per = new float[10] { j_obj.Item_Per_0[i], j_obj.Item_Per_1[i], j_obj.Item_Per_2[i], j_obj.Item_Per_3[i], j_obj.Item_Per_4[i], j_obj.Item_Per_5[i], j_obj.Item_Per_6[i], j_obj.Item_Per_7[i], j_obj.Item_Per_8[i], j_obj.Item_Per_9[i], },

                Max_Count = j_obj.Max_Count[i],
            };

            obj.Max_Reward_Info = InvenManager.ToItemInfo(new int[] { j_obj.Max_Reward_Kind_0[i] }, new int[] { j_obj.Max_Reward_ID_0[i] }, new int[] { j_obj.Max_Reward_Count_0[i] });
            obj.First_Reward_Info = InvenManager.ToItemInfo(new int[] { j_obj.First_Reward_Kind_0[i] }, new int[] { j_obj.First_Reward_ID_0[i] }, new int[] { j_obj.First_Reward_Count_0[i] });
            D_Data.Add(obj.ID, obj);
        }

        foreach (RandomBoxData cur_box in D_Data.Values)
        {
            if(cur_box.Random_Info == null)
            {
                loop_count = 0;
                loop_ID = cur_box.ID;
                Set_RandomInfo(cur_box);
            }
        }
    }

    public static RandomBoxData Get(int key)
    {
        if (D_Data.ContainsKey(key))
        {
            return D_Data[key];
        }
        else
        {
            if (D_Data.Count == 0)
            {
                throw new Exception("RandomBoxData count is 0");
            }
            else
            {
                throw new Exception("RandomBoxData key is null : " + key);
            }
        }
    }

    static void Set_RandomInfo(RandomBoxData cur_box)
    {
        loop_count++;
        if (loop_count > 100)
        {
            throw new Exception("Infinity Loop ID : " + loop_ID);
        }
        //NonePer 갯수 n
        // n == 0 일때 확률은 비율로 계산
        // n != 0 일때 확률은 확률값있는것들 다 빼고 남은값을 n목록들이 n등분해서 계산

        var random_info = new List<(int kind, int id, int count, float per)>();
        List<(int kind, int id, int count)> l_none_per = new List<(int, int, int)>(); //확률 없는 상품
        List<(int kind, int id, int count, float per)> l_per = new List<(int, int, int, float)>(); //확률 있는 상품
        if(cur_box.Range_Kind != 0 && cur_box.Range_Max != 0 && cur_box.Range_Min != 0)
        {
            ItemData cur_item;
            for(int i = cur_box.Range_Min; i <= cur_box.Range_Max; i++)
            {
                if (ItemData.Get((cur_box.Range_Kind, i), out cur_item))
                {
                    if (cur_item.GachaLock == false)
                    {
                        l_none_per.Add((cur_box.Range_Kind, i, 1));
                    }
                }
            }
        }

        for(int i = 0; i < cur_box.Item_Kind.Length; i++)
        {
            if(cur_box.Item_Count[i] != 0)
            {
                if(cur_box.Item_Per[i] > 0)
                {
                    l_per.Add((cur_box.Item_Kind[i], cur_box.Item_ID[i], cur_box.Item_Count[i], cur_box.Item_Per[i]));
                }
                else
                {
                    l_none_per.Add((cur_box.Item_Kind[i], cur_box.Item_ID[i], cur_box.Item_Count[i]));
                }
            }
        }

        float per_total = 0;

        for(int i = 0; i < l_per.Count; i++)
        {
            per_total += l_per[i].per;
            random_info.Add(l_per[i]);
        }

        if (l_none_per.Count > 0)
        {
            if (per_total < 100)
            {
                float none_per = (100 - per_total) / l_none_per.Count;
                for(int i = 0; i < l_none_per.Count; i ++)
                {
                    random_info.Add((l_none_per[i].kind, l_none_per[i].id, l_none_per[i].count, none_per));
                }
            }
            else
            {
                throw new Exception("확률 미지정 아이템이 있는데 확률값이 100이 넘습니다");
            }
        }
        
        //랜덤박스들 풀어서 넘겨줘야함
        for(int i = 0; i < random_info.Count; i++)
        {
            if(random_info[i].kind == ItemData.RandomboxKind)
            {
                float cur_per = random_info[i].per;
                RandomBoxData info_box = RandomBoxData.Get(random_info[i].id);
                random_info.RemoveAt(i);
                i--;
                if(info_box.Random_Info == null)
                {
                    Set_RandomInfo(info_box);
                }

                float info_box_totalper = 0;
                for (int j = 0; j < info_box.Random_Info.Length; j++)
                {
                    info_box_totalper += info_box.Random_Info[j].per;
                }

                for (int j=0;j< info_box.Random_Info.Length; j++)
                {
                    random_info.Add((info_box.Random_Info[j].kind, info_box.Random_Info[j].id, info_box.Random_Info[j].count, info_box.Random_Info[j].per * (cur_per / info_box_totalper)));
                }
            }
            if(random_info.Count > 10000)
            {
                throw new Exception("samebox infinity loop ID : " + loop_ID);
            }
        }
        cur_box.Random_Info = random_info.ToArray();
        Debug.Log("랜덤인포 세팅 : " + cur_box.ID+" "+ random_info.Count);

    }
    public (int kind, int id, int count) Gacha()
    {
        float[] arr_per = new float[Random_Info.Length];
        for(int i = 0; i < arr_per.Length; i++)
        {
            arr_per[i] = Random_Info[i].per;
        }
        var result_info = Random_Info[Math_Define.Get_RandomResult(arr_per)];
        return (result_info.kind, result_info.id, result_info.count);
    }
}
#endregion