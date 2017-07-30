// This code is part of the Mini SS-System library (https://github.com/AnhPham/ssmini) maintained by Anh Pham (anhpt.csit@gmail.com).
// It is released for free under the MIT open source license (https://github.com/AnhPham/ssmini/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace SS
{
    public enum SceneType
    {
        DEFAULT,
        SCENE,
        SCENE_CLEAR_ALL,
        POPUP,
        VIEW,
        VIEW_FULL_SCREEN,
        LOADING,
        TAB
    }

    public struct SceneData
    {
        public Vector3 Position;
        public Data Data;
        public bool HasAnimation;
        public SceneType SceneType;
        public int MinDepth;
        public bool InStack;

        public SceneData(SceneType sceneType, Vector3 position, Data data, bool hasAnimation, int minDepth, bool inStack)
        {
            SceneType = sceneType;
            Position = position;
            Data = data;
            HasAnimation = hasAnimation;
            MinDepth = minDepth;
            InStack = inStack;
        }
    }

    public class SceneManager
    {
        static Dictionary<string, Controller> m_Scenes = new Dictionary<string, Controller>();
        static Dictionary<string, SceneData> m_Command = new Dictionary<string, SceneData>();
        static Stack<Controller> m_Stack = new Stack<Controller>();
        static List<string> m_IgnorePrefixList = new List<string>();

        static SceneTransition m_SceneTransition;
        static SceneManagerSupporter m_SceneSupporter;
        static Controller m_CurrentSceneController;

        static Controller m_LoadingController;
        static bool m_LoadingActive;

        static Controller m_TabController;
        static bool m_TabActive;

        static string m_CurrentSceneName;
        static Camera m_CameraUI;

        static SceneManager()
        {
            Application.targetFrameRate = 60;

            Object sceneTransition = Resources.Load("SceneTransition");
            m_SceneTransition = ((GameObject)GameObject.Instantiate(sceneTransition)).GetComponent<SceneTransition>();
            m_SceneTransition.gameObject.name = "SceneTransition";

            GameObject sceneSupporter = new GameObject("SceneManagerSupporter");
            m_SceneSupporter = sceneSupporter.AddComponent<SceneManagerSupporter>();

            ShieldColor = new Color(0.235f, 0.235f, 0.235f, 0.5f);

            SceneFadeTime = 0.5f;
            SceneAnimationTime = 0.283f;
        }

        public static Color ShieldColor
        {
            get;
            set;
        }

        public static float SceneAnimationTime
        {
            get;
            set;
        }

        public static float SceneFadeTime
        {
            get;
            set;
        }

        public static string ScenePrefix
        {
            get;
            set;
        }

        public static void SetIgnorePrefixScenes(params string[] sceneNames)
        {
            m_IgnorePrefixList = new List<string>(sceneNames);
        }

        public static void Scene(string sceneName, bool clearAll, Data data = null)
        {
            sceneName = AddPrefixToSceneName(sceneName);

            if (clearAll)
            {
                ClearCommands();
                AddCommand(sceneName, new SceneData(SceneType.SCENE_CLEAR_ALL, ScenePosition, data, false, 0, true));
                m_SceneTransition.LoadScene(sceneName, clearAll);
            }
            else
            {
                if (m_CurrentSceneController == null || sceneName.CompareTo(m_CurrentSceneController.SceneName()) != 0)
                {
                    AddCommand(sceneName, new SceneData(SceneType.SCENE, ScenePosition, data, false, 0, true));
                    m_SceneTransition.LoadScene(sceneName, clearAll);
                }
                else
                {
                    BackToScene(true);
                }
            }
        }

        public static void OnFadedIn(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = m_CurrentSceneName;
            }
            Controller controller = m_Scenes[sceneName];
            controller.Supporter.OnShown();
        }

        public static void OnFadedOut(string sceneName, bool clearAll)
        {
            if (clearAll)
            {
                HideAll();
                ClearScenes();
                #if UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_6
                Application.LoadLevel(sceneName);
                #else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
                #endif
            }
            else
            {
                if (m_Scenes.ContainsKey(sceneName))
                {
                    ExcuteCommand(sceneName);
                }
                else
                {
                    #if UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_6
                    Application.LoadLevelAdditive(sceneName);
                    #else
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    #endif
                }
            }
        }

        public static void Reset(Data data = null)
        {
            if (!string.IsNullOrEmpty(m_CurrentSceneName))
            {
                Scene(m_CurrentSceneName, true, data);
            }
        }

        public static string CurrentSceneName
        {
            get
            {
                return m_CurrentSceneName;
            }
        }

        public static void Popup(string sceneName, bool hasAnimation = false, Data data = null)
        {
            AddView(sceneName, SceneType.POPUP, PopupPosition,  m_Stack.Count * 10 + 60, hasAnimation, data);
        }

        public static void View(string sceneName, bool fullScreen = false, bool hasAnimation = false, Data data = null)
        {
            SceneType sceneType = (fullScreen) ? SceneType.VIEW_FULL_SCREEN : SceneType.VIEW;
            AddView(sceneName, sceneType, ViewPosition, m_Stack.Count * 10, hasAnimation, data);
        }

        public static string LoadingSceneName
        {
            set
            {
                SetFixController(m_LoadingController, value, SceneType.LOADING, LoadingPosition, 99);
            }
        }

        public static void Loading(bool active)
        {
            ActiveFixController(m_LoadingController, ref m_LoadingActive, active);
        }

        public static string TabSceneName
        {
            set
            {
                SetFixController(m_TabController, value, SceneType.TAB, TabPosition, 50);
            }
        }

        public static Camera CameraUI
        {
            set
            {
                m_CameraUI = value;
            }
        }

        public static void Tab(bool active)
        {
            ActiveFixController(m_TabController, ref m_TabActive, active);
        }

        public static void Close(bool hasAnimation = false)
        {
            if (m_Stack.Count > 0)
            {
                m_SceneTransition.ShieldOn();

                Controller controller = m_Stack.Pop();
                controller.Supporter.Hide(hasAnimation);

                switch (controller.SceneData.SceneType)
                {
                    case SceneType.VIEW_FULL_SCREEN:
                        if (m_Stack.Count > 0)
                        {
                            Controller topController = m_Stack.Peek();
                            topController.gameObject.SetActive(true);
                        }
                        break;
                }
            }
        }

        public static void BackToScene(bool hasAnimation = false)
        {
            if (m_Stack.Count > 1)
            {
                Controller topController = m_Stack.Pop();

                while (m_Stack.Count > 1)
                {
                    Controller controller = m_Stack.Pop();
                    LostFocusAndRaiseHidden(controller);
                }

                m_Stack.Push(topController);

                Close(hasAnimation);
            }
        }

        public static void HideAll()
        {
            while (m_Stack.Count > 0)
            {
                Controller controller = m_Stack.Pop();
                LostFocusAndRaiseHidden(controller);
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
            m_Scenes.Add(controller.SceneName(), controller);
            foreach (var scene in m_Scenes)
            {
                scene.Value.OnAnySceneLoaded(controller);
            }

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            ExcuteCommand(controller.SceneName());
        }

        public static void OnShown(Controller controller)
        {
            switch (controller.SceneData.SceneType)
            {
                case SceneType.POPUP:
                case SceneType.VIEW:
                case SceneType.VIEW_FULL_SCREEN:
                    m_SceneTransition.ShieldOff();
                    break;
            }

            switch (controller.SceneData.SceneType)
            {
                case SceneType.DEFAULT:
                case SceneType.SCENE:
                case SceneType.SCENE_CLEAR_ALL:
                    break;
                default:
                    RaiseShownAndDeactivePrevScenes(controller);
                    break;
            }
        }

        public static void OnHidden(Controller controller)
        {
            m_SceneTransition.ShieldOff();

            LostFocusAndRaiseHidden(controller);
            ActiveTopSceneOnHidden(controller);
        }

        public static void RemoveAudioListener()
        {
            if (m_SceneSupporter == null)
                return;
            
            AudioListener al = m_SceneSupporter.GetComponent<AudioListener>();
            if (al != null)
            {
                Component.Destroy(al);
            }
        }

        public static void AddAudioListener()
        {
            if (m_SceneSupporter == null)
                return;
            
            AudioListener al = m_SceneSupporter.GetComponent<AudioListener>();
            if (al == null)
            {
                m_SceneSupporter.gameObject.AddComponent<AudioListener>();
            }
        }

        static void RaiseShownAndDeactivePrevScenes(Controller controller)
        {
            controller.Supporter.OnShown();

            DeactivePrevSceneOnShown(controller);
        }

        static void LostFocusAndRaiseHidden(Controller controller)
        {
            controller.OnFocus(false);
            controller.gameObject.SetActive(false);
            controller.Supporter.OnHidden();
        }

        public static bool ActiveShield()
        {
            return m_SceneTransition.Active || m_LoadingActive;
        }

        static void SetFixController(Controller controller, string sceneName, SceneType sceneType, Vector3 position, int minDepth)
        {
            if (controller == null)
            {
                AddCommand(sceneName, new SceneData(sceneType, position, null, false, minDepth, false));
                #if UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_6
                Application.LoadLevelAdditive(sceneName);
                #else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                #endif
            }
        }

        static void ActiveFixController(Controller controller, ref bool controllerActive, bool active)
        {
            controllerActive = active;

            if (controller != null)
            {
                controller.gameObject.SetActive(active);
            }
        }

        static void ClearCommands()
        {
            m_Command.Clear();
        }

        static void ClearScenes()
        {
            m_Scenes.Clear();
            m_Stack.Clear();
        }

        static void AddView(string sceneName, SceneType sceneType, Vector3 position, int minDepth, bool hasAnimation = false, Data data = null)
        {
            sceneName = AddPrefixToSceneName(sceneName);

            m_SceneTransition.ShieldOn();

            AddCommand(sceneName, new SceneData(sceneType, position, data, hasAnimation, minDepth, true));

            if (m_Scenes.ContainsKey(sceneName))
            {
                ExcuteCommand(sceneName);
            }
            else
            {
                #if UNITY_5_2 || UNITY_5_1 || UNITY_5_0 || UNITY_4_6
                Application.LoadLevelAdditive(sceneName);
                #else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                #endif
            }
        }

        static string AddPrefixToSceneName(string sceneName)
        {
            return m_IgnorePrefixList.Contains(sceneName) ? sceneName : ScenePrefix + sceneName;
        }

        static Vector3 ScenePosition
        {
            get { return Vector3.zero; }
        }

        static Vector3 PopupPosition
        {
            get { return Vector3.zero; }
        }

        static Vector3 ViewPosition
        {
            get { return Vector3.zero; }
        }

        static Vector3 LoadingPosition
        {
            get { return Vector3.zero; }
        }

        static Vector3 TabPosition
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

            if (m_Command.ContainsKey(sceneName))
            {
                SceneData sceneData = m_Command[sceneName];

                ExcutePushStack(controller, sceneData.InStack);
                ExcuteController(controller, sceneData);
                ExcuteSceneType(controller, sceneData.SceneType);

                m_Command.Remove(sceneName);
            }
            else
            {
                ExcutePushStack(controller, true);
                ExcuteController(controller, new SceneData());
                ExcuteSceneType(controller, SceneType.DEFAULT);
            }

            foreach (var scene in m_Scenes)
            {
                scene.Value.OnAnySceneActivated(controller);
            }
        }

        static void ExcutePushStack(Controller controller, bool inStack)
        {
            if (inStack)
            {
                if (m_Stack.Count > 0)
                {
                    Controller prevController = m_Stack.Peek();
                    prevController.OnFocus(false);
                }
                m_Stack.Push(controller);
            }
        }

        static void ExcuteController(Controller controller, SceneData sceneData)
        {
            controller.SceneData = sceneData;
            controller.transform.position = sceneData.Position;
            controller.Supporter.OnActive(sceneData.Data);
            controller.OnFocus(true);
            controller.Supporter.ResortDepth(sceneData.MinDepth);
            controller.Supporter.AssignCameraUI(m_CameraUI);
            controller.Supporter.Show(sceneData.HasAnimation);
        }

        static void ExcuteSceneType(Controller controller, SceneType sceneType)
        {
            switch (sceneType)
            {
                case SceneType.DEFAULT:
                    m_SceneTransition.FadeInScene();
                    controller.Supporter.ActiveEventSystem(true);
                    m_CurrentSceneController = controller;
                    m_CurrentSceneName = controller.SceneName();
                    break;
                case SceneType.SCENE:
                    m_SceneTransition.FadeInScene();
                    controller.Supporter.DestroyEventSystem();
                    controller.Supporter.ReplaceEventSystem(m_CurrentSceneController);
                    m_CurrentSceneController = controller;
                    m_CurrentSceneName = controller.SceneName();
                    DeactivePrevSceneOnShown(controller);
                    break;
                case SceneType.SCENE_CLEAR_ALL:
                    m_SceneTransition.FadeInScene();
                    m_CurrentSceneController = controller;
                    m_CurrentSceneName = controller.SceneName();
                    controller.Supporter.ActiveEventSystem(true);
                    break;
                case SceneType.POPUP:
                    controller.Supporter.CreateShields();
                    controller.Supporter.DestroyEventSystem();
                    break;
                case SceneType.VIEW:
                    controller.Supporter.DestroyEventSystem();
                    break;
                case SceneType.VIEW_FULL_SCREEN:
                    controller.Supporter.DestroyEventSystem();
                    break;
                case SceneType.LOADING:
                    ExcuteFixController(ref m_LoadingController, controller, m_LoadingActive);
                    break;
                case SceneType.TAB:
                    ExcuteFixController(ref m_TabController, controller, m_TabActive);
                    break;
            }
        }

        static void ExcuteFixController(ref Controller fixController, Controller controller, bool active)
        {
            fixController = controller;
            fixController.gameObject.SetActive(active);
            fixController.Supporter.DestroyEventSystem();
            GameObject.DontDestroyOnLoad(fixController.gameObject);
        }

        static void DeactivePrevSceneOnShown(Controller controller)
        {
            switch (controller.SceneData.SceneType)
            {
                case SceneType.SCENE:
                    if (m_Stack.Count > 1)
                    {
                        m_Stack.Pop();

                        while (m_Stack.Count > 0)
                        {
                            Controller prevController = m_Stack.Pop();
                            DeactiveRaiseHidden(prevController);
                        }

                        m_Stack.Push(controller);
                    }
                    break;
                case SceneType.VIEW_FULL_SCREEN:
                    if (m_Stack.Count > 1)
                    {
                        m_Stack.Pop();

                        Controller prevController = m_Stack.Peek();
                        DeactiveRaiseHidden(prevController);

                        m_Stack.Push(controller);
                    }
                    break;
            }
        }

        static void DeactiveRaiseHidden(Controller controller)
        {
            controller.gameObject.SetActive(false);
            controller.Supporter.OnHidden();
        }

        static void ActiveTopSceneOnHidden(Controller controller)
        {
            if (m_Stack.Count > 0)
            {
                Controller topController = m_Stack.Peek();
                topController.OnFocus(true);

                switch (controller.SceneData.SceneType)
                {
                    case SceneType.VIEW_FULL_SCREEN:
                        topController.Supporter.OnShown();
                        break;
                }
            }
        }
    }
}