// This code is part of the SS-Scene library, released by Anh Pham (anhpt.csit@gmail.com).

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SS
{
    public class UnmanagedCanvas : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            GetComponent<Canvas>().worldCamera = SceneManager.CameraUI;
            GetComponent<CanvasScaler>().matchWidthOrHeight = SS.Utils.Screen.GetCanvasScalerMatch();
        }
    }
}
