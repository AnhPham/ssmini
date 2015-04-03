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
        public GameObject Target;
        public string OnShownFunctionName;
        public string OnHiddenFunctionName;
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

        protected void SendOnShown(Data data)
        {
            if (data != null && !string.IsNullOrEmpty(data.OnShownFunctionName))
            {
                data.Target.SendMessage(data.OnShownFunctionName);
            }
        }

        protected void SendOnHidden(Data data)
        {
            if (data != null && !string.IsNullOrEmpty(data.OnHiddenFunctionName))
            {
                data.Target.SendMessage(data.OnHiddenFunctionName);
            }
        }
    }
}
