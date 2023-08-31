using UnityEngine;
using System;
public class NetworkManager : Manager<NetworkManager>
{
    public static bool isNetworkConnect;
    public static event EventHandler<bool> ev_NetworkChanged;
    // Start is called before the first frame update
    void Awake()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            isNetworkConnect = false;
        }
        else
        {
            isNetworkConnect = true;
        }
    }

    void Update()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            if(isNetworkConnect == true)
            {
                isNetworkConnect = false;
                ev_NetworkChanged?.Invoke(this, false);
            }
        }
        else
        {
            if (isNetworkConnect == false)
            {
                isNetworkConnect = true;
                ev_NetworkChanged?.Invoke(this, true);
            }
        }
    }
}
