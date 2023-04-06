using System;
using System.Collections.Generic;
using static Data_User;
using static UD_Func;
public class InvenManager : Single<InvenManager>
{

    //인벤 획득 및 판매는 한번에 여러개씩 하는경우가 많음 이경우를 위한 코드들 필요

    public static event EventHandler Ev_Acquire_Item;
    public static event EventHandler Ev_Sell_Item;
 
    //인벤토리도 현재 유저아이템들의 상태가 필요함 상태리스트 : None, Equip 등등추가
    //퀘스트와 마찬가지로 모든상태들을 매니저에서 갱신하고 ui에서 받아와서 쓰는 구조
    //현재장착중인 캐릭터 및 여러 쉽게사용하기위한 필요변수를 정의해두고 인벤매니저 인스턴스시 갱신하고 갱신되어야할때 갱신바로바로해줌
    public bool Check_Available_Acquire_Item(E_ItemKind kind, int item_id, int count)
    {
        switch (kind)
        {
            case E_ItemKind.Gold:
                break;
            case E_ItemKind.Character:
                if (Inven_Get_Empty() == null)
                {
                    return false;
                }
                break;
        }
        return true;

    }
    public void Acquire_Item(E_ItemKind kind, int item_id, int count, bool use_post = true)
    {
        if (Check_Available_Acquire_Item(kind, item_id, count) == false)
        {
            //인벤 자리없음
            //우편
            return;
        }

        switch (kind)
        {
            case E_ItemKind.Gold:
                Inventory User_Inven = Inven_Get_Item(E_ItemKind.Gold, 0);
                if (User_Inven != null)
                {
                    User_Inven.Count += count;
                }
                else
                {
                    User_Inven = Inven_Get_Empty();
                    User_Inven.Item_Kind = E_ItemKind.Gold;
                    User_Inven.Item_ID = item_id;
                    User_Inven.Count = count;
                }
                break;
            case E_ItemKind.Character:
                for (int i = 0; i < count; i++)
                {
                    User_Inven = Inven_Get_Empty();
                    User_Inven.Item_Kind = E_ItemKind.Character;
                    User_Inven.Item_ID = item_id;
                    User_Inven.Count = 1;
                }
                break;
        }

        Ev_Acquire_Item?.Invoke(this, EventArgs.Empty);
        Data.Instance.UserData_Changed();
    }

    public void Sell(Data_User.Inventory user_inven, int count)
    {
        D_Item cur_item = Data_Item.Instance.Get_Item(user_inven.Item_Kind, user_inven.Item_ID);

        for (int i = 0; i < cur_item.Sell_Item_Kind.Length; i++)
        {
            Acquire_Item(cur_item.Sell_Item_Kind[i], cur_item.Sell_Item_ID[i], cur_item.Sell_Item_Value[i] * count);
        }

        if (user_inven.Count <= count)
        {
            user_inven.Item_Kind = E_ItemKind.None;
            user_inven.Item_ID = 0;
            user_inven.Count = 0;
        }
        else
        {
            user_inven.Count -= count;
        }

        Ev_Sell_Item?.Invoke(this, EventArgs.Empty);
        Data.Instance.UserData_Changed();
    }
}
