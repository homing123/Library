using System;

public class InputManager : Manager<InputManager>
{
    public static event EventHandler<bool> ev_ApplicationPause;
    private void OnApplicationPause(bool pause)
    {
        ev_ApplicationPause?.Invoke(this, pause);
    }
}
