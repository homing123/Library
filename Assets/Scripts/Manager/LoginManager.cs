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
        //로그인 처리
        switch (User_Login.m_UserLogin.LoginType)
        {
            case User_Login.E_LoginType.None:
                //첫 접속
                isfirstLogin = true;
                //로그인 패널 생성
                //버튼클릭시 해당 버튼 로그인 실행

                //로그인 성공 시 해당로그인 타입으로 저장
                break;
            default:
                //바로 로그인 실행

                break;
                
        }


        //유저 데이터 처리

        //첫접속이었는 경우
        if (isfirstLogin)
        {
            //같은 유저아이디 서버에 있는지 확인 후 있으면 토큰 최신화
        }
        else
        {
            //유저아이디 같으나 토큰 다를경우 중복로그인 리턴, 유저아이디 토큰 일치하면 정상로그인 리턴
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
        Debug.Log("로그인 성공");
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
                Debug.Log("구글플레이 로그인 시도");
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
//첫 로그인 확인

//첫로그인 = 로그인 창 출현 => 로그인완료시 토큰 , 유저아이디 획득 => 같은 유저아이디 서버에 있는지 확인 후 있으면 토큰 최신화

//첫로그인 x = 이전에했던 로그인으로 로그인 => 유저아이디 같으나 토큰 다를경우 중복로그인 리턴, 유저아이디 토큰 일치하면 정상로그인 리턴

//현재계정이 게스트일 경우 계정 변경 = 게스트계정과 변경계정 모두 데이터가 있을경우 => 선택

//게스트만 있을경우 게스트 데이터랑 변경계정 연결

//둘다없을경우, 변경계정만 있을경우 존재 불가능 (게스트 로그인 후 사용하는 기능임)