using UnityEngine;
using System.Collections;
using SS;

public class DemoGameController : Controller
{
    public override string SceneName()
    {
        return "DemoGame";
    }

    public void OnAttackButtonTap()
    {
        Debug.Log("Attack");
    }

    public void OnSaveButtonTap()
    {
        SceneManager.Popup("DemoSave", true);
    }
}