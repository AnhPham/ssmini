using UnityEngine;
using System.Collections;
using SS;

public class DemoPopupController : Controller
{
    public override string SceneName()
    {
        return "DemoPopup";
    }

    public void OnCloseButtonTap()
    {
        SceneManager.Close(true);
    }
}