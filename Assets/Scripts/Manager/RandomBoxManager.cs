using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoxManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class J_RandomBoxData
{
    //MaxCount를 가득 채우면 MaxReward 아이템 획득
    //MaxReward 아이템 획득시 카운트 초기화

    public const string Path = "RandomBox";

    public int[] ID;
    public int[] Range_Kind;
    public int[] Range_Min;
    public int[] Range_Max;
    public int[] Kind_0, Kind_1, Kind_2, Kind_3, Kind_4, Kind_5, Kind_6, Kind_7, Kind_8, Kind_9;
    public int[] ID_0, ID_1, ID_2, ID_3, ID_4, ID_5, ID_6, ID_7, ID_8, ID_9;
    public int[] Count_0, Count_1, Count_2, Count_3, Count_4, Count_5, Count_6, Count_7, Count_8, Count_9;
    public float[] Per_0, Per_1, Per_2, Per_3, Per_4, Per_5, Per_6, Per_7, Per_8, Per_9;
    public int[] Max_Count;
    public int[] Max_Reward_Kind_0;
    public int[] Max_Reward_ID_0;
    public int[] Max_Reward_Count_0;

}
public class RandomBoxData
{
    static Dictionary<int, RandomBoxData> D_Data = new Dictionary<int, RandomBoxData>();

    public int ID;

    public (int kind, int id, int count, int per)[] Random_Info;
    public int Max_Count;
    public (int kind, int id, int count) Max_Reward_Info;

    public static void Data_DicSet(J_RandomBoxData j_obj)
    {
        for (int i = 0; i < j_obj.ID.Length; i++)
        {
            RandomBoxData obj = new RandomBoxData()
            {
                ID = j_obj.ID[i],
                Max_Count = j_obj.Max_Count[i],
            };


            
            D_Data.Add(obj.ID, obj);

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

    static (int kind, int id, int count, float per)[] ToRandomInfo(int range_kind, int range_max, int range_min, int[] kind, int[] id, int[] count, float[] per)
    {
        //NonePer 갯수 n
        // n == 0 일때 확률은 비율로 계산
        // n != 0 일때 확률은 확률값있는것들 다 빼고 남은값을 n목록들이 n등분해서 계산

        var random_info = new List<(int kind, int id, int count, float per)>();
        List<(int kind, int id, int count)> l_none_per = new List<(int, int, int)>(); //확률 없는 상품
        List<(int kind, int id, int count, float per)> l_per = new List<(int, int, int, float)>(); //확률 있는 상품
        if(range_kind != 0 && range_max != 0 && range_min != 0)
        {
            ItemData cur_item;
            for(int i = range_min; i <= range_max; i++)
            {
                if (ItemData.Get((range_kind, i), out cur_item))
                {
                    if (cur_item.GachaLock == false)
                    {
                        l_none_per.Add((range_kind, i, 1));
                    }
                }
            }
        }

        for(int i = 0; i < kind.Length; i++)
        {
            if(count[i] != 0)
            {
                if(per[i] > 0)
                {
                    l_per.Add((kind[i], id[i], count[i], per[i]));
                }
                else
                {
                    l_none_per.Add((kind[i], id[i], count[i]));
                }
            }
        }

        float per_total = 0;

        for(int i = 0; i < l_per.Count; i++)
        {
            per_total += l_per[i].per;
            random_info.Add(l_per[i]);
        }
        for (int i = 0; i < l_per.Count; i++)
        {
            random_info.Add(l_per[i]);
        }
        if (l_none_per.Count == 0)
        {
            var arr_per = new float[l_per.Count];
            for(int i = 0; i < arr_per.Length; i++)
            {
                arr_per[i] = l_per[i].per;
            }
        }
        else
        {
            //100 - 정해진확률값 후 남은값을 n목록들이 n등분해서 계산

        }
    }
}