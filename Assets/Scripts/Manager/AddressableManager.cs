using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
public class AddressableManager : MonoBehaviour
{
    static Dictionary<string, Object> D_Obj = new Dictionary<string, Object>();

    public static float Per; //Current success percent
    public static int ObjCount; //������Ʈ ��ü ����
    public static int LoadedCount; //�ε� �Ϸ�� ������Ʈ ����

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
