// This code is part of the Mini SS-System library (https://github.com/AnhPham/ssmini) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/ssmini/blob/master/LICENSE)

using UnityEngine;
using System.Collections;
using SS;

public class DemoTabController : Controller
{
    bool m_DemoClear;
    string m_Prefix;

    void SetPrefix()
    {
        m_Prefix = (m_DemoClear) ? "_Clear" : string.Empty;
    }

    public override string SceneName()
    {
        return "DemoTab";
    }

    public bool isDemoClear
    {
        get { return m_DemoClear; }
        set { m_DemoClear = value; SetPrefix(); }
    }

    public void OnButton1Tap()
    {
        SceneManager.Scene("DemoGame1" + m_Prefix, m_DemoClear);
    }

    public void OnButton2Tap()
    {
        SceneManager.Scene("DemoGame2" + m_Prefix, m_DemoClear);
    }

    public void OnButton3Tap()
    {
        SceneManager.Scene("DemoGame3" + m_Prefix, m_DemoClear);
    }

    public void OnButton4Tap()
    {
        SceneManager.Scene("DemoGame4" + m_Prefix, m_DemoClear);
    }
}