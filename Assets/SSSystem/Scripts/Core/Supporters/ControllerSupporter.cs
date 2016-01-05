using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

        [SerializeField]
        EventSystem m_EventSystem;

        int m_FrameCounter;
        bool m_CreatedShields;
        State m_State;
        Data m_Data;

        void Awake()
        {
            if (Application.isPlaying)
            {
                SceneManager.OnLoaded(m_Controller);
            }
        }

        public virtual void OnActive(Data data)
        {
            m_Controller.OnActive(data);
            m_Data = data;
        }

        public void DestroyEventSystem()
        {
            if (m_EventSystem != null)
            {
                Destroy(m_EventSystem.gameObject);
            }
        }

        public void ActiveEventSystem(bool active)
        {
            if (m_EventSystem != null)
            {
                m_EventSystem.enabled = active;

                if (active)
                {
                    m_EventSystem.transform.parent = null;
                }
            }
        }

        public void ReplaceEventSystem(Controller controller)
        {
            if (controller != null)
            {
                m_EventSystem = controller.Supporter.m_EventSystem;
                controller.Supporter.m_EventSystem = null;
            }
        }

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

        public void OnShown()
        {
            m_Controller.OnShown();

            if (m_Data != null && m_Data.OnShown != null)
            {
                m_Data.OnShown(m_Controller);
            }
        }

        public void OnHidden()
        {
            m_Controller.OnHidden();

            if (m_Data != null && m_Data.OnHidden != null)
            {
                m_Data.OnHidden(m_Controller);
            }
        }

        public void ResortDepth(int min)
        {
            ResortCanvasList(min);
        }

        public void CreateShields()
        {
            if (!m_CreatedShields)
            {
                for (int i = 0; i < m_CanvasArray.Length; i++)
                {
                    GameObject g = Instantiate<GameObject>(Resources.Load<GameObject>("Shield"));
                    g.name = "Shield";

                    Image image = g.GetComponent<Image>();
                    image.color = SceneManager.ShieldColor;

                    Transform t = g.transform;
                    t.SetParent(m_CanvasArray[i].transform);
                    t.SetSiblingIndex(0);
                    t.localScale = Vector3.one;

                    RectTransform rt = t.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;
                    rt.anchorMax = Vector2.one;
                    rt.pivot = new Vector2(0.5f, 0.5f);
					rt.offsetMax = new Vector2 (2, 2);
					rt.offsetMin = new Vector2 (-2, -2);
                }

                m_CreatedShields = true;
            }
        }

        void ResortCanvasList(int min)
        {
            Sort(m_CanvasArray);

            for (int i = 0; i < m_CanvasArray.Length; i++)
            {
                if (m_CanvasArray[i] != null)
                {
                    m_CanvasArray[i].sortingOrder = min + i;
                }
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
                FindEventSystem();
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

        void FindEventSystem()
        {
            m_EventSystem = FindObjectOfType<EventSystem>();
            ActiveEventSystem(false);
        }
        #else
        void Update()
        {
            UpdateFrameCounter();
        }
        #endif
    }
}
