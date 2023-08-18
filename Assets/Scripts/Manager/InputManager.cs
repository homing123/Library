using System;
using UnityEngine;
public class InputManager : Manager<InputManager>
{
    public static event EventHandler<bool> ev_ApplicationPause;

    public static event EventHandler<int> ev_MouseDown;

    private void OnApplicationPause(bool pause)
    {
        ev_ApplicationPause?.Invoke(this, pause);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ev_MouseDown?.Invoke(this, 0);
        }
    }
}
