using UnityEngine;

namespace GleyTrafficSystem
{
    public class ShowDisconnectedWaypoints : ShowWaypointsBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {         
            save = SettingsLoader.LoadDisconnectedWaypointsSave();
             return base.Initialize(windowProperties);
        }


        public override void DrawInScene()
        {
            waypointsOfInterest = WaypointDrawer.ShowDisconnectedWaypoints(roadColors.waypointColor, save.showConnections, roadColors.waypointColor, save.showSpeed, roadColors.speedColor, save.showCars, roadColors.carsColor);
           
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
            SettingsLoader.SaveDisconnectedWaypointsSettings(save, roadColors);
            base.DestroyWindow();
        }
    }
}
