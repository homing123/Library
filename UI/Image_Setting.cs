using UnityEngine;
using UnityEngine.UI;

public class Image_Setting : Image
{
    public void Change_Sprite(Sprite image)
    {
        sprite = image;
    }
    public void Change_Sprite(int image_id)
    {
        sprite = Data_Image.Instance.Get_Image(image_id);
    }
    public void Set_AutoSize()
    {

    }
}
