using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;

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

    public static async Task<E_DataConnector_Request> Upload(string data, E_FTP_Kind kind)
    {
        string Path = Get_Path(kind);

        E_DataConnector_Request request = E_DataConnector_Request.None;
        if (NetworkManager.Instance.isNetwork_Connect == false)
        {
            request = E_DataConnector_Request.No_Network_Connection;
        }
        else if (LoginAdapter.isLogin)
        {
            request = E_DataConnector_Request.Not_Logged_In;
        }
        else
        {
            byte[] byte_data = Encoding.UTF8.GetBytes(data);
            
            try
            {
                FtpWebRequest ftp_req = WebRequest.Create(Path) as FtpWebRequest;

                //업로드용이라고 메소드 설정
                ftp_req.Method = WebRequestMethods.Ftp.UploadFile;

                //아이디 비번으로 자격증명 생성
                ftp_req.Credentials = new NetworkCredential(FTP_ID, FTP_Password);

                //패시브모드 설정 보통 false (false 는 서버 -> 클라 로 연결하고 true 는 클라 -> 서버 로 연결한다) (true는 방화벽에 제한을 받지않지만 서버보안에 취약하다)
                ftp_req.UsePassive = false;

                //데이터 길이 설정 (서버에서 적절한 크기의 공간 준비)
                ftp_req.ContentLength = byte_data.Length;

                //서버응답 대기시간 제한  1 second = 1000 (기본값은 무한임)
                ftp_req.Timeout = 5000;

                //프록시 서버 사용안함 (기본값 null임)
                //ftp_req.Proxy = null;

                //메모리 자동해제를 위한 using
                using (Stream reqStream = await ftp_req.GetRequestStreamAsync())
                {
                    //서버로 데이터 전달
                    await reqStream.WriteAsync(byte_data, 0, byte_data.Length);
                }

                //서버로 부터 응답받기 (응답 후 처리가 없어도 응답을 받아야함 없으면 오류생긴다고함)
                using (FtpWebResponse ftp_response = await ftp_req.GetResponseAsync() as FtpWebResponse)
                {
                    int status_value = (int)ftp_response.StatusCode;
                    LogManager.Log("FTP_Upload Response : " + ftp_response.StatusCode + " " + status_value);
                    //200~299 Success
                    if (status_value >= 200 && status_value <= 299)
                    {
                        request = E_DataConnector_Request.Success;
                    }
                    //400~599 Fail
                    else if (status_value >= 400 && status_value <= 599)
                    {
                        if (status_value == (int)FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            request = E_DataConnector_Request.File_None;
                        }
                        else
                        {
                            request = E_DataConnector_Request.Fail_Unknown;
                        }
                    }
                    else
                    {
                        request = E_DataConnector_Request.Fail_Unknown;
                    }
                }

            }
            catch (WebException e)
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.Timeout:
                        LogManager.Log("FTP_Upload Exception : TimeOut , " + e.Status + " " + request);
                        break;
                    default:
                        LogManager.Log("FTP_Upload Exception : " + e.Status + " " + request);
                        break;
                }
                request = E_DataConnector_Request.Fail_Unknown;
            }
        }

        return request;
    }
    public static async Task<(E_DataConnector_Request request, string data)> Download(E_FTP_Kind kind)
    {
        string Path = Get_Path(kind);

        string Data = null;
        E_DataConnector_Request request = E_DataConnector_Request.None;
        if (NetworkManager.Instance.isNetwork_Connect == false)
        {
            request = E_DataConnector_Request.No_Network_Connection;
        }
        else if (LoginAdapter.isLogin)
        {
            request = E_DataConnector_Request.Not_Logged_In;
        }
        else
        {
            try
            {
                FtpWebRequest ftp_req = WebRequest.Create(Path) as FtpWebRequest;
                ftp_req.Method = WebRequestMethods.Ftp.DownloadFile;
                ftp_req.Credentials = new NetworkCredential(FTP_ID, FTP_Password);
                ftp_req.UsePassive = false;
                ftp_req.Timeout = 5000;

                //서버로 부터 응답받기 (응답 후 처리가 없어도 응답을 받아야함 없으면 오류생긴다고함)
                using (FtpWebResponse ftp_response = await ftp_req.GetResponseAsync() as FtpWebResponse)
                {
                    int status_value = (int)ftp_response.StatusCode;
                    LogManager.Log("FTP_Download Response : " + ftp_response.StatusCode + " " + status_value);
                    //200~299 Success
                    if (status_value >= 200 && status_value <= 299)
                    {
                        using (StreamReader reader = new StreamReader(ftp_response.GetResponseStream()))
                        {
                            Data = await reader.ReadToEndAsync();
                            request = E_DataConnector_Request.Success;
                        }
                    }
                    //400~599 Fail
                    else if (status_value >= 400 && status_value <= 599)
                    {
                        if (status_value == (int)FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            request = E_DataConnector_Request.File_None;
                        }
                        else
                        {
                            request = E_DataConnector_Request.Fail_Unknown;
                        }
                    }
                    else
                    {
                        request = E_DataConnector_Request.Fail_Unknown;
                    }
                }

            }
            catch (WebException e)
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.Timeout:
                        LogManager.Log("FTP_Download Exception : TimeOut , " + e.Status + " " + request);
                        break;
                    default:
                        LogManager.Log("FTP_Download Exception : " + e.Status + " " + request);
                        break;
                }
                request = E_DataConnector_Request.Fail_Unknown;
            }
        }


        return (request, Data);
    }
    public static async Task<E_DataConnector_Request> Delete(E_FTP_Kind kind)
    {
        string Path = Get_Path(kind);

        E_DataConnector_Request request = E_DataConnector_Request.None;
        if (NetworkManager.Instance.isNetwork_Connect == false)
        {
            request = E_DataConnector_Request.No_Network_Connection;
        }
        else if (LoginAdapter.isLogin)
        {
            request = E_DataConnector_Request.Not_Logged_In;
        }
        else
        {
            try
            {
                FtpWebRequest ftp_req = WebRequest.Create(Path) as FtpWebRequest;
                ftp_req.Method = WebRequestMethods.Ftp.DeleteFile;
                ftp_req.Credentials = new NetworkCredential(FTP_ID, FTP_Password);
                ftp_req.UsePassive = false;
                ftp_req.Timeout = 5000;

                //서버로 부터 응답받기 (응답 후 처리가 없어도 응답을 받아야함 없으면 오류생긴다고함)
                using (FtpWebResponse ftp_response = await ftp_req.GetResponseAsync() as FtpWebResponse)
                {
                    int status_value = (int)ftp_response.StatusCode;
                    LogManager.Log("FTP_Delete Response : " + ftp_response.StatusCode + " " + status_value);
                    //200~299 Success
                    if (status_value >= 200 && status_value <= 299)
                    {
                        request = E_DataConnector_Request.Success;
                    }
                    //400~599 Fail
                    else if (status_value >= 400 && status_value <= 599)
                    {
                        if (status_value == (int)FtpStatusCode.ActionNotTakenFileUnavailable)
                        {
                            request = E_DataConnector_Request.File_None;
                        }
                        else
                        {
                            request = E_DataConnector_Request.Fail_Unknown;
                        }
                    }
                    else
                    {
                        request = E_DataConnector_Request.Fail_Unknown;
                    }
                }

            }
            catch (WebException e)
            {
                switch (e.Status)
                {
                    case WebExceptionStatus.Timeout:
                        LogManager.Log("FTP_Delete Exception : TimeOut , " + e.Status + " " + request);
                        break;
                    default:
                        LogManager.Log("FTP_Delete Exception : " + e.Status + " " + request);
                        break;
                }
                request = E_DataConnector_Request.Fail_Unknown;
            }
        }

        return request;
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
