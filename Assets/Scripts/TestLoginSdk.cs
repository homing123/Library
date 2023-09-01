using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoginSdk : MonoBehaviour
{
    [SerializeField] bool issuccess;
    public static TestLoginSdk Instance;
    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Success()
    {
        yield return new WaitForSeconds(1);
        LoginSuccess("User_id", "Token");
    }
    IEnumerator Fail()
    {
        yield return new WaitForSeconds(1);
        LoginFail();
    }

    public void Login(User_Login.E_LoginType type)
    {
        switch (type)
        {
            case User_Login.E_LoginType.GooglePlay:
                Debug.Log("구글플레이 로그인 시도");

                if (issuccess)
                {
                    StartCoroutine(Success());
                }
                else
                {
                    StartCoroutine(Fail());
                }
                break;
            case User_Login.E_LoginType.Guest:
                Debug.Log("게스트 로그인 시도");
                if (issuccess)
                {
                    StartCoroutine(Success());
                }
                else
                {
                    StartCoroutine(Fail());
                }
                break;
        }
    }
    void LoginSuccess(string a, string b)
    {
        Debug.Log("로그인 성공");

        LoginAdapter.LoginSuccessed(a, b);
    }
    void LoginFail()
    {
        Debug.Log("로그인 실패");

        LoginAdapter.LoginFailed();
    }
}
