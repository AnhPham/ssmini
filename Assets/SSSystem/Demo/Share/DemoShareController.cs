using UnityEngine;
using System.Collections;
using SS;
using UnityEngine.UI;

public class DemoShareController : Controller
{
    [SerializeField]
    ScrollRect m_ScrollRect;

    Data m_PopupData;

    void Awake()
    {
        m_PopupData = new Data(null, OnPopupHidden);
    }

    public override string SceneName()
    {
        return "DemoShare";
    }

    public override void OnActive(Data data)
    {
        ResetScrollRectPosition();
    }

    public void OnCloseButtonTap()
    {
        SceneManager.Close(true);
    }

    public void OnShareButtonTap()
    {
        SceneManager.Popup("DemoPopup", true, m_PopupData);
    }

    void ResetScrollRectPosition()
    {
        m_ScrollRect.StopMovement();
        m_ScrollRect.content.anchoredPosition = Vector2.zero;
    }

    void OnPopupHidden(Controller controller)
    {
        SceneManager.BackToScene(true);
    }
}