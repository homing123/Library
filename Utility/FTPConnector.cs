using System;
using System.Threading.Tasks;
using System.Net;

public enum E_FTP_Kind
{
    UserData = 0,
    ErrorFile = 1
}
public class FTPConnector
{
    const string FTP_ID = "";
    const string FTP_Password = "";
    const string FTP_Path = "";

    void temp()
    {

    }
    public static async Task Upload(string data, E_FTP_Kind kind, Action<E_DataConnector_Request> Ac_Success = null, Action<E_DataConnector_Request> Ac_Fail = null)
    {
        string Path = Get_Path(kind);

        E_DataConnector_Request request;

        if (NetworkManager.Instance.isNetwork_Connect == false)
        {
            request = E_DataConnector_Request.No_Network_Connection;
        }
        else if (LoginAdapter.isLogin)
        {
            request = E_DataConnector_Request.Not_Logged_In;
        }
        try
        {
            FtpWebRequest ftp_req = WebRequest.Create(Path) as FtpWebRequest;
            ftp_req.Method = WebRequestMethods.Ftp.UploadFile;

            
        }
        catch (Exception e)
        {

        }
    }
    public static async Task<string> Download(E_FTP_Kind kind, Action<E_DataConnector_Request> Ac_Success = null, Action<E_DataConnector_Request> Ac_Fail = null)
    {

    }
    public static async Task Delete()
    {
        
    }

    static string Get_Path(E_FTP_Kind path_kind)
    {
        switch (path_kind)
        {
            case E_FTP_Kind.UserData:
#if UNITY_EDITOR
                return "";
#elif UNITY_ANDROID
                return "";
#elif UNITY_IOS
                return "";
#endif
            case E_FTP_Kind.ErrorFile:
#if UNITY_EDITOR
                return "";
#elif UNITY_ANDROID
                return "";
#elif UNITY_IOS
                return "";
#endif
        }
        return "";
    }
}
