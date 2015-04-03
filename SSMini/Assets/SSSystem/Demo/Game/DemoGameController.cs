using UnityEngine;
using System.Collections;
using SS;

public class DemoGameData : Data
{
    public Texture2D ScreenCapture;
}

public class DemoGameController : Controller
{
    [SerializeField]
    GameObject m_BulletPrefab;

    [SerializeField]
    Transform m_Target;

    DemoGameData data = new DemoGameData();

    void Awake()
    {
        SceneManager.ShieldColor = new Color(0, 0, 0, 0.8f);
        SceneManager.SceneFadeTime = 0.25f;
        SceneManager.LoadingSceneName = "DemoLoading";
        SceneManager.TabSceneName = "DemoTab";
        SceneManager.Tab(true);
    }

    void OnDestroy()
    {
        SetDataTexture(null);
    }

    public override string SceneName()
    {
        return gameObject.name;
    }

    public void OnAttackButtonTap()
    {
        // Instantiate bullet
        GameObject g = Instantiate(m_BulletPrefab) as GameObject;

        // Set its position same camera position
        g.SetActive(true);
        g.transform.position = Camera.main.transform.position;

        // Add force to bullet
        Rigidbody rb = g.GetComponent<Rigidbody>();
        rb.AddForce((m_Target.position - g.transform.position) * 80);
    }

    public void OnSaveButtonTap()
    {
        StartCoroutine(Capture());
    }

    IEnumerator Capture()
    {
        // Capture screen
        yield return new WaitForEndOfFrame();
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // Set data
        SetDataTexture(texture);

        // Simulate 1 second loading
        SceneManager.Loading(true);
        yield return new WaitForSeconds(1);
        SceneManager.Loading(false);

        // Load save view
        SceneManager.FullScreenView("DemoSave", true, data);
    }

    void SetDataTexture(Texture2D texture)
    {
        if (data.ScreenCapture != null)
        {
            Destroy(data.ScreenCapture);
        }
        data.ScreenCapture = texture;
    }
}