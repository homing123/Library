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
        //로그인 처리
        switch (User_Login.m_UserLogin.LoginType)
        {
            case User_Login.E_LoginType.None:
                //첫 접속
                isfirstLogin = true;
                //로그인 패널 생성
                UI_Login.Create();
                //버튼클릭시 해당 버튼 로그인 실행

                //로그인 성공 시 해당로그인 타입으로 저장
                break;
            default:
                //바로 로그인 실행
                LoginAdapter.Login(User_Login.m_UserLogin.LoginType);
                break;
                
        }

        while (isLogin == false)
        {
            yield return GameManager.seconds;
        }

        if (UserManager.Use_Local == false)
        {
            //서버 사용시 유효성검사
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
                //유저아이디 같으나 토큰 다를경우 중복로그인 리턴, 유저아이디 토큰 일치하면 정상로그인 리턴
                var task = ServerConnector.Post(Check_DuplicateLogin, ServerConnector.ToJson(User_Login.m_UserLogin));
                while (task.IsCompleted == false)
                {
                    yield return GameManager.seconds;
                }
                if(task.Result.code == ServerConnector.E_ServerRequestCode.Duplicate_Login)
                {
                    //중복 로그인 시 처리
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
            //게스트 있고 구글 없을경우
            yield return null;

            //게스트 있고 구글 잇을경우


        }
        else
        {

        }
    }


    [ContextMenu("로그")]
    public void Test_Log()
    {
        Debug.Log($"로그인 유저데이터 로그 Type : {User_Login.m_UserLogin.LoginType}, ID : {User_Login.m_UserLogin.ID}, Token : {User_Login.m_UserLogin.Token}, NickName : {User_Login.m_UserLogin.NickName}");
    }
    
}

public class LoginAdapter
{
    const bool Editor_Login = true;
    static User_Login.E_LoginType Type;
    public static void LoginSuccessed(string id, string token)
    {
        Debug.Log("로그인 성공 함수");
        User_Login.m_UserLogin.Set_LoginInfo(Type, id, token);
        LoginManager.isLogin = true;
    }
    public static void LoginFailed()
    {
        Debug.Log("로그인 실패 재시도");
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
//첫 로그인 확인

//첫로그인 = 로그인 창 출현 => 로그인완료시 토큰 , 유저아이디 획득 => 같은 유저아이디 서버에 있는지 확인 후 있으면 토큰 최신화

//첫로그인 x = 이전에했던 로그인으로 로그인 => 유저아이디 같으나 토큰 다를경우 중복로그인 리턴, 유저아이디 토큰 일치하면 정상로그인 리턴

//현재계정이 게스트일 경우 계정 변경 = 게스트계정과 변경계정 모두 데이터가 있을경우 => 선택

//게스트만 있을경우 게스트 데이터랑 변경계정 연결

//둘다없을경우, 변경계정만 있을경우 존재 불가능 (게스트 로그인 후 사용하는 기능임)