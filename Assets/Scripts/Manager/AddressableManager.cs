using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
public class AddressableManager : MonoBehaviour
{
    static Dictionary<string, Object> D_Obj = new Dictionary<string, Object>();

    public static float Per; //Current success percent
    public static int ObjCount; //오브젝트 전체 갯수
    public static int LoadedCount; //로드 완료된 오브젝트 갯수

    public static async Task Load()
    {
        
    }

    //public static GameObject Get(string key)
    //{
    //    if (D_Obj.ContainsKey(key))
    //    {

    //    }
    //}
    //public static async Task<GameObject> Get_Async(string key)
    //{
        
    //}
}
