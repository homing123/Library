using System.Collections;
using UnityEngine;
using System;
using static Define;

public class AdManager : Single<AdManager>
{
    public I_Ad_SDK M_SDK;
    Action Ac_Reward;
    Action Ac_Cancel;

    bool before_isLoaded_Interstitial;
    bool before_isLoaded_Reward;

    bool isAd = false;

    public static event EventHandler Ev_Interstitial_Loaded; //전면광고 로드 완료될때 이벤트
    public static event EventHandler Ev_Interstitial_None; //전면광고 로드 안되있을때 이벤트
    public static event EventHandler Ev_Reward_Loaded; //리워드광고 로드 완료될때 이벤트
    public static event EventHandler Ev_Reward_None; //리워드광고 로드 안되있을때 이벤트
    float Cur_AD_ReLoad_Time;

    private void Awake()
    {
        Setting();
    }
    void Setting()
    {
        if (GameManager.Instance.Use_AD)
        {
            if (M_SDK == null)
            {
                switch (GameManager.Instance.AD_SDK)
                {
                    case E_ADSDK.AdPopcorn:
                        //M_SDK = new AdPopcorn();
                        break;
                }
                if (M_SDK == null)
                {
                    return;
                }
                M_SDK.Setting();
                M_SDK.Load_Interstitial();
                M_SDK.Load_Reward();
                Show(E_AdKind.Banner);
            }
        }
    }


    private void Update()
    {
        if (GameManager.Instance.Use_AD == false)
        {
            return;
        }
        if (before_isLoaded_Interstitial != M_SDK.isLoaded_Interstitial())
        {
            before_isLoaded_Interstitial = M_SDK.isLoaded_Interstitial();
            if (M_SDK.isLoaded_Interstitial())
            {
                Ev_Interstitial_Loaded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Ev_Interstitial_None?.Invoke(this, EventArgs.Empty);
            }
        }

        if (before_isLoaded_Reward != M_SDK.isLoaded_Reward())
        {
            before_isLoaded_Reward = M_SDK.isLoaded_Reward();
            if (M_SDK.isLoaded_Reward())
            {
                Ev_Reward_Loaded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Ev_Reward_None?.Invoke(this, EventArgs.Empty);
            }
        }

        ReLoad();
    }
    void ReLoad()
    {
        Cur_AD_ReLoad_Time += Time.deltaTime;
        if (Cur_AD_ReLoad_Time > AD_ReLoad_Time)
        {
            Cur_AD_ReLoad_Time = 0;
            if (M_SDK.isFail_Interstitial())
            {
                M_SDK.Load_Interstitial();
            }
            if (M_SDK.isFail_Reward())
            {
                M_SDK.Load_Reward();
            }
        }
    }

    public bool Check_isLoad(E_AdKind adkind)
    {
        if (GameManager.Instance.Use_AD == false)
        {
            return true;
        }
        switch (adkind)
        {
            case E_AdKind.Interstitial:
                return M_SDK.isLoaded_Interstitial();
            case E_AdKind.Reward_Video:
                return M_SDK.isLoaded_Reward();
        }
        return false;
    }


    public void Show(E_AdKind adkind, Action ac_reward = null, Action ac_cancel = null)
    {
        if (GameManager.Instance.Use_AD == false)
        {
            ac_reward?.Invoke();
            return;
        }
        isAd = true;
        switch (adkind)
        {
            case E_AdKind.Banner:
                M_SDK.Show_Banner();
                break;
            case E_AdKind.Interstitial:
                M_SDK.Show_Interstitial();
                break;
            case E_AdKind.Reward_Video:
                M_SDK.Show_Reward();
                break;
        }
        Ac_Reward = ac_reward;
        Ac_Cancel = ac_cancel;
    }
    public void Reward()
    {
        if (M_SDK == null)
        {
            return;
        }
        Ac_Reward?.Invoke();
        Ac_Reward = null;
        isAd = false;
    }
    public void Cancel()
    {
        if (M_SDK == null)
        {
            return;
        }
        Ac_Cancel?.Invoke();
        Ac_Cancel = null;
        isAd = false;
    }
    private void OnDestroy()
    {
        if (M_SDK != null)
        {
            M_SDK.Destroy();
        }
    }


    public void Load()
    {
        if (M_SDK == null)
        {
            return;
        }
        M_SDK.Load_Interstitial();
        M_SDK.Load_Reward();
    }
    public void Close_Banner()
    {
        if (M_SDK == null)
        {
            return;
        }
        M_SDK.Close_Banner();
    }
}
public abstract class I_Ad_SDK
{
    public abstract void Setting();
    public abstract void Show_Banner();
    public abstract void Close_Banner();
    public abstract void Load_Interstitial();
    public abstract void Show_Interstitial();
    public abstract bool isLoaded_Interstitial();

    public abstract bool isFail_Interstitial();
    public abstract void Load_Reward();
    public abstract void Show_Reward();
    public abstract bool isLoaded_Reward();
    public abstract bool isFail_Reward();


    public abstract void Destroy();

}