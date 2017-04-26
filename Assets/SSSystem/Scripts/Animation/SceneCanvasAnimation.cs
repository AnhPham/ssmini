// This code is part of the Mini SS-System library (https://github.com/AnhPham/ssmini) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/ssmini/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace SS
{
    public class SceneCanvasAnimation : SceneAnimation
    {
        #region Enum
        enum State
        {
            IDLE,
            SHOW,
            HIDE
        }

        enum AnimationDirection
        {
            FROM_BOTTOM,
            FROM_TOP,
            FROM_RIGHT
        }
        #endregion

        #region SerializeField
        [SerializeField]
        AnimationDirection m_AnimationDirection = AnimationDirection.FROM_RIGHT;
        #endregion

        #region Private Variable
        float m_AnimationTime;
        Vector2 m_Start;
        Vector2 m_End;
        RectTransform m_RectTransform;
        RectTransform m_CanvasRectTransform;
        State m_State = State.IDLE;
        float m_StartTime;
        #endregion

        void Awake()
        {
            if (Application.isPlaying)
            {
                if (SceneManager.SceneAnimationTime > 0)
                {
                    m_AnimationTime = SceneManager.SceneAnimationTime;
                }
                else
                {
                    m_AnimationTime = 0.283f;
                }
            }
        }

        RectTransform RectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = GetComponent<RectTransform>();
                }

                return m_RectTransform;
            }
        }

        RectTransform CanvasRectTransform
        {
            get
            {
                if (m_CanvasRectTransform == null)
                {
                    Transform p = transform.parent;
                    while (p != null)
                    {
                        if (p.GetComponent<Canvas>() != null)
                        {
                            m_CanvasRectTransform = p.GetComponent<RectTransform>();
                            break;
                        }
                        p = p.parent;
                    }
                }

                return m_CanvasRectTransform;
            }
        }

        public override void HideBeforeShowing()
        {
            RectTransform.anchoredPosition = new Vector2(10000, 0);
        }

        public override void Show(bool hasAnimation)
        {
            switch (m_AnimationDirection)
            {
                case AnimationDirection.FROM_BOTTOM:
                    m_Start = new Vector2(0, -ScreenHeight());
                    m_End = Vector2.zero;
                    break;
                case AnimationDirection.FROM_TOP:
                    m_Start = new Vector2(0, ScreenHeight());
                    m_End = Vector2.zero;
                    break;
                case AnimationDirection.FROM_RIGHT:
                    m_Start = new Vector2(ScreenWidth(), 0);
                    m_End = Vector2.zero;
                    break;
            }

            if (hasAnimation)
            {
                m_State = State.SHOW;
                m_StartTime = Time.realtimeSinceStartup;
            }
            else
            {
                RectTransform.anchoredPosition = m_End;
                OnShown();
            }
        }

        public override void Hide(bool hasAnimation)
        {
            switch (m_AnimationDirection)
            {
                case AnimationDirection.FROM_BOTTOM:
                    m_Start = Vector2.zero;
                    m_End = new Vector2(0, -ScreenHeight());
                    break;
                case AnimationDirection.FROM_TOP:
                    m_Start = Vector2.zero;
                    m_End = new Vector2(0, ScreenHeight());
                    break;
                case AnimationDirection.FROM_RIGHT:
                    m_Start = Vector2.zero;
                    m_End = new Vector2(ScreenWidth(), 0);
                    break;
            }

            if (hasAnimation)
            {
                m_State = State.HIDE;
                m_StartTime = Time.realtimeSinceStartup;
            }
            else
            {
                RectTransform.anchoredPosition = m_End;
                OnHidden();
            }
        }

        void UpdateByTime()
        {
            if (Application.isPlaying)
            {
                switch (m_State)
                {
                    case State.SHOW:
                    case State.HIDE:
                        float time = Time.realtimeSinceStartup - m_StartTime;
                        RectTransform.anchoredPosition = Vector2.Lerp(m_Start, m_End, time / m_AnimationTime);

                        if (time >= m_AnimationTime)
                        {
                            OnEndAnimation();
                            m_State = State.IDLE;
                        }
                        break;
                }
            }
        }

        void OnEndAnimation()
        {
            switch (m_State)
            {
                case State.SHOW:
                    OnShown();
                    break;
                case State.HIDE:
                    OnHidden();
                    break;
            }
        }

        float ScreenHeight()
        {
            if (CanvasRectTransform != null)
            {
                return CanvasRectTransform.sizeDelta.y;
            }
            return Screen.height;
        }

        float ScreenWidth()
        {
            if (CanvasRectTransform != null)
            {
                return CanvasRectTransform.sizeDelta.x;
            }
            return Screen.width;
        }

        #if UNITY_EDITOR
        protected override void Update()
        {
            base.Update();
            UpdateByTime();
        }
        #else
        void Update()
        {
            UpdateByTime();
        }
        #endif
    }
}