// This code is part of the Mini SS-System library (https://github.com/AnhPham/ssmini) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/ssmini/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace SS
{
    public interface IController
    {
        string SceneName();
    }

    public class Data
    {
        public delegate void CallbackDelegate(Controller controller);
        public CallbackDelegate OnShown;
        public CallbackDelegate OnHidden;

        public Data()
        {
        }

        public Data(CallbackDelegate onShown, CallbackDelegate onHidden)
        {
            OnShown = onShown;
            OnHidden = onHidden;
        }
    }

    [RequireComponent(typeof(ControllerSupporter))]
    public abstract class Controller : MonoBehaviour, IController
    {
        [SerializeField]
        public ControllerSupporter Supporter;

        [SerializeField]
        public SceneAnimation Animation;

        public abstract string SceneName();

        public virtual void OnActive(Data data)
        {
        }

        public virtual void OnFocus(bool isFocus)
        {
        }

        public virtual void OnReFocus()
        {
        }

        public virtual void OnShown()
        {
        }

        public virtual void OnHidden()
        {
        }

        public virtual void OnKeyBack()
        {
            SceneManager.Close(true);
        }

        public virtual void OnAnySceneLoaded(Controller controller)
        {
        }

        public virtual void OnAnySceneActivated(Controller controller)
        {
        }

        public virtual void Show(bool hasAnimation)
        {
            if (Animation != null)
            {
                Animation.Show(hasAnimation);
            }
            else
            {
                SceneManager.OnShown(this);
            }
        }

        public virtual void Hide(bool hasAnimation)
        {
            if (Animation != null)
            {
                Animation.Hide(hasAnimation);
            }
            else
            {
                SceneManager.OnHidden(this);
            }
        }

        public void HideBeforeShowing()
        {
            if (Animation != null)
            {
                Animation.HideBeforeShowing();
            }
        }

        public SceneData SceneData
        {
            get;
            set;
        }
    }
}
