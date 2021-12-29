using System.IO;
using UnityEditor;
using UnityEngine;
namespace GleyTrafficSystem
{
    public class SettingsWindow : EditorWindow
    {
        private const string WINDOW_NAME = "Traffic System - v.";
        private const string PATH = "Assets//GleyPlugins/TrafficSystem/Scripts/Version.txt";
        private const int MIN_WIDTH = 400;
        private const int MIN_HEIGHT = 500;

        private static SettingsWindow window;
        private static WindowType activeWindow;
        private static ISetupWindow activeSetupWindow;
        private static bool initialized;
        private static bool blockClicks;

        private RaycastHit hitInfo;
        private bool canClick;
        private static bool playState;


        #region Initialization
        [MenuItem("Window/Gley/Traffic System", false, 90)]
        private static void Init()
        {
            initialized = false;
            LoadWindow();
            ResetSettingsWindow();

        }


        static void LoadWindow()
        {
            StreamReader reader = new StreamReader(PATH);
            string longVersion = JsonUtility.FromJson<Gley.About.AssetVersion>(reader.ReadToEnd()).longVersion;
            window = (SettingsWindow)GetWindow(typeof(SettingsWindow));
            window.titleContent = new GUIContent(WINDOW_NAME + longVersion);
            window.minSize = new Vector2(MIN_WIDTH, MIN_HEIGHT);
            window.Show();
        }


        static void ResetSettingsWindow()
        {
            if (window == null)
            {
                LoadWindow();
            }
            playState = Application.isPlaying;
            if (initialized == true)
            {
                return;
            }
            ResetToHomeScreen();
        }

        private static void ResetToHomeScreen()
        {
            initialized = true;
            NavigationRuntimeData.InitializeData();
            SetActiveWindow(WindowType.SettingsWindow, false);
            SceneView.RepaintAll();
        }


        private void OnEnable()
        {
            SceneView.duringSceneGui += OnScene;
        }


        private void OnFocus()
        {
            ResetSettingsWindow();
        }


        private void OnDisable()
        {
            BlockClicks(false);
            SceneView.duringSceneGui -= OnScene;
        }
        #endregion


        #region WindowGUI
        private void OnGUI()
        {
            if(playState!=Application.isPlaying)
            {
                ResetToHomeScreen();
            }
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.Space();

            if (activeSetupWindow == null)
            {
                SetActiveWindow(WindowType.SettingsWindow, false);
            }

            if (activeSetupWindow.DrawInWIndow(position.width, position.height) == false)
            {
                Back();
            }
        }


        public static void Refresh()
        {
            window.Repaint();
        }
        #endregion


        #region SceneDisplay
        private void OnScene(SceneView obj)
        {
            if (playState != Application.isPlaying)
            {
                ResetToHomeScreen();
            }

            if (GleyPrefabUtilities.PrefabChanged())
            {
                ResetToHomeScreen();
            }

            if (blockClicks == false)
                return;

            Color handlesColor = Handles.color;
            Matrix4x4 handlesMatrix = Handles.matrix;
            Draw();
            Input();
            Handles.color = handlesColor;
            Handles.matrix = handlesMatrix;
        }

       

        private void Draw()
        {
            activeSetupWindow.DrawInScene();
        }
        #endregion


        #region Input
        private void Input()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;

            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
            {
                UndoAction();
            }

            if (e.type == EventType.MouseMove)
            {
                Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (GleyPrefabUtilities.EditingInsidePrefab())
                {
                    if (GleyPrefabUtilities.GetScenePrefabRoot().scene.GetPhysicsScene().Raycast(worldRay.origin, worldRay.direction, out hitInfo, Mathf.Infinity, NavigationRuntimeData.GetRoadLayers()))
                    {
                        canClick = true;
                    }
                    else
                    {
                        canClick = false;
                    }
                }
                else
                {
                    if (Physics.Raycast(worldRay, out hitInfo, Mathf.Infinity, NavigationRuntimeData.GetRoadLayers()))
                    {
                        canClick = true;
                    }
                    else
                    {
                        canClick = false;
                    }
                }
                MouseMove();
            }

            if (canClick)
            {
                if (e.type == EventType.MouseDown && e.shift)
                {
                    if (e.button == 0)
                    {
                        LeftClick();
                        e.Use();
                    }
                    if (e.button == 1)
                    {
                        RightClick();
                        e.Use();
                    }
                }
            }
        }


