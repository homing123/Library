
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using UnityEngine;
using Newtonsoft.Json;
using System.Text;
public class ServerConnector
{

    const int Retry_Delay = 1000; // 1000 = 1second
    const int Retry_Count = 3;
    public enum E_ServerRequestCode
    {
        None = 0,
        Success = 200,
        Duplicate_Login = 201,
        Fail = 400,
    }
    public static void ServerError(string url, string errormessage, E_ServerRequestCode code)
    {
        Debug.Log($"Server Error : URL : {url} \n ErrorCode : {code} \n ErrorMessage : {errormessage}");
    }

    public static async Task<(E_ServerRequestCode code, JToken data)> Post(string url, string data)
    {
        using(var httpClient = new HttpClient())
        {
            HttpResponseMessage response = null;
            bool isResponse_Success = false;
            E_ServerRequestCode statuscode = E_ServerRequestCode.None;

            HttpContent content = data != null ? new StringContent(data, Encoding.UTF8, "application/json") : null;

            for (int try_count = 0; try_count < Retry_Count; try_count++) 
            {
                response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    isResponse_Success = true;
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject jsonResponse = JObject.Parse(responseBody);
                    statuscode = jsonResponse["statusCode"].Value<E_ServerRequestCode>();
                    if ((int)statuscode >= 200 && (int)statuscode <= 299)
                    {
                        JToken bodydata = JObject.Parse(jsonResponse["body"].ToString());
                        return (statuscode, bodydata);
                    }
                }

                await Task.Delay(Retry_Delay);
            }

            if (isResponse_Success)
            {
                ServerError(url, response.ReasonPhrase, E_ServerRequestCode.None);
            }
            else
            {
                ServerError(url, "Check_Code", statuscode);
            }
            throw new System.Exception("Server Error");
        }
    }

    public static async Task<(E_ServerRequestCode code, JToken data)> Get(string url)
    {
        using (var httpClient = new HttpClient())
        {
            HttpResponseMessage response = null;
            bool isResponse_Success = false;
            E_ServerRequestCode statuscode = E_ServerRequestCode.None;

            for (int try_count = 0; try_count < Retry_Count; try_count++)
            {
                response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    isResponse_Success = true;
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject jsonResponse = JObject.Parse(responseBody);
                    statuscode = jsonResponse["statusCode"].Value<E_ServerRequestCode>();
                    if ((int)statuscode >= 200 && (int)statuscode <= 299)
                    {
                        JToken bodydata = JObject.Parse(jsonResponse["body"].ToString());
                        return (statuscode, bodydata);
                    }
                }

                await Task.Delay(Retry_Delay);
            }

            if (isResponse_Success)
            {
                ServerError(url, response.ReasonPhrase, E_ServerRequestCode.None);
            }
            else
            {
                ServerError(url, "Check_Code", statuscode);
            }
            throw new System.Exception("Server Error");
        }
    }

    public static string ToJson<T>(T obj, int kind = 0)
    {
        //¿©±â¼­ 
        switch (kind)
        {
            case 0:
                return JsonConvert.SerializeObject(obj);
            default:
                return null;
        }
    }
}
