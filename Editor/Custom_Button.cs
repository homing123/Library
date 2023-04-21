using UnityEditor;
using UnityEngine;
using System.IO;
public class Custom_Button : MonoBehaviour
{
    [MenuItem("Custom/UD_Delete")]
    public static void UD_Delete()
    {
        UD_Func.UserData_Delete();
        Debug.Log("UD_Delete Success");
    }
}
