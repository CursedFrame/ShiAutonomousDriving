using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class VehicleRoutesSetupWindow : SetupWindowBase
    {
        private int nrOfCars;
        private CarRoutesSave save;
        private float scrollAdjustment = 112;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            WaypointDrawer.Initialize();
            nrOfCars = System.Enum.GetValues(typeof(VehicleTypes)).Length;
            save = SettingsLoader.LoadCarRoutes();

            if (save.routesColor.Count < nrOfCars)
            {
                for (int i = save.routesColor.Count; i < nrOfCars; i++)
                {
                    save.routesColor.Add(Color.white);
                    save.active.Add(true);
                }
            }
            WaypointDrawer.onWaypointClicked += WaypointClicked;
            return base.Initialize(windowProperties);
        }


        public override void DrawInScene()
        {
            for (int i = 0; i < nrOfCars; i++)
            {
                if (save.active[i])
                {
                    WaypointDrawer.DrawWaypointsForCar((VehicleTypes)i, save.routesColor[i]);
                }
            }

            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("Car Routes: ");
            for (int i = 0; i < nrOfCars; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(((VehicleTypes)i).ToString(), GUILayout.MaxWidth(150));
                save.routesColor[i] = EditorGUILayout.ColorField(save.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (save.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View", GUILayout.MaxWidth(BUTTON_DIMENSION)))
                {
                    save.active[i] = !save.active[i];
                    SceneView.RepaintAll();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();
            }

            base.ScrollPart(width, height);
            EditorGUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            WaypointDrawer.onWaypointClicked -= WaypointClicked;
            SettingsLoader.SaveCarRoutes(save);
            base.DestroyWindow();
        }


        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            SettingsWindow.SetActiveWindow(WindowType.EditWaypoint, true);
        }
    }
}