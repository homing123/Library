using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class LoginManager : Manager<LoginManager>
{
    public static bool isLogin = false;
    public static bool LoginSequence_Success = false;

    const string TokenSet = "TokenSet";
    const string Check_DuplicateLogin = "Check_DuplicateLogin";

    private void Awake()
    {
        User_Login.m_UserLogin = new User_Login();
    }

    public IEnumerator LoginSequence()
    {
        isLogin = false;
        bool isfirstLogin = false;
        LoginSequence_Success = false;
        //�α��� ó��
        switch (User_Login.m_UserLogin.LoginType)
        {
            case User_Login.E_LoginType.None:
                //ù ����
                isfirstLogin = true;
                //�α��� �г� ����
                UI_Login.Create();
                //��ưŬ���� �ش� ��ư �α��� ����

                //�α��� ���� �� �ش�α��� Ÿ������ ����
                break;
            default:
                //�ٷ� �α��� ����
                LoginAdapter.Login(User_Login.m_UserLogin.LoginType);
                break;
                
        }

        while (isLogin == false)
        {
            yield return GameManager.seconds;
        }

        if (UserManager.Use_Local == false)
        {
            //���� ���� ��ȿ���˻�
            if (isfirstLogin)
            {
                var task = ServerConnector.Post(TokenSet, ServerConnector.ToJson(User_Login.m_UserLogin));
                while (task.IsCompleted == false)
                {
                    yield return GameManager.seconds;
                }      
            }
            else
            {
                //�������̵� ������ ��ū �ٸ���� �ߺ��α��� ����, �������̵� ��ū ��ġ�ϸ� ����α��� ����
                var task = ServerConnector.Post(Check_DuplicateLogin, ServerConnector.ToJson(User_Login.m_UserLogin));
                while (task.IsCompleted == false)
                {
                    yield return GameManager.seconds;
                }
                if(task.Result.code == ServerConnector.E_ServerRequestCode.Duplicate_Login)
                {
                    //�ߺ� �α��� �� ó��
                }
            }
        }

        LoginSequence_Success = true;
    }

    public IEnumerator AccountChangeSequence(User_Login.E_LoginType type)
    {
        if(User_Login.m_UserLogin.LoginType == type)
        {
            throw new System.Exception("AccountChange Sequence Type is Same");
        }

        if(User_Login.m_UserLogin.LoginType == User_Login.E_LoginType.Guest)
        {
            //�Խ�Ʈ �ְ� ���� �������
            yield return null;

            //�Խ�Ʈ �ְ� ���� �������


        }
        else
        {

        }
    }


    [ContextMenu("�α�")]
    public void Test_Log()
    {
        Debug.Log($"�α��� ���������� �α� Type : {User_Login.m_UserLogin.LoginType}, ID : {User_Login.m_UserLogin.ID}, Token : {User_Login.m_UserLogin.Token}, NickName : {User_Login.m_UserLogin.NickName}");
    }
    
}

public class LoginAdapter
{
    const bool Editor_Login = true;
    static User_Login.E_LoginType Type;
    public static void LoginSuccessed(string id, string token)
    {
        Debug.Log("�α��� ���� �Լ�");
        User_Login.m_UserLogin.Set_LoginInfo(Type, id, token);
        LoginManager.isLogin = true;
    }
    public static void LoginFailed()
    {
        Debug.Log("�α��� ���� ��õ�");
        Login(Type);
    }


    public static void Login(User_Login.E_LoginType type)
    {
        if (Editor_Login)
        {
            Type = type;
            TestLoginSdk.Instance.Login(type);
        }
        else
        {

        }
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