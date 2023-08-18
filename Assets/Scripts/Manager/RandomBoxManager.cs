using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomBoxManager : Manager<RandomBoxManager>
{
    private void Awake()
    {
        StreamingManager.Read_Data<J_RandomBoxData>(StreamingManager.Get_StreamingPath(J_RandomBoxData.Path), RandomBoxData.Data_DicSet);
    }


    public (int kind, int id, int count)[] Gacha(int id, int count)
    {
        if(count == 0)
        {
            throw new Exception("Gacha Coutn is 0. ID : " + id);
        }
        RandomBoxData cur_box = RandomBoxData.Get(id);
        List<(int, int, int)> l_result = new List<(int, int, int)>();
        for(int i = 0; i < count; i++)
        {
            l_result.Add(cur_box.Gacha());
        }
        return l_result.ToArray();
    }

    [SerializeField] int Test_RandomBoxID;
    [SerializeField] int Test_Count;
    [ContextMenu("�̱�")]
    public void Test_Gacha()
    {
        var gacha_info = Gacha(Test_RandomBoxID, Test_Count);
        for(int i = 0; i < gacha_info.Length; i++)
        {
            Debug.Log("Gacha Result. Kind : " + gacha_info[i].kind + " ID : " + gacha_info[i].id + " Count : " + gacha_info[i].count);
        }
    }
    [ContextMenu("Ȯ�� �α�")]
    public void Test_Log()
    {
        RandomBoxData cur_box = RandomBoxData.Get(Test_RandomBoxID);
        for(int i = 0; i < cur_box.Random_Info.Length; i++)
        {
            Debug.Log("RandomBox Info. Kind : " + cur_box.Random_Info[i].kind + " ID : " + cur_box.Random_Info[i].id + " Count : " + cur_box.Random_Info[i].count + " Per : " + cur_box.Random_Info[i].per);
        }
    }
}
public class J_RandomBoxData
{
    //MaxCount�� ���� ä��� MaxReward ������ ȹ��
    //MaxReward ������ ȹ��� ī��Ʈ �ʱ�ȭ

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
        //NonePer ���� n
        // n == 0 �϶� Ȯ���� ������ ���
        // n != 0 �϶� Ȯ���� Ȯ�����ִ°͵� �� ���� �������� n��ϵ��� n����ؼ� ���

        var random_info = new List<(int kind, int id, int count, float per)>();
        List<(int kind, int id, int count)> l_none_per = new List<(int, int, int)>(); //Ȯ�� ���� ��ǰ
        List<(int kind, int id, int count, float per)> l_per = new List<(int, int, int, float)>(); //Ȯ�� �ִ� ��ǰ
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
                throw new Exception("Ȯ�� ������ �������� �ִµ� Ȯ������ 100�� �ѽ��ϴ�");
            }
        }
        
        //�����ڽ��� Ǯ� �Ѱ������
        for(int i = 0; i < random_info.Count; i++)
        {
            if(random_info[i].kind == InvenManager.RandomboxKind)
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
        Debug.Log("�������� ���� : " + cur_box.ID+" "+ random_info.Count);

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