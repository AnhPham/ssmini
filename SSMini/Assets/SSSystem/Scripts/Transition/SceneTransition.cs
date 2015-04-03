using UnityEngine;
using System.Collections;

namespace SS
{
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField]
        Animation m_ShieldAnimation;

        [SerializeField]
        string m_FadeInAnimName = "FadeIn";

        [SerializeField]
        string m_FadeOutAnimName = "FadeOut";

        string m_NextSceneName;
        bool m_Active;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            FadeInScene();
        }

		public void ShieldOff()
		{
			Active = false;
		}

		public void ShieldOn()
		{
			Active = true;

			m_ShieldAnimation[m_FadeOutAnimName].time = 0;
			m_ShieldAnimation.Play(m_FadeOutAnimName);
			m_ShieldAnimation.Sample();
			m_ShieldAnimation.Stop();
		}

        // Scene gradually appear
        public void FadeInScene()
        {
            if (this != null)
            {
                Active = true;
                m_ShieldAnimation.Play(m_FadeInAnimName);
            }
        }

        // Scene gradually disappear
        void FadeOutScene()
        {
            if (this != null)
            {
                Active = true;
                m_ShieldAnimation.Play(m_FadeOutAnimName);
            }
        }

        public void LoadScene(string sceneName)
        {
            m_NextSceneName = sceneName;
            FadeOutScene();
        }

        public void OnFadedIn()
        {
            if (this != null)
            {
                Active = false;
            }
        }

        public void OnFadedOut()
        {
            Application.LoadLevel(m_NextSceneName);
        }

        public bool Active
        {
            get
            {
                return m_Active;
            }
            protected set
            {
                m_Active = value;
                gameObject.SetActive(m_Active);
            }
        }
    }
}