using System.Collections.Generic;
using System;
using UnityEngine;
public class StoreManager : Manager<StoreManager>
{
    private void Awake()
    {
        StreamingManager.Read_Data<J_StoreData>(StreamingManager.Get_StreamingPath(J_StoreData.Path), StoreData.Data_DicSet);
    }

    public void Buy((int, int, int) price_info, Action ac_buy)
    {
        if (InvenManager.Instance.AvailablePurchase(price_info))
        {
            InvenManager.Instance.Remove_bykind(price_info);
            ac_buy();
        }
    }
    public void Buy(int id, int count = 1)
    {
        if(count == 0)
        {
            throw new Exception("Buy Count is 0. ID : " + id);
        }
        StoreData cur_store = StoreData.Get(id);

        var price_info = cur_store.Get_PriceInfo(count);
        List<(int kind, int id, int count)> reward_info = null;

        if (InvenManager.Instance.AvailablePurchase(price_info))
        {
            reward_info = cur_store.Get_RewardInfo_List(count);
            InvenManager.Instance.Remove_bykind(price_info);

            for(int i = 0; i < reward_info.Count; i++)
            {
                switch (reward_info[i].kind)
                {
                    case InvenManager.RandomboxKind:
                        reward_info.Add(RandomBoxData.Get(reward_info[i].id).Gacha());
                        reward_info.RemoveAt(i);
                        i--;
                        break;
                }
            }
            InvenManager.Instance.Add(reward_info);
            Debug.Log("구매 완료");
        }
        else
        {
            Debug.Log("재화 부족");
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
public class J_StoreData
{
    public const string Path = "Store";

    public int[] ID;
    public int[] Type;
    public int[] Title;
    public int[] Description;
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