        private void MouseMove()
        {
            switch (activeWindow)
            {    
                case WindowType.EditRoad:
                    activeSetupWindow.MouseMove(hitInfo.point);
                    break;
            }
        }


        private void LeftClick()
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.LeftClick(hitInfo.point);
            }
        }


        private void RightClick()
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.RightClick(hitInfo.point);
            }
        }


        private void UndoAction()
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.UndoAction();
            }
        }
        #endregion


        #region WindowNavigation
        public static void SetActiveWindow(WindowType newWindow, bool addCurrent)
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.DestroyWindow();
            }
            if (addCurrent)
            {
                NavigationRuntimeData.AddWindow(activeSetupWindow.GetWindowType());
            }
            activeWindow = newWindow;
            switch (activeWindow)
            {
                case WindowType.SettingsWindow:
                    activeSetupWindow = CreateInstance<MainMenuWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ImportPackages:
                    activeSetupWindow = CreateInstance<ImportPackagesWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.RoadSetup:
                    activeSetupWindow = CreateInstance<RoadSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.WaypointSetup:
                    activeSetupWindow = CreateInstance<WaypointSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.SceneSetup:
                    activeSetupWindow = CreateInstance<SceneSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ExternalTools:
                    activeSetupWindow = CreateInstance<ExternalToolsWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.Debug:
                    activeSetupWindow = CreateInstance<DebugWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;

                //road windows
                case WindowType.CreateRoad:
                    activeSetupWindow = CreateInstance<NewRoadWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ConnectRoads:
                    activeSetupWindow = CreateInstance<ConnectRoadsWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ViewRoads:
                    activeSetupWindow = CreateInstance<ViewRoadsWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.EditRoad:
                    activeSetupWindow = CreateInstance<EditRoadWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;

                //waypoint windows
                case WindowType.ShowAllWaypoints:
                    activeSetupWindow = CreateInstance<ShowAllWaypoints>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ShowCarTypeEditedWaypoints:
                    activeSetupWindow = CreateInstance<ShowVehicleTypeEditedWaypoints>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ShowDisconnectedWaypoints:
                    activeSetupWindow = CreateInstance<ShowDisconnectedWaypoints>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ShowGiveWayWaypoints:
                    activeSetupWindow = CreateInstance<ShowGiveWayWaypoints>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ShowSpeedEditedWaypoints:
                    activeSetupWindow = CreateInstance<ShowSpeedEditedWaypoints>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ShowStopWaypoints:
                    activeSetupWindow = CreateInstance<ShowStopWaypoints>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.ShowVehiclePathProblems:
                    activeSetupWindow = CreateInstance<ShowVehiclePathProblems>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.EditWaypoint:
                    activeSetupWindow = CreateInstance<EditWaypointWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;

                //scene setup
                case WindowType.GridSetupWindow:
                    activeSetupWindow = CreateInstance<GridSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.SpeedRoutesSetupWindow:
                    activeSetupWindow = CreateInstance<SpeedRoutesSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.CarRoutesSetupWindow:
                    activeSetupWindow = CreateInstance<VehicleRoutesSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.LayerSetupWindow:
                    activeSetupWindow = CreateInstance<LayerSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;

                //intersection setup
                case WindowType.IntersectionSetup:
                    activeSetupWindow = CreateInstance<IntersectionSetupWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.PriorityIntersection:
                    activeSetupWindow = CreateInstance<PriorityIntersectionWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
                case WindowType.TrafficLightsIntersection:
                    activeSetupWindow = CreateInstance<TrafficLightsIntersectionWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;

                //car
                case WindowType.CarTypes:
                    activeSetupWindow = CreateInstance<VehicleTypesWindow>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;

                // External Tools
                case WindowType.EasyRoadsSetup:
                    activeSetupWindow = CreateInstance<EasyRoadsSetup>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;

                case WindowType.CidySetup:
                    activeSetupWindow = CreateInstance<CidySetup>().Initialize(AllSettingsWindows.GetWindowProperties(activeWindow));
                    break;
            }
            if (window)
            {
                window.Repaint();
            }
        }


        public static void Back()
        {
            SetActiveWindow(NavigationRuntimeData.RemoveLastWindow(), false);
        }

        public static void BlockClicks(bool block)
        {
            blockClicks = block;
        }


        private void OnDestroy()
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.DestroyWindow();
            }
        }
        #endregion
    }
}
