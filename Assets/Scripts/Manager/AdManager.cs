using System;
using System.Collections.Generic;
using UnityEngine;
using static AdManager;

public class AdManager : Manager<AdManager>
{
    public enum E_Adkind 
    {
        Banner,
        Interstitial,
        Reward_Video,
    }

    public enum E_AdSdk
    {
        Test,
        Admob,
        AdPopcorn,
    }

    public static bool isShow = false;
    public I_AdSdk_Adapter i_Adapter;
    public Action ac_Reward;

    public const int Ad_Count = 2;
    const float Reload_Time = 30;
    public float cur_Time = 0;
    public static Dictionary<E_Adkind, EventHandler<bool>> D_ev_Loaded = new Dictionary<E_Adkind, EventHandler<bool>>() {
        {
            E_Adkind.Interstitial, null
        } ,
        {
            E_Adkind.Reward_Video, null
        }
    };
    private void Awake()
    {
        if (GameSetting.Ad == false)
        {
            i_Adapter = null;
        }
        else
        {
            switch (GameSetting.AdSdk)
            {
                case E_AdSdk.Test:
                    //i_Adapter = new Ad_Sdk_Test();
                    break;
                case E_AdSdk.Admob:
                    break;
                case E_AdSdk.AdPopcorn:
                    break;
            }
        }

        GameManager.ac_DataLoaded += Reload;
    }
    
    private void Update()
    {
        Reload_Check();
    }
    void Reload_Check()
    {
        cur_Time += Time.unscaledDeltaTime;
        if (cur_Time > Reload_Time)
        {
            cur_Time = 0;
            Reload();
        }
    }
    void Reload()
    {
        if (i_Adapter != null)
        {
            i_Adapter.Reload();
        }
    }

    public void Show(E_Adkind adkind, Action ac_reward = null)
    {
        if (i_Adapter != null)
        {
            ac_Reward = ac_reward;
            i_Adapter.Show(adkind);
        }
        else
        {
            ac_reward?.Invoke();
        }
    }
    public bool isLoaded(E_Adkind adkind)
    {
        if(i_Adapter != null)
        {
            return i_Adapter.isLoaded(adkind);
        }
        else
        {
            return true;
        }
    }
    
    //로드 여부 확인할수있어야하고 로드완료 and 로드안됨 이벤트 있어야함
    //로드 실패시 주기적으로 다시 불러줘야함

    //로드 -> 성공 or 실패결과 처리 실패시 다시 시도
    //서버보내기 실패시 다시보내기 여러번 반복 후 실패계속되면 실패처리

}


public abstract class I_AdSdk_Adapter
{
    public class Ad_Info 
    {
        public bool Loading;
        public int LoadedCount;
    }

    public static Dictionary<E_Adkind, Ad_Info> D_AdInfo = new Dictionary<E_Adkind, Ad_Info>()
    {
        {
            E_Adkind.Interstitial,new Ad_Info()
        } ,
        {
            E_Adkind.Reward_Video,new Ad_Info()
        }
    };
    public bool isLoaded(E_Adkind ad_kind)
    {
        if (D_AdInfo[ad_kind].LoadedCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Loaded(E_Adkind adkind)
    {
        D_AdInfo[adkind].Loading = false;
        D_AdInfo[adkind].LoadedCount++;
        if (D_AdInfo[adkind].LoadedCount == 1)
        {
            AdManager.D_ev_Loaded[adkind]?.Invoke(this, true);
        }
    }
    public void Show(E_Adkind adkind)
    {
        AdManager.isShow = true;
        if (D_AdInfo[adkind].LoadedCount > 0)
        {
            D_AdInfo[adkind].LoadedCount--;
            if (D_AdInfo[adkind].LoadedCount == 0)
            {
                AdManager.D_ev_Loaded[adkind]?.Invoke(this, false);
            }
        }
        else
        {
            throw new Exception("AdLoaded Count is 0. kind : " + adkind);
        }
    }
    public void Close(E_Adkind adkind, bool reward)
    {
        AdManager.isShow = false;
        Load(adkind);
        if (reward)
        {
            AdManager.Instance.ac_Reward?.Invoke();
        }
    }
    public void Load(E_Adkind adkind)
    {
        D_AdInfo[adkind].Loading = true;
    }
    public void LoadFail(E_Adkind adkind)
    {
        D_AdInfo[adkind].Loading = false;
    }
    public void Reload()
    {
        foreach(KeyValuePair<E_Adkind, Ad_Info> keyvalue in D_AdInfo)
        {
            if (keyvalue.Value.Loading == false && keyvalue.Value.LoadedCount < AdManager.Ad_Count)
            {
                Load(keyvalue.Key);
            }
        }
    }

}


//public class Ad_Sdk_Test : I_AdSdk_Adapter
//{
  



//}

