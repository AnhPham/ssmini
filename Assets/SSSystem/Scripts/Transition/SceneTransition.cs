using UnityEngine;
using System.Collections;

namespace SS
{
    public class SceneTransition : MonoBehaviour
    {
        enum State
        {
            SHIELD_OFF,
            SHIELD_ON,
            SHIELD_FADE_IN,
            SHIELD_FADE_OUT,
            SCENE_LOADING
        }

        [SerializeField]
        Animation m_ShieldAnimation;

        [SerializeField]
        string m_FadeInAnimName = "FadeIn";

        [SerializeField]
        string m_FadeOutAnimName = "FadeOut";

        string m_NextSceneName;
        bool m_ClearAll;
        bool m_Active;

        State m_State;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

		public void ShieldOff()
		{
            if (m_State == State.SHIELD_ON)
            {
                m_State = State.SHIELD_OFF;
                Active = false;
            }
		}

		public void ShieldOn()
		{
            if (m_State == State.SHIELD_OFF)
            {
                m_State = State.SHIELD_ON;
                Active = true;

                m_ShieldAnimation[m_FadeOutAnimName].time = 0;
                m_ShieldAnimation.Play(m_FadeOutAnimName);
                m_ShieldAnimation.Sample();
                m_ShieldAnimation.Stop();
            }
		}

        // Scene gradually appear
        public void FadeInScene()
        {
            if (this != null)
            {
                if (SceneManager.SceneFadeTime == 0)
                {
                    ShieldOff();
                }
                else
                {
                    Active = true;
                    m_ShieldAnimation[m_FadeInAnimName].speed = 1f / SceneManager.SceneFadeTime;
                    m_ShieldAnimation.Play(m_FadeInAnimName);
                    m_ShieldAnimation[m_FadeInAnimName].time = 0;
                    m_ShieldAnimation.Sample();

                    m_State = State.SHIELD_FADE_IN;
                }
            }
        }

        // Scene gradually disappear
        void FadeOutScene()
        {
            if (this != null)
            {
                if (SceneManager.SceneFadeTime == 0)
                {
                    ShieldOn();
                    OnFadedOut();
                }
                else
                {
                    Active = true;
                    m_ShieldAnimation[m_FadeOutAnimName].speed = 1f / SceneManager.SceneFadeTime;
                    m_ShieldAnimation.Play(m_FadeOutAnimName);

                    m_State = State.SHIELD_FADE_OUT;
                }
            }
        }

        public void LoadScene(string sceneName, bool clearAll)
        {
            m_NextSceneName = sceneName;
            m_ClearAll = clearAll;

            FadeOutScene();
        }

        public void OnFadedIn()
        {
            if (this != null)
            {
                m_State = State.SHIELD_OFF;
                Active = false;
            }
        }

        public void OnFadedOut()
        {
            m_State = State.SCENE_LOADING;
            SceneManager.LoadLevel(m_NextSceneName, m_ClearAll);
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