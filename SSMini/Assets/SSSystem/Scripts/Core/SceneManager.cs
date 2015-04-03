using UnityEngine;
using System.Collections.Generic;

namespace SS
{
    public enum SceneType
    {
        SCENE,
        POPUP
    }

    public struct SceneData
    {
        public Vector3 Position;
        public Data Data;
        public bool HasAnimation;
        public SceneType SceneType;
        public int MinDepth;

        public SceneData(SceneType sceneType, Vector3 position, Data data, bool hasAnimation, int minDepth)
        {
            SceneType = sceneType;
            Position = position;
            Data = data;
            HasAnimation = hasAnimation;
            MinDepth = minDepth;
        }
    }

    public class SceneManager
    {
        static Dictionary<string, Controller> m_Scenes = new Dictionary<string, Controller>();
        static Dictionary<string, SceneData> m_Command = new Dictionary<string, SceneData>();
        static Stack<Controller> m_Stack = new Stack<Controller>();
        static SceneTransition m_SceneTransition;

        static SceneManager()
        {
            Application.targetFrameRate = 60;

            Object sceneTransition = Resources.Load("SceneTransition");
            m_SceneTransition = ((GameObject)GameObject.Instantiate(sceneTransition)).GetComponent<SceneTransition>();

            GameObject sceneSupporter = new GameObject("SceneManagerSupporter");
            sceneSupporter.AddComponent<SceneManagerSupporter>();
        }

        public static void Clear()
        {
            m_Scenes.Clear();
            m_Command.Clear();
            m_Stack.Clear();
        }

        public static void Scene(string sceneName)
        {
            Clear();
            
            AddCommand(sceneName, new SceneData(SceneType.SCENE, Vector3.zero, null, false, 0));
            m_SceneTransition.LoadScene(sceneName);
        }

        public static void Popup(string sceneName, bool hasAnimation = false, Data data = null)
        {
            m_SceneTransition.ShieldOn();

            AddCommand(sceneName, new SceneData(SceneType.POPUP, PopupPosition, data, hasAnimation, m_Stack.Count * 10));

            if (m_Scenes.ContainsKey(sceneName))
            {
                ExcuteCommand(sceneName);
            }
            else
            {
                Application.LoadLevelAdditive(sceneName);
            }
        }

        public static void Close(bool hasAnimation = false)
        {
            if (m_Stack.Count > 0)
            {
                m_SceneTransition.ShieldOn();

                Controller controller = m_Stack.Pop();
                controller.Supporter.Hide(hasAnimation);

                if (m_Stack.Count > 0)
                {
                    controller = m_Stack.Peek();
                    controller.OnFocus(true);
                }
            }
        }

        public static Controller Top()
        {
            if (m_Stack.Count > 0)
            {
                return m_Stack.Peek();
            }
            return null;
        }

        public static void OnLoaded(Controller controller)
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            m_Scenes.Add(controller.SceneName(), controller);
            ExcuteCommand(controller.SceneName());
        }

        public static void OnShown(Controller controller)
        {
            m_SceneTransition.ShieldOff();
            controller.OnShown();
        }

        public static void OnHidden(Controller controller)
        {
            m_SceneTransition.ShieldOff();
            controller.OnFocus(false);
            controller.gameObject.SetActive(false);
            controller.OnHidden();
        }

        public static bool ActiveShield()
        {
            return m_SceneTransition.Active;
        }

        static Vector3 PopupPosition
        {
            get { return Vector3.zero; }
        }

        static void AddCommand(string sceneName, SceneData sceneData)
        {
            m_Command.Add(sceneName, sceneData);
        }

        static void ExcuteCommand(string sceneName)
        {
            Controller controller = m_Scenes[sceneName];
            controller.gameObject.SetActive(true);

            if (m_Stack.Count > 0)
            {
                Controller prevController = m_Stack.Peek();
                prevController.OnFocus(false);
            }
            m_Stack.Push(controller);

            if (m_Command.ContainsKey(sceneName))
            {
                SceneData sceneData = m_Command[sceneName];

                controller.transform.position = sceneData.Position;
                controller.OnActive(sceneData.Data);
                controller.OnFocus(true);
                controller.Supporter.ResortDepth(sceneData.MinDepth);
                controller.Supporter.Show(sceneData.HasAnimation);

                switch (sceneData.SceneType)
                {
                    case SceneType.SCENE:
                        m_SceneTransition.FadeInScene();
                        break;
                    case SceneType.POPUP:
                        controller.Supporter.CreateShields();
                        break;
                }

                m_Command.Remove(sceneName);
            }
            else
            {
                controller.OnActive(null);
                controller.OnFocus(true);
                controller.Supporter.ResortDepth(0);
                controller.Supporter.Show(false);
            }
        }
    }
}