using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Reflection;
#if UNITY_ANDROID
using UnityEngine.Networking;
#endif

using static Data_User;

public class Data : SingleMono<Data>
{
    public static event EventHandler Ev_UserData_Change;
    bool HaveToSave_UserData;
    public void UserData_Changed()
    {
        if (HaveToSave_UserData == false)
        {
            HaveToSave_UserData = true;
        }
    }
    private void LateUpdate()
    {
        if (HaveToSave_UserData)
        {
            //ud바뀌면 갱신되어야하는 값들 갱신하기위한 이벤트
            Ev_UserData_Change?.Invoke(this, EventArgs.Empty);
            HaveToSave_UserData = false;
            //ud저장

            UD_Func.UserData_Save();
        }   
    }

    #region Streaming
    List<E_DataRead_State> L_DataAsync = new List<E_DataRead_State>();
    List<string> L_TypeName = new List<string>();
    List<Task> L_Task = new List<Task>();
    public float Get_StreamingDataRead_Percent()
    {
        if(L_DataAsync==null || L_DataAsync.Count == 0)
        {
            return 0;
        }
        int success_count = 0;
        for(int i = 0; i < L_DataAsync.Count; i++)
        {
            if (L_DataAsync[i] == E_DataRead_State.Success)
            {
                success_count++;
            }
        }
        return (float)success_count / L_DataAsync.Count * 100;
    }
    public async void Read_StreamingData_Async(Action Ac_Success = null)
    {
        L_DataAsync.Clear();
        L_TypeName.Clear();

        int idx = 0;

        string _FileName = "FileName";
        string _isNone_Instance = "isNone_Instance";
        string _ReadData_Async = "ReadData_Async";

        Type Type_SingleData = typeof(Single_Data<>);
        Type[] types = Get_StreamingDataType();


        for (int i = 0; i < types.Length; i++)
        {
            //Get FileName
            string Name = (string)types[i].GetField(_FileName, BindingFlags.Static | BindingFlags.Public).GetValue(null);

            //Get type_Parent (Single_Data<T>)
            Type Type_type_Parent = Type_SingleData.MakeGenericType(types[i]);

            //Check isNone_Instance
            MethodInfo method = Type_type_Parent.GetMethod(_isNone_Instance, BindingFlags.Static | BindingFlags.Public);
            bool _isNone_Instance_Result = (bool)method.Invoke(null, null);
            if (_isNone_Instance_Result)
            {
                int _idx = idx;
                idx++;
                L_TypeName.Add(types[i].Name);
                L_DataAsync.Add(E_DataRead_State.None);

                //Call ReadData_Async
                MethodInfo Method_ReadData_Async = typeof(Data_Reader).GetMethod(_ReadData_Async, BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(types[i]);
                Action Ac_ReadData_Success = new Action(() =>
                {
                    L_DataAsync[_idx] = E_DataRead_State.Success;
                });
                L_Task.Add(new Task(() => { Method_ReadData_Async.Invoke(null, new object[] { Name, null, Ac_ReadData_Success }); }));
            }
        }

        for (int i = 0; i < L_Task.Count; i++)
        {
            L_Task[i].Start();
        }

        await Task.WhenAll(L_Task);
        L_Task.Clear();

        Ac_Success?.Invoke();

        //Log
        for(int i = 0; i < L_DataAsync.Count; i++)
        {
            LogManager.Log(L_TypeName[i] + " " + L_DataAsync[i]);
        }
        LogManager.Log("Read_StreamingData_Async Complete");
    }


    public static Type[] Get_StreamingDataType()
    {
        Type Type_SingleData = typeof(Single_Data<>);
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        List<Type> L_Type = new List<Type>();
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].IsClass == true && types[i].BaseType != null && types[i].BaseType.IsGenericType == true && types[i].BaseType.GetGenericTypeDefinition() == Type_SingleData)
            {
                L_Type.Add(types[i]);
            }
        }

        return L_Type.ToArray();
    }

    #endregion
    #region Addressable
    #endregion
}




#region Quest
public class Data_Quest : Single_Data<Data_Quest>
{

    readonly public static string FileName = "Quest";
    readonly public static string URL = "";

