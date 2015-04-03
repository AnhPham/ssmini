using UnityEngine;
using System.Collections;
using SS;

public class DemoSaveController : Controller
{
    public override string SceneName()
    {
        return "DemoSave";
    }

    public void OnCloseButtonTap()
    {
        SceneManager.Close(true);
    }

    public void OnShareButtonTap()
    {
        SceneManager.Popup("DemoShare", true);
    }
}