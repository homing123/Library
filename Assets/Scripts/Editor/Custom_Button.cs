using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Diagnostics;
public class Custom_Button : Editor
{
    [MenuItem("Custom/Delete")]
    public static void Delete()
    {
        UserManager.Delete_Local();
    }
    [MenuItem("GameObject/UI/TextEx")]
    public static void Create_TextEx()
    {
        GameObject obj = new GameObject("TextEx");
        obj.AddComponent<TextEx>();

        Create_UI(obj);

    }

    [MenuItem("GameObject/UI/ButtonEx")]
    public static void Create_ButtonEx()
    {
        GameObject obj = new GameObject("ButtonEx");
        ButtonEx m_button = obj.AddComponent<ButtonEx>();
        m_button.targetGraphic = obj.AddComponent<Image>();
        GameObject text = new GameObject("TextEx");
        text.AddComponent<TextEx>();
        text.transform.SetParent(obj.transform);
        Create_UI(obj);

    }

    static void Create_UI(GameObject obj)
    {
        obj.layer = LayerMask.NameToLayer("UI");

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
        }

        if (Selection.activeGameObject)
        {
            Canvas m_canvas = Selection.activeGameObject.GetComponentInParent<Canvas>();
            if(m_canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                m_canvas = canvasGO.AddComponent<Canvas>();
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
                canvasGO.layer = LayerMask.NameToLayer("UI");
                canvasGO.transform.SetParent(Selection.activeGameObject.transform);
                obj.transform.SetParent(m_canvas.transform);
                Define.Init_Canvas(m_canvas);

            }
            else
            {
                obj.transform.SetParent(Selection.activeGameObject.transform);
            }
        }
        else
        {
            Canvas m_canvas = FindObjectOfType<Canvas>();

            if (m_canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                m_canvas = canvasGO.AddComponent<Canvas>();
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
                canvasGO.layer = LayerMask.NameToLayer("UI");
                Define.Init_Canvas(m_canvas);
            }

            obj.transform.SetParent(m_canvas.transform);


        }
    }


    [MenuItem("Custom/DataSheet")]
    public static void Create_ChromeDataSheet()
    {
        string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; // 크롬 실행 파일 경로
        string urlToOpen = "https://docs.google.com/spreadsheets/d/1k5lznq26tIqMJxt_r1gz5EfXuJr-DJcTn3M0C8gP8eY/edit#gid=0"; // 열고 싶은 URL

        Process.Start(chromePath, urlToOpen);

    }
    [MenuItem("Custom/DataConverter")]
    public static void Create_ChromeDataConverter()
    {
        string chromePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"; // 크롬 실행 파일 경로
        string urlToOpen = "https://shancarter.github.io/mr-data-converter/"; // 열고 싶은 URL

        Process.Start(chromePath, urlToOpen);

    }
    [MenuItem("Custom/Binary")]
    public static void CovertToBinary()
    {
        StreamingManager.ConvertTextFilesToBinary();
    }
}

