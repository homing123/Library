using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneHandler : MonoBehaviour
{
    public const string LoadScene = "Sc_Load";
    public const string MainScene = "Sc_Main";
    public const string PlayScene = "Sc_Play";

    public static void SceneLoad(string scene_name)
    {
        SceneManager.LoadScene(scene_name);
    }
}
