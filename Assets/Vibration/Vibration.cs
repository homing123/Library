using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public static class Vibration
{
    const bool EditorLog = false;

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#else
    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaObject vibrator;
#endif


    public static void Vibrate()
    {
#if UNITY_EDITOR
        if (EditorLog)
        {
            Debug.Log("Vibrate");
        }
#elif UNITY_ANDROID
        vibrator.Call("vibrate", 700);

#elif UNITY_IOS
                Vibration_Plugin.Vibrate(1);
#endif   
    }

   
}

#if UNITY_IOS
public class Vibration_Plugin
{
    [DllImport("__Internal")]
    private static extern void vibrateWithIdx(int idx);
    public static void Vibrate(int idx)
    {
        vibrateWithIdx(idx);
        
    }
}
#endif