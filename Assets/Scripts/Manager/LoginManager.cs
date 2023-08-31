using System.Collections;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class LoginManager : Manager<LoginManager>
{
    [SerializeField] GameObject G_LoginPanel;
    
    public static bool LoginSequence_Success = false;

    private void Awake()
    {
        User_Login.m_UserLogin = new User_Login();
    }
    public IEnumerator LoginSequence()
    {
        bool isfirstLogin = false;
        LoginSequence_Success = false;
        //�α��� ó��
        switch (User_Login.m_UserLogin.LoginType)
        {
            case User_Login.E_LoginType.None:
                //ù ����
                isfirstLogin = true;
                //�α��� �г� ����
                //��ưŬ���� �ش� ��ư �α��� ����

                //�α��� ���� �� �ش�α��� Ÿ������ ����
                break;
            default:
                //�ٷ� �α��� ����

                break;
                
        }


        //���� ������ ó��

        //ù�����̾��� ���
        if (isfirstLogin)
        {
            //���� �������̵� ������ �ִ��� Ȯ�� �� ������ ��ū �ֽ�ȭ
        }
        else
        {
            //�������̵� ������ ��ū �ٸ���� �ߺ��α��� ����, �������̵� ��ū ��ġ�ϸ� ����α��� ����
        }
        LoginSequence_Success = true;
    }

    
}

public class LoginAdapter
{
    const bool Editor_Login = true;
    static User_Login.E_LoginType Type;
    public static void LoginSuccessed( string id, string token)
    {
        Debug.Log("�α��� ����");
        User_Login.m_UserLogin.Set_LoginInfo(Type, id, token);
    }
    

    public static void Login(User_Login.E_LoginType type)
    {
        if (Editor_Login)
        {
            Type = type;
            Test_LoginSdk.Instance.Login(type);
        }
        else
        {

        }
    }
}

public class Test_LoginSdk : MonoBehaviour
{
    public static Test_LoginSdk Instance;
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
                break;
        }
    }
    void LoginSuccess(string a, string b)
    {
        LoginAdapter.LoginSuccessed(a, b);
    }
    void LoginFail()
    {

    }
}

public class User_Login : UserData_Local
{
    public enum E_LoginType
    {
        None = 0,
        Guest = 1,
        GooglePlay = 2,
        GameCenter = 3,
    }
    public const string Path = "Login";
    public static User_Login m_UserLogin;

    public E_LoginType LoginType;
    public string ID;
    public string Token;
    public string NickName;

    public override void Load()
    {
        if (UserManager.Exist_LocalUD(Path))
        {
            var data = UserManager.Load_LocalUD<User_Login>(Path);
            LoginType = data.LoginType;
            ID = data.ID;
            Token = data.Token;
            NickName = data.NickName;
        }
        else
        {
            Debug.Log("Login_Init");

            LoginType = E_LoginType.None;
            UserManager.Save_LocalUD(Path, this);
        }
    }

    public void Set_LoginInfo(E_LoginType type, string id, string token)
    {
        LoginType = type;
        ID = id;
        Token = token;
        UserManager.Save_LocalUD(Path, this);
    }
    public void Set_NickName(string nickname)
    {
        NickName = nickname;
        UserManager.Save_LocalUD(Path, this);
    }
}
//ù �α��� Ȯ��

//ù�α��� = �α��� â ���� => �α��οϷ�� ��ū , �������̵� ȹ�� => ���� �������̵� ������ �ִ��� Ȯ�� �� ������ ��ū �ֽ�ȭ

//ù�α��� x = �������ߴ� �α������� �α��� => �������̵� ������ ��ū �ٸ���� �ߺ��α��� ����, �������̵� ��ū ��ġ�ϸ� ����α��� ����

//��������� �Խ�Ʈ�� ��� ���� ���� = �Խ�Ʈ������ ������� ��� �����Ͱ� ������� => ����

//�Խ�Ʈ�� ������� �Խ�Ʈ �����Ͷ� ������� ����

//�Ѵپ������, ��������� ������� ���� �Ұ��� (�Խ�Ʈ �α��� �� ����ϴ� �����)