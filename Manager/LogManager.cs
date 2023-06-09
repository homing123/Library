using UnityEngine;

public class LogManager : SingleMono<LogManager>
{
    public LogManager()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            string Error_Log = "\n- Time : " + System.DateTime.Now + "\n- Log : " + logString + "\n- Trace: " + stackTrace + "\n- Type: " + type.ToString() + "\n";
            if (GameManager.Instance.Use_ErrorLog_Popup)
            {
                //�����α� �㶧���� �˾�

            }
        }
    }

    public static void Log(string log)
    {
        if (GameManager.Instance.Use_Log)
        {
            Debug.Log(log);
        }
        if (GameManager.Instance.Use_Log_Popup)
        {
            //�α� �㶧���� �˾�

        }
    }
}
