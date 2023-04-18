using System;
public class LoginAdapter
{
    public static bool isLogin;
    public static string Login_ID;
    public static void Login(Action Ac_Success = null, Action Ac_Fail = null)
    {
#if UNITY_EDITOR
        Ac_Success?.Invoke();
#elif UNITY_STANDALONE_WIN
        Ac_Success?.Invoke();
#elif UNITY_ANDROID
         GPGSBinder.Inst.Login((success, localuser) =>
        {
            if (success)
            {
                isLogin = true;
                Login_ID = localuser.id;

                Ac_Success?.Invoke();
            }
            else
            {
                Ac_Fail?.Invoke();
            }
        }
        );
#elif UNITY_IOS
         Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                isLogin = true;
                Login_ID = Social.localUser.id.Replace(":", "_");

                Ac_Success?.Invoke();
            }
            else
            {
                Ac_Fail?.Invoke();
            }
        });
#endif
    }
}
