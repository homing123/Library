using UnityEngine;
using System;

public class NotchHandler : MonoBehaviour
{
    public static event EventHandler Ev_Notch_Change;
    public static float Bottom_Notch_Height { get; private set; }

    public static void Change_Bottom_Notch_Height(float bottom_notch_height)
    {
        Bottom_Notch_Height = bottom_notch_height;
        Ev_Notch_Change?.Invoke(null, EventArgs.Empty);
    }
}
