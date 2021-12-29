using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class SpeedRoutesSetupWindow : SetupWindowBase
    {
        private List<int> speeds;
        private SpeedRoutesSave save;
        private float scrollAdjustment = 112;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            speeds = WaypointDrawer.GetDifferentSpeeds();
            save = SettingsLoader.LoadSpeedRoutes();
            if (save.routesColor.Count < speeds.Count)
            {
                int nrOfColors = speeds.Count - save.routesColor.Count;
                for (int i = 0; i < nrOfColors; i++)
                {
                    save.routesColor.Add(Color.white);
                    save.active.Add(true);
                }
            }

            WaypointDrawer.onWaypointClicked += WaypointClicked;
            return base.Initialize(windowProperties);
        }


        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            SettingsWindow.SetActiveWindow(WindowType.EditWaypoint, true);
        }


        public override void DrawInScene()
        {
            for (int i = 0; i < speeds.Count; i++)
            {
                if (save.active[i])
                {
                    WaypointDrawer.ShowSpeedLimits(speeds[i], save.routesColor[i]);
                }
            }

            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("SpeedRoutes: ");
            for (int i = 0; i < speeds.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(speeds[i].ToString(), GUILayout.MaxWidth(50));
                save.routesColor[i] = EditorGUILayout.ColorField(save.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (save.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View"))
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
            SettingsLoader.SaveSpeedRoutes(save);
            base.DestroyWindow();
        }
    }
}
