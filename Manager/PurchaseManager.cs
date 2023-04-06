using System;
using System.Collections.Generic;
using static Data_User;
using static UD_Func;
public class PurchaseManager : Single<PurchaseManager>
{
    //Trigger_Event : Buy, Sell
    //Register_Event : 

    public static event EventHandler Ev_Purchase;

    //구매가능한지 확인하는 함수
    public E_Check_Available_Purchase_Info Check_Available_Purchase(D_Product cur_product)
    {
        //획득할 자리 있는지 확인
        //자리없으면 우편으로넘길거임


        //비용을 낼수있는지 확인
        for (int i = 0; i < cur_product.Price_Kind.Length; i++)
        {
            switch (cur_product.Price_Kind[i])
            {
                case E_ItemKind.Ad:
                    break;
                case E_ItemKind.InAppPurchasing:
                    break;
            }
            Inventory User_Inven = Inven_Get_Item(cur_product.Price_Kind[i], cur_product.Price_Value[i]);
            if (User_Inven == null)
            {
                return E_Check_Available_Purchase_Info.Lack_Of_Price;
            }
            else
            {
                if (User_Inven.Count < cur_product.Price_Value[i])
                {
                    return E_Check_Available_Purchase_Info.Lack_Of_Price;
                }
            }
        }

        return E_Check_Available_Purchase_Info.Available;
    }

    public void Purchase(D_Product cur_product)
    {
        //스토어 구매 가능한지 확인
        if (Check_Available_Purchase(cur_product) != E_Check_Available_Purchase_Info.Available)
        {
            return;
        }

        //가격지불
        for (int i = 0; i < cur_product.Price_Kind.Length; i++)
        {
            Inventory User_Inven = Inven_Get_Item(cur_product.Price_Kind[i], cur_product.Price_Value[i]);
            User_Inven.Count -= cur_product.Price_Value[i];
        }

        //아이템 획득
        for (int i = 0; i < cur_product.Product_Kind.Length; i++) 
        {
            InvenManager.Instance.Acquire_Item(cur_product.Product_Kind[i], cur_product.Product_ID[i], cur_product.Product_Value[i]);
        }


        Ev_Purchase?.Invoke(this, EventArgs.Empty);

        Data.Instance.UserData_Changed();
    }
}