    public int[] M_ID;
    public bool[] Release;
    public E_QuestType[] Q_Type;
    public int[] Mission_Kind;
    public int[] Mission_Value;
    public E_ItemKind[] Reward_Kind_0, Reward_Kind_1;
    public int[] Reward_ID_0, Reward_ID_1;
    public int[] Reward_Value_0, Reward_Value_1;

    Dictionary<int, D_Quest> D_Quest;
    Dictionary<E_QuestType, List<D_Quest>> D_QuestType;
    public override void Setting()
    {
        if (D_Quest == null)
        {
            D_Quest = new Dictionary<int, D_Quest>();
            D_QuestType = new Dictionary<E_QuestType, List<D_Quest>>();
            foreach(E_QuestType value in Enum.GetValues(typeof(E_QuestType)))
            {
                D_QuestType.Add(value, new List<D_Quest>());
            }
        }
        for (int i = 0; i < M_ID.Length; i++)
        {
            D_Quest Cur_Quest = new D_Quest(

                M_ID[i],
                Release[i],
                Q_Type[i],
                Mission_Kind[i],
                Mission_Value[i],
                new E_ItemKind[2] { Reward_Kind_0[i], Reward_Kind_1[i] },
                new int[2] { Reward_ID_0[i], Reward_ID_1[i] },
                new int[2] { Reward_Value_0[i], Reward_Value_1[i] }

                );
            D_Quest.Add(M_ID[i], Cur_Quest);
            D_QuestType[Q_Type[i]].Add(Cur_Quest);
        }

    }
    public D_Quest Get_Quest(int quest_id)
    {
        return D_Quest[quest_id];
    }
    public List<D_Quest> Get_Quest_Type(E_QuestType type)
    {
        return D_QuestType[type];
    }
}
public class D_Quest
{
    public int M_ID;
    public bool Release;
    public E_QuestType Q_Type;
    public int Mission_Kind;
    public int Mission_Value;
    public E_ItemKind[] Reward_Kind;
    public int[] Reward_ID;
    public int[] Reward_Value;

    public D_Quest(int iD, bool release, E_QuestType q_Type, int mission_Kind, int mission_Value, E_ItemKind[] reward_Kind, int[] reward_ID, int[] reward_Value)
    {
        M_ID = iD;
        Release = release;
        Q_Type = q_Type;
        Mission_Kind = mission_Kind;
        Mission_Value = mission_Value;
        Reward_Kind = reward_Kind;
        Reward_ID = reward_ID;
        Reward_Value = reward_Value;
    }
}
#endregion
#region Product
public class Data_Product : Single_Data<Data_Product>
{
    readonly public static string FileName = "Product";
    readonly public static string URL = "";

    public int[] M_ID;
    public bool[] Release;
    public E_ProductType[] Product_Type;
    public E_ItemKind[] Price_Kind_0, Price_Kind_1, Price_Kind_2;
    public int[] Price_ID_0, Price_ID_1, Price_ID_2;
    public int[] Price_Value_0, Price_Value_1, Price_Value_2;
    public E_ItemKind[] Product_Kind_0, Product_Kind_1, Product_Kind_2;
    public int[] Product_ID_0, Product_ID_1, Product_ID_2;
    public int[] Product_Value_0, Product_Value_1, Product_Value_2;

    Dictionary<int, D_Product> D_Product;
    Dictionary<E_ProductType, List<D_Product>> D_ProductType;
    public override void Setting()
    {
        if (D_Product == null)
        {
            D_Product = new Dictionary<int, D_Product>();
            D_ProductType = new Dictionary<E_ProductType, List<D_Product>>();
            foreach (E_ProductType value in Enum.GetValues(typeof(E_ProductType)))
            {
                D_ProductType.Add(value, new List<D_Product>());
            }
        }
        for (int i = 0; i < M_ID.Length; i++)
        {
            D_Product Cur_Product = new D_Product(

                M_ID[i],
                Release[i],
                Product_Type[i],
                new E_ItemKind[3] { Price_Kind_0[i], Price_Kind_1[i], Price_Kind_2[i] },
                new int[3] { Price_ID_0[i], Price_ID_1[i], Price_ID_2[i]},
                new int[3] { Price_Value_0[i], Price_Value_1[i], Price_Value_2[i] },

                new E_ItemKind[3] { Product_Kind_0[i], Product_Kind_1[i], Product_Kind_2[i] },
                new int[3] { Product_ID_0[i], Product_ID_1[i], Product_ID_2[i] },
                new int[3] { Product_Value_0[i], Product_Value_1[i], Product_Value_2[i] }

                );
            D_Product.Add(M_ID[i], Cur_Product);
            D_ProductType[Product_Type[i]].Add(Cur_Product);
        }
    }
    public D_Product Get_Product(int product_id)
    {
        return D_Product[product_id];
    }
    public List<D_Product> Get_Product_Type(E_ProductType type)
    {
        return D_ProductType[type];
    }
}
public class D_Product
{
    public int M_ID;
    public bool Release;
    public E_ProductType Product_Type;
    public E_ItemKind[] Price_Kind;
    public int[] Price_ID;
    public int[] Price_Value;
    public E_ItemKind[] Product_Kind;
    public int[] Product_ID;
    public int[] Product_Value;

