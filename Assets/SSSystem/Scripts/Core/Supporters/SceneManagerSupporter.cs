using UnityEngine;
using System.Collections;

namespace SS
{
    public class SceneManagerSupporter : MonoBehaviour
    {
        void Awake()
        {
            gameObject.AddComponent<AudioListener>();
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            #if UNITY_EDITOR || UNITY_ANDROID
            UpdateInput();
            #endif
        }

        void UpdateInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!SceneManager.ActiveShield())
                {
                    Controller controller = SceneManager.Top();
                    if (controller != null)
                    {
                        controller.OnKeyBack();
                    }
                }
            }
        }
    }
}
