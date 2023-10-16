using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.IO;
using Define;
public class RandomBoxManager : Manager<RandomBoxManager>
{
    private void Awake()
    {
        User_Randombox.m_UserRandombox = new User_Randombox();
        StreamingManager.lt_StrLoad.Add(async () =>
        {
            var data = await StreamingManager.ReadDataAsync<J_RandomBoxData>(StreamingManager.Get_StreamingPath(J_RandomBoxData.Path));
            RandomBoxData.Data_DicSet(data);
        });
    }

    [SerializeField] int Test_RandomBoxID;
    [ContextMenu("Ȯ�� �α�")]
    public void Test_Log()
    {
        RandomBoxData cur_box = RandomBoxData.Get(Test_RandomBoxID);
        for (int i = 0; i < cur_box.RandomInfo.Length; i++)
        {
            Debug.Log("RandomBox Info. Kind : " + cur_box.RandomInfo[i].kind + " ID : " + cur_box.RandomInfo[i].id + " Count : " + cur_box.RandomInfo[i].count + " Per : " + cur_box.RandomInfo[i].per);
        }
    }
}
#region UD_Randombox
public class User_Randombox : UserData_Server
{
    public const string Path = "Randombox";
    public static User_Randombox m_UserRandombox;
    public static Action ac_RandomboxChanged = () => { Debug.Log("RandomboxChanged Action"); };

    public Dictionary<int, int> D_Randombox = new Dictionary<int, int>(); //���ں� ����Ƚ�� ������

    public override async Task Load()
    {
        await Task.Delay(GameManager.Instance.TaskDelay);
        if (UserManager.Use_Local)
        {
            if (UserManager.Exist_LocalUD(Path))
            {
                var data = UserManager.Load_LocalUD<User_Randombox>(Path);
                D_Randombox = data.D_Randombox;
            }
            else
            {
                Debug.Log("Randombox_Init");

                UserManager.Save_LocalUD(Path, this);
            }
        }
        else
        {
            //�������� �ִ��������� Ȯ�� �� ������ �����ؼ� ������ �װ� �������
        }
    }
}
public class LocalUser_Randombox
{
    /// <summary>
    /// ���� �����۸���Ʈ�߿� �����ڽ��� ������ �����ڽ��� ������ ����� ���縮��Ʈ�� �߰� �� ����
    /// </summary>
    /// <param name="item_info"></param>
    /// <returns></returns>
    public static async Task<((int kind, int id, int count)[] reward_info, Dictionary<int, int> D_RandomboxChanged)> Open_Randombox((int kind, int id, int count)[] item_info)
    {
        //ex �Ű����� (���, �ڽ�, ���)
        //ex return (���, ���, �ڽ����)
        await Task.Delay(GameManager.Instance.TaskDelay);
        User_Randombox m_userrandombox = UserManager.Load_LocalUD<User_Randombox>(User_Randombox.Path);
        Dictionary<int, int> d_RandomboxChanged = new Dictionary<int, int>();

        List<(int kind, int id, int count)> l_reward_info = new List<(int kind, int id, int count)>(); //������

        int loopCount = 0; //���ѷ��� ����
        for (int i = 0; i < item_info.Length; i++)
        {
            if (item_info[i].kind == (int)E_ItemKind.Randombox)
            {
                for (int j = 0; j < item_info[i].count; j++)
                {
                    loopCount++;
                    if (loopCount > 1000)
                    {
                        throw new Exception("Open_Randombox has Infinity Loop ID : " + item_info[i].id + " Count : " + item_info[i].count);
                    }

                    //�����ڽ� ����
                    l_reward_info.AddRange(m_userrandombox.Get_RandomboxResult(item_info[i].id));
                    d_RandomboxChanged[item_info[i].id] = m_userrandombox.D_Randombox[item_info[i].id];
                }
            }
            else
            {
                l_reward_info.Add(item_info[i]);
            }
        }
        UserManager.Save_LocalUD(User_Randombox.Path, m_userrandombox);

        return (l_reward_info.ToArray(), d_RandomboxChanged);
    }
}
public static class Ex_Randombox
{
    /// <summary>
    /// �����ڽ� ��� ��� �Լ�
    /// </summary>
    /// <param name="user_Randombox"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static (int kind, int id, int count)[] Get_RandomboxResult(this User_Randombox user_Randombox, int id)
    {
        RandomBoxData cur_box = RandomBoxData.Get(id);

        user_Randombox.Add_OpenCount(id);

        List<(int kind, int id, int count)> l_iteminfo = new List<(int kind, int id, int count)>(); //Result List

        //check first open
        if (cur_box.L_FirstRewardInfo.Empty() == false && user_Randombox.isFirst(id))
        {
            l_iteminfo.AddRange(cur_box.L_FirstRewardInfo);
        }
        else
        {
            l_iteminfo.Add(cur_box.Gacha());
        }

        //add extra reward
        if (cur_box.L_ExtraRewardInfo.Empty() == false)
        {
            l_iteminfo.AddRange(cur_box.L_ExtraRewardInfo);
        }
        return l_iteminfo.ToArray();
    }
    public static bool isFirst(this User_Randombox m_userrandombox, int id)
    {
        if ( m_userrandombox.D_Randombox[id] == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void Add_OpenCount(this User_Randombox m_userrandombox, int id)
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
[Serializable]
public class J_RandomBoxData
{
    //MaxCount�� ���� ä��� MaxReward ������ ȹ��
    //MaxReward ������ ȹ��� ī��Ʈ �ʱ�ȭ

    public const string Path = "RandomBox";

    public int[] ID;
    public int[] Type_Kind;
    public int[] Single_Kind;
    public int[] Single_ID;
    public int[] Single_Count;
    public float[] Single_Per;
    public int[] ExtraReward_Kind;
    public int[] ExtraReward_ID;
    public int[] ExtraReward_Count;
    public int[] FirstReward_Kind;
    public int[] FirstReward_ID;
    public int[] FirstReward_Count;

}
public class RandomBoxData
{
    static int loop_count = 0;
    static int loop_ID;
    static Dictionary<int, RandomBoxData> D_Data = new Dictionary<int, RandomBoxData>();

    public int ID;
    public List<int> L_TypeKind = new List<int>();//Ÿ������
    public List<(int kind, int id, int count, float per)> L_SingleInfo = new List<(int kind, int id, int count, float per)>(); //��������
    public List<(int kind, int id, int count)> L_ExtraRewardInfo = new List<(int kind, int id, int count)>(); //�߰����� ��ǰ
    public List<(int kind, int id, int count)> L_FirstRewardInfo = new List<(int kind, int id, int count)>(); //ù�̱�� ��ǰ

    public (int kind, int id, int count, float per)[] RandomInfo;//���� + ���� ���� ������ǰ��� �� Ȯ�� (���ڴ� �ش������ �����۸���� ������)

    public static void Data_DicSet(J_RandomBoxData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            if (D_Data.ContainsKey(j_obj.ID[i]) == false)
            {
                RandomBoxData obj = new RandomBoxData()
                {
                    ID = j_obj.ID[i],
                };
                D_Data.Add(obj.ID, obj);
               
            }
            RandomBoxData cur_data = D_Data[j_obj.ID[i]];

            cur_data.L_SingleInfo.AddRandomInfo(j_obj.Single_Kind[i], j_obj.Single_ID[i], j_obj.Single_Count[i], j_obj.Single_Per[i]);
            cur_data.L_ExtraRewardInfo.AddItemInfo(j_obj.ExtraReward_Kind[i], j_obj.ExtraReward_ID[i], j_obj.ExtraReward_Count[i]);
            cur_data.L_FirstRewardInfo.AddItemInfo(j_obj.FirstReward_Kind[i], j_obj.FirstReward_ID[i], j_obj.FirstReward_Count[i]);
        }

        foreach (RandomBoxData cur_box in D_Data.Values)
        {
            if (cur_box.RandomInfo == null)
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

        var random_info = new List<(int kind, int id, int count, float per)>();
        List<(int kind, int id, int count)> l_none_per = new List<(int, int, int)>(); //Ȯ�� ���� ��ǰ
        List<(int kind, int id, int count, float per)> l_per = new List<(int, int, int, float)>(); //Ȯ�� �ִ� ��ǰ

        //Ÿ�԰� Ȯ�����»�ǰ ����Ʈ�� �߰�
        for(int i = 0; i < cur_box.L_TypeKind.Count; i++)
        {
            l_none_per.AddRange(ItemData.GetItems_byKind_WithCount(cur_box.L_TypeKind[i]));
        }

        //������ Ȯ�������� ����Ʈ�� �߰�
        for (int i = 0; i < cur_box.L_SingleInfo.Count; i++)
        {
            if (cur_box.L_SingleInfo[i].per > 0)
            {
                l_per.Add((cur_box.L_SingleInfo[i]));

                //���������Ѱ� Ÿ�԰����� ����
                l_none_per.Remove((cur_box.L_SingleInfo[i].kind, cur_box.L_SingleInfo[i].id, cur_box.L_SingleInfo[i].count));
            }
            else
            {
                if (l_none_per.Contains((cur_box.L_SingleInfo[i].kind, cur_box.L_SingleInfo[i].id, cur_box.L_SingleInfo[i].count)) == false)
                {
                    l_none_per.Add((cur_box.L_SingleInfo[i].kind, cur_box.L_SingleInfo[i].id, cur_box.L_SingleInfo[i].count));
                }
            }    
        }

        //Ȯ�������� �� Ȯ�� = (100 - Ȯ�������� Ȯ�� ��) / Ȯ���������� ����
        float per_total = 0;

        //Ȯ�������� ��������Ʈ�� �߰�
        for (int i = 0; i < l_per.Count; i++)
        {
            per_total += l_per[i].per;
            random_info.Add(l_per[i]);
        }

        //Ȯ���������� ��������Ʈ�� �߰�
        if (l_none_per.Count > 0)
        {
            if (per_total < 100)
            {
                float none_per = (100 - per_total) / l_none_per.Count;
                for (int i = 0; i < l_none_per.Count; i++)
                {
                    random_info.Add((l_none_per[i].kind, l_none_per[i].id, l_none_per[i].count, none_per));
                }
            }
            else
            {
                throw new Exception("Ȯ�� ������ �������� �ִµ� Ȯ������ 100�� �ѽ��ϴ�");
            }
        }

        //���� ����Ʈ�� �����ڽ������� �ش�ڽ��� ����� ����
        for (int i = 0; i < random_info.Count; i++)
        {
            if (random_info[i].kind == (int)E_ItemKind.Randombox)
            {
                float cur_per = random_info[i].per;
                RandomBoxData info_box = RandomBoxData.Get(random_info[i].id);
                random_info.RemoveAt(i);
                i--;
                if (info_box.RandomInfo == null)
                {
                    Set_RandomInfo(info_box);
                }

                float info_box_totalper = 0;
                for (int j = 0; j < info_box.RandomInfo.Length; j++)
                {
                    info_box_totalper += info_box.RandomInfo[j].per;
                }

                for (int j = 0; j < info_box.RandomInfo.Length; j++)
                {
                    random_info.Add((info_box.RandomInfo[j].kind, info_box.RandomInfo[j].id, info_box.RandomInfo[j].count, info_box.RandomInfo[j].per * (cur_per / info_box_totalper)));
                }
            }
            if (random_info.Count > 10000)
            {
                throw new Exception("samebox infinity loop ID : " + loop_ID);
            }
        }
        cur_box.RandomInfo = random_info.ToArray();
        //Debug.Log("�������� ���� : " + cur_box.ID+" "+ random_info.Count);

    }


    /// <summary>
    /// ��������� ����� �޾ƿ��� �Լ�
    /// </summary>
    /// <returns></returns>
    public (int kind, int id, int count) Gacha()
    {
        float[] arr_per = new float[RandomInfo.Length];
        for (int i = 0; i < arr_per.Length; i++)
        {
            arr_per[i] = RandomInfo[i].per;
        }
        var result_info = RandomInfo[Math_Define.Get_RandomResult(arr_per)];
        return (result_info.kind, result_info.id, result_info.count);
    }
    
}
#endregion