using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
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
    public static bool isReward = false;
    public ADAdapter Cur_Adapter;
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
            Cur_Adapter = null;
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
        if (Cur_Adapter != null)
        {
            Cur_Adapter.Reload();
        }
    }

    public void Show(E_Adkind adkind, Action ac_reward = null)
    {
        if (Cur_Adapter != null)
        {
            ac_Reward = ac_reward;
            Cur_Adapter.Show(adkind);
        }
        else
        {
            ac_reward?.Invoke();
        }
    }

    public async Task<bool> ShowAsync(E_Adkind adkind)
    {
        if (Cur_Adapter != null)
        {
            Cur_Adapter.Show(adkind);
            while (isShow)
            {
                await Task.Delay(10);
            }
            await Task.Delay(10);
            return isReward;
        }
        else
        {
            await Task.Delay(10);
            return true;
        }
    }
    public bool isLoaded(E_Adkind adkind)
    {
        if(Cur_Adapter != null)
        {
            return Cur_Adapter.isLoaded(adkind);
        }
        else
        {
            return true;
        }
    }
    
    //�ε� ���� Ȯ���Ҽ��־���ϰ� �ε�Ϸ� and �ε�ȵ� �̺�Ʈ �־����
    //�ε� ���н� �ֱ������� �ٽ� �ҷ������

    //�ε� -> ���� or ���а�� ó�� ���н� �ٽ� �õ�
    //���������� ���н� �ٽú����� ������ �ݺ� �� ���а�ӵǸ� ����ó��

}


public abstract class ADAdapter
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
        isReward = false;
        if (D_AdInfo[adkind].LoadedCount > 0)
        {
            D_AdInfo[adkind].LoadedCount--;
            if (D_AdInfo[adkind].LoadedCount == 0)
            {
                AdManager.D_ev_Loaded[adkind]?.Invoke(this, false);
            }
            _Show(adkind);
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
            isReward = true;
            AdManager.Instance.ac_Reward?.Invoke();
        }
    }
    public void Load(E_Adkind adkind)
    {
        D_AdInfo[adkind].Loading = true;
        _Load(adkind);
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

    public abstract void _Load(E_Adkind adkind);
    public abstract void _Show(E_Adkind adkind);

}


//public class Ad_Sdk_Test : I_AdSdk_Adapter
//{
  



//}