    public D_Product(int iD, bool release, E_ProductType product_Type, E_ItemKind[] price_Kind, int[] price_ID, int[] price_Value, E_ItemKind[] product_Kind, int[] product_ID, int[] product_Value)
    {
        M_ID = iD;
        Release = release;
        Product_Type = product_Type;
        Price_Kind = price_Kind;
        Price_ID = price_ID;
        Price_Value = price_Value;
        Product_Kind = product_Kind;
        Product_ID = product_ID;
        Product_Value = product_Value;
    }
}
#endregion
#region Item
public class Data_Item : Single_Data<Data_Item>
{
    readonly public static string FileName = "Item";
    readonly public static string URL = "";

    public E_ItemKind[] Item_Kind;
    public int[] Item_ID;
    public int[] Image;
    public E_ItemKind[] Sell_Item_Kind_0;
    public int[] Sell_Item_ID_0;
    public int[] Sell_Item_Value_0;

    Dictionary<(E_ItemKind, int), D_Item> D_Item;
    public override void Setting()
    {
        if (D_Item == null)
        {
            D_Item = new Dictionary<(E_ItemKind, int), D_Item>();
        }
        for(int i = 0; i < Item_ID.Length; i++) 
        {
            D_Item Cur_Item = new D_Item(
                Item_Kind[i],
                Item_ID[i],
                Image[i],
                new E_ItemKind[1] {Sell_Item_Kind_0[i]},
                new int[1] {Sell_Item_ID_0[i]},
                new int[1] {Sell_Item_Value_0[i]}
                );

            D_Item.Add((Item_Kind[i], Item_ID[i]), Cur_Item);
        }
    }
    public D_Item Get_Item(E_ItemKind item_kind, int item_id)
    {
        return D_Item[(item_kind, item_id)];
    }
}
public class D_Item
{
    public E_ItemKind Item_Kind;
    public int Item_ID;
    public int Image;
    public E_ItemKind[] Sell_Item_Kind;
    public int[] Sell_Item_ID;
    public int[] Sell_Item_Value;

    public D_Item(E_ItemKind item_Kind, int item_ID, int image, E_ItemKind[] sell_Item_Kind, int[] sell_Item_ID, int[] sell_Item_Value)
    {
        Item_Kind = item_Kind;
        Item_ID = item_ID;
        Image = image;
        Sell_Item_Kind = sell_Item_Kind;
        Sell_Item_ID = sell_Item_ID;
        Sell_Item_Value = sell_Item_Value;
    }
}
#endregion
#region Language
public class Data_Language : Single_Data<Data_Language>
{
    readonly public static string FileName = "Item";
    readonly public static string URL = "";

    public int[] M_ID;
    public string[] Kor;
    public string[] Eng;

    Dictionary<int, string> D_Cur_Lang;
    Dictionary<int, string> D_Kor;
    Dictionary<int, string> D_Eng;
    public override void Setting()
    {
        if (D_Kor == null)
        {
            D_Kor = new Dictionary<int, string>();
            D_Eng = new Dictionary<int, string>();
            for (int i = 0; i < M_ID.Length; i++)
            {
                D_Kor.Add(M_ID[i], Kor[i]);
                D_Eng.Add(M_ID[i], Eng[i]);
            }

            //해당 이벤트 등록은 치트버전에선 데이터 새로 받아오는 경우때문에 해제해줄 필요가 있으나
            //정식버전에선 해제해줄 필요가 없다
            GameManager.Ev_Language_Change += Change_Language_Kind;
        }

        if (UserData == null)
        {
            Change_Language_Kind(null, Application.systemLanguage.Get_LanguageKind());
        }
        else
        {
            Change_Language_Kind(null, UserData.Language_Kind);
        }
    }
    void Change_Language_Kind(object sender, E_Language kind)
    {
        switch (kind)
        {
            case E_Language.Kor:
                D_Cur_Lang = D_Kor;
                break;
            case E_Language.Eng:
                D_Cur_Lang = D_Eng;
                break;
        }
    }
    public string Get_Lang(int lang_id)
    {
        return D_Cur_Lang[lang_id];
    }
}

#endregion
#region Image
public class Data_Image : Single_Data<Data_Image>
{
    readonly public static string FileName = "Image";
    readonly public static string URL = "";

    public int[] M_ID;
    public string[] Path;

    Dictionary<int, Sprite> D_Image;

    public override void Setting()
    {
        if (D_Image == null)
        {
            D_Image = new Dictionary<int, Sprite>();
        }
        for (int i = 0; i < M_ID.Length; i++)
        {
            //어드레서블 사용
        }
    }
    public Sprite Get_Image(int image_id)
    {
        return D_Image[image_id];
    }
}
public class D_Image
{
    public int M_ID;
    public string Path;

    public D_Image(int iD, string path)
    {
        M_ID = iD;
        Path = path;
    }
}
#endregion
#region Audio
public class Data_Audio : Single_Data<Data_Audio>
{
    readonly public static string FileName = "Audio";
    readonly public static string URL = "";

    public int[] M_ID;
    public string[] Path;

    Dictionary<int, AudioClip> D_Audio;

    public override void Setting()
    {
        if(D_Audio == null)
        {
            D_Audio = new Dictionary<int, AudioClip>();
        }
        for (int i = 0; i < M_ID.Length; i++)
        {
            //어드레서블 사용
        }
    }
    public AudioClip Get_Audio(int audio_id)
    {
        return D_Audio[audio_id];
    }
}
public class D_Audio
{
    public int M_ID;
    public string Path;

}
#endregion

#region Data_Reader
public class Data_Reader
{
    public static async Task<T> ReadData_Async<T>(string filename, string filepath, Action Ac_Success)
    {
        if (filepath == null)
        {
            filepath = Application.streamingAssetsPath + "/Text/";
        }
        string path = filepath + filename + ".txt";
        string data = await File.ReadAllTextAsync(path);
        T class_= JsonUtility.FromJson<T>(data); 
        Ac_Success?.Invoke();

        return class_;
    }
    public static T ReadData<T>(string filename, string filepath = null)
    {

#if UNITY_EDITOR
        if (filepath == null)
        {
            filepath = Application.streamingAssetsPath + "/Text/";
        }
        string path = filepath + filename + ".txt";
        return JsonUtility.FromJson<T>(File.ReadAllText(path));
#elif UNITY_STANDALONE_WIN
        if (filepath == null)
        {
            filepath = Application.streamingAssetsPath +"/Binary/";
        }
        string path = filepath + filename + ".bin";
        WWW reader = new WWW(path);
        while (!reader.isDone) { }
        MemoryStream ms = new MemoryStream(reader.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        return (T)bf.Deserialize(ms);
#elif UNITY_ANDROID
        if (filepath == null)
        {
            filepath = Application.streamingAssetsPath + "/Binary/";
        }
        string path = filepath + filename + ".bin";
        UnityWebRequest webrequest = UnityWebRequest.Get(path);
        webrequest.SendWebRequest();
        while (!webrequest.isDone)
        {

        }
        MemoryStream ms = new MemoryStream(webrequest.downloadHandler.data);
        BinaryFormatter bf = new BinaryFormatter();
        return (T)bf.Deserialize(ms);
#elif UNITY_IOS
        if (filepath == null)
        {
            filepath = Application.streamingAssetsPath + "/Binary/";
        }
        string path = filepath + filename + ".bin";
        MemoryStream ms = new MemoryStream(File.ReadAllBytes(path));
        BinaryFormatter bf = new BinaryFormatter();
        return (T)bf.Deserialize(ms);
#endif


    }

}

#endregion
