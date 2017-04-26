// This code is part of the Mini SS-System library (https://github.com/AnhPham/ssmini) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/ssmini/blob/master/LICENSE)

using UnityEngine;
using System.Collections;
using SS;
using UnityEngine.UI;

public class DemoPreviewController : Controller
{
    [SerializeField]
    Image m_PhotoImage;

    DemoGameData m_GameData;

    public override string SceneName()
    {
        return "DemoPreview";
    }

    public override void OnActive(Data data)
    {
        if (data != null)
        {
            m_PhotoImage.GetComponent<AspectRatioFitter>().aspectRatio = (float)Screen.width / Screen.height;

            m_GameData = (DemoGameData)data;
            Texture2D texture = m_GameData.ScreenCapture;

            m_PhotoImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }

    public void OnCloseButtonTap()
    {
        SceneManager.Close(true);
    }

    public void OnShareButtonTap()
    {
        SceneManager.View("DemoShare", true, true);
    }
}