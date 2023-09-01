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
                Debug.Log("�����÷��� �α��� �õ�");

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
                Debug.Log("�Խ�Ʈ �α��� �õ�");
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
        Debug.Log("�α��� ����");

        LoginAdapter.LoginSuccessed(a, b);
    }
    void LoginFail()
    {
        Debug.Log("�α��� ����");

        LoginAdapter.LoginFailed();
    }
}
