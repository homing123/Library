using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Login : MonoBehaviour
{

    public static UI_Login Instance;

    public static UI_Login Create()
    {
        if(Instance == null)
        {
            Instance = Instantiate(Prefabs.Instance.UI_Login).GetComponent<UI_Login>();
        }
        return Instance;
    }
    public static void Destroy()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    public void Btn_GoogleLogin()
    {
        LoginAdapter.Login(User_Login.E_LoginType.GooglePlay);
        Destroy();
    }
    public void Btn_GuestLogin()
    {
        LoginAdapter.Login(User_Login.E_LoginType.Guest);
        Destroy();
    }
    // Start is called before the first frame update
}
