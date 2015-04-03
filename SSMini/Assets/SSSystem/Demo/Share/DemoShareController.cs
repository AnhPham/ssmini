using UnityEngine;
using System.Collections;
using SS;

public class DemoShareController : Controller
{
    public override string SceneName()
    {
        return "DemoShare";
    }

    public void OnCloseButtonTap()
    {
        SceneManager.Close(true);
    }

    public void OnShareButtonTap()
    {
        SceneManager.Popup("DemoPopup", true);
    }
}