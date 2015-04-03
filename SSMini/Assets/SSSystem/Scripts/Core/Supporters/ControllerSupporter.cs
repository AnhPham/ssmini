using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SS
{
    [ExecuteInEditMode]
    public class ControllerSupporter : MonoBehaviour
    {
        const int WAIT_ANIM_FRAME = 4;

        enum State
        {
            IDLE,
            SHOW,
            HIDE
        }

        [SerializeField]
        Controller m_Controller;

        [SerializeField]
        Canvas[] m_CanvasArray;

        int m_FrameCounter;
        State m_State;

        public void Show(bool hasAnimation)
        {
            if (hasAnimation)
            {
                m_Controller.HideBeforeShowing();
                m_State = State.SHOW;
                m_FrameCounter = 0;
            }
            else
            {
                m_Controller.Show(false);
            }
        }

        public void Hide(bool hasAnimation)
        {
            if (hasAnimation)
            {
                m_State = State.HIDE;
                m_FrameCounter = 0;
            }
            else
            {
                m_Controller.Hide(false);
            }
        }

        public void ResortDepth(int min)
        {
            ResortCanvasList(min);
        }

        void Awake()
        {
            if (Application.isPlaying)
            {
                SceneManager.OnLoaded(m_Controller);
            }
        }

        public void CreateShields()
        {
            for (int i = 0; i < m_CanvasArray.Length; i++)
            {
                GameObject g = Instantiate(Resources.Load("Shield")) as GameObject;
                Transform t = g.transform;

                t.SetParent(m_CanvasArray[i].transform);
                t.SetSiblingIndex(0);
                t.localScale = Vector3.one;
            }
        }

        void ResortCanvasList(int min)
        {
            Sort(m_CanvasArray);

            for (int i = 0; i < m_CanvasArray.Length; i++)
            {
                m_CanvasArray[i].sortingOrder = min + i;
            }
        }

        void Sort(Canvas[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = array.Length - 1; j > i; j--)
                {
                    if (array[j].sortingOrder < array[j - 1].sortingOrder)
                    {
                        Swap<Canvas>(ref array[j], ref array[j-1]);
                    }
                }
            }
        }

        void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        void UpdateFrameCounter()
        {
            switch (m_State)
            {
                case State.SHOW:
                    m_FrameCounter++;
                    if (m_FrameCounter == WAIT_ANIM_FRAME)
                    {
                        m_Controller.Show(true);
                        m_State = State.IDLE;
                    }
                    break;
                case State.HIDE:
                    m_FrameCounter++;
                    if (m_FrameCounter == WAIT_ANIM_FRAME)
                    {
                        m_Controller.Hide(true);
                        m_State = State.IDLE;
                    }
                    break;
            }
        }

        #if UNITY_EDITOR
        List<Canvas> canvasTempList = new List<Canvas>();

        void Update()
        {
            AutoFind();
            UpdateFrameCounter();
        }

        void AutoFind()
        {
            if (!Application.isPlaying)
            {
                SetSupporter();
                FindCanvases();
            }
        }

        void SetSupporter()
        {
            if (m_Controller == null)
            {
                m_Controller = GetComponent<Controller>();
                m_Controller.Supporter = this;
            }
        }

        void FindCanvases()
        {
            Canvas[] canvasArray = FindObjectsOfType<Canvas>();

            canvasTempList.Clear();
            for (int i = 0; i < canvasArray.Length; i++)
            {
                if (canvasArray[i].renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvasTempList.Add(canvasArray[i]);
                }
            }

            m_CanvasArray = canvasTempList.ToArray();
        }
        #else
        void Update()
        {
            UpdateFrameCounter();
        }
        #endif
    }
}
