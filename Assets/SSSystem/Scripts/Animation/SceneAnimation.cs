using UnityEngine;
using System.Collections;

namespace SS
{
    [ExecuteInEditMode]
    public class SceneAnimation : MonoBehaviour
    {
        [SerializeField]
        Controller target;

        public virtual void HideBeforeShowing()
        {
        }

        public virtual void Show(bool hasAnimation)
        {
            OnShown();
        }

        public virtual void Hide(bool hasAnimation)
        {
            OnHidden();
        }

        public void OnShown()
        {
            SceneManager.OnShown(target);
        }

        public void OnHidden()
        {
            SceneManager.OnHidden(target);
        }

        #if UNITY_EDITOR
        protected virtual void Update()
        {
            AutoFind();
        }

        void AutoFind()
        {
            if (!Application.isPlaying)
            {
                if (target == null)
                {
                    target = FindObjectOfType<Controller>();

                    if (target != null)
                    {
                        target.Animation = this;
                    }
                }
            }
        }
        #endif
    }
}

