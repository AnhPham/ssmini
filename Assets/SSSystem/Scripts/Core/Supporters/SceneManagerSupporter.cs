// This code is part of the Mini SS-System library (https://github.com/AnhPham/ssmini) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/ssmini/blob/master/LICENSE)

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
