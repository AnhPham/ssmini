using UnityEngine;
using System.Collections;

namespace SS
{
    public class SceneLegacyAnimation : SceneAnimation
    {
        [SerializeField]
        protected string m_ShowAnimName = "Show";

        [SerializeField]
        protected string m_HideAnimName = "Hide";

        Animation m_Animation;
        Animation Animation
        {
            get
            {
                if (m_Animation == null)
                {
                    m_Animation = GetComponent<Animation>();
                }
                return m_Animation;
            }
        }

        public override void HideBeforeShowing()
        {
            Animation.Play(m_ShowAnimName);
            Animation[m_ShowAnimName].time = 0;
            Animation.Sample();
            Animation.Stop();
        }

        public override void Show(bool hasAnimation)
        {
            Animation.Play(m_ShowAnimName);

            if (!hasAnimation)
            {
                Animation[m_ShowAnimName].time = Animation[m_ShowAnimName].length;
                Animation.Sample();
            }
        }

        public override void Hide(bool hasAnimation)
        {
            Animation.Play(m_HideAnimName);

            if (!hasAnimation)
            {
                Animation[m_HideAnimName].time = Animation[m_HideAnimName].length;
                Animation.Sample();
            }
        }
    }
}

