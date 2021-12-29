namespace GleyTrafficSystem
{
    public class ShowAllWaypoints : ShowWaypointsBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            save = SettingsLoader.LoadAllWaypointsSave();
            return base.Initialize(windowProperties);
        }


        public override void DrawInScene()
        {
            WaypointDrawer.DrawAllWaypoints(roadColors.waypointColor, save.showConnections, roadColors.waypointColor, save.showSpeed, roadColors.speedColor, save.showCars, roadColors.carsColor, save.showOtherLanes, roadColors.laneChangeColor);
            base.DrawInScene();
        }


        public override void DestroyWindow()
        {
            SettingsLoader.SaveAllWaypointsSettings(save, roadColors);
            base.DestroyWindow();
        }
    }
}
