using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
public class IAP_Connector : IStoreListener
{

    public IAP_Connector()
    {
        //OnInitialized();
    }
    //초기화 성공
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        throw new System.NotImplementedException();
    }
    //초기화 실패
    public void OnInitializeFailed(InitializationFailureReason error, string? message)
    {
        LogManager.Log(error + " " + message);
    }
    //초기화 실패 미사용
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }
  

   

  
    //결제 취소
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
    }
    //결제 완료
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        return PurchaseProcessingResult.Complete;
    }
}
