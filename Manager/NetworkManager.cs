using System;
using UnityEngine;
using System.Net.NetworkInformation;

public class NetworkManager : Single<NetworkManager>
{
    public  bool isNetwork_Connect;
    public static event EventHandler Ev_Network_Connect;
    public static event EventHandler Ev_Network_Disconnect;
    public NetworkManager()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                isNetwork_Connect = false;
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                isNetwork_Connect = true;
                break;
        }

        NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
    }

    private void OnDestroy()
    {
        NetworkChange.NetworkAvailabilityChanged -= NetworkChange_NetworkAvailabilityChanged;
    }
    private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
        if (e.IsAvailable == true)
        {
            isNetwork_Connect = true;
            Ev_Network_Connect?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            isNetwork_Connect = false;
            Ev_Network_Disconnect?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool Check_Network_Connect(bool Use_Message = true)
    {
        if (isNetwork_Connect == true)
        {
            return true;
        }
        else
        {
            if (Use_Message)
            {
                //¸Þ¼¼Áö
            }
            return false;
        }
    }
}
