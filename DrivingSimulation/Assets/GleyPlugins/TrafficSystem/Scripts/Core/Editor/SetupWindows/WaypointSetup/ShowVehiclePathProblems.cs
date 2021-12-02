using UnityEngine;

namespace GleyTrafficSystem
{
    public class ShowVehiclePathProblems : ShowWaypointsBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            save = SettingsLoader.LoadPathProblemsWaypointsSave();
            return base.Initialize(windowProperties);
        }


        public override void DrawInScene()
        {
            waypointsOfInterest = WaypointDrawer.ShowVehicleProblems(roadColors.selectedWaypointColor, save.showConnections, roadColors.waypointColor, save.showSpeed, roadColors.speedColor, save.showCars, roadColors.carsColor, save.showOtherLanes, roadColors.waypointColor);
            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            SettingsLoader.SavePathProblemsWaypointsSettings(save,roadColors);
            base.DestroyWindow();
        }
    }
}
