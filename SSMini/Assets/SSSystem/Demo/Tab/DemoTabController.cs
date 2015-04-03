using UnityEngine;
using System.Collections;
using SS;

public class DemoTabController : Controller
{
    public override string SceneName()
    {
        return "DemoTab";
    }

    public void OnButton1Tap()
    {
        SceneManager.Scene("DemoGame1");
    }

    public void OnButton2Tap()
    {
        SceneManager.Scene("DemoGame2");
    }

    public void OnButton3Tap()
    {
        SceneManager.Scene("DemoGame3");
    }

    public void OnButton4Tap()
    {
        SceneManager.Scene("DemoGame4");
    }
}