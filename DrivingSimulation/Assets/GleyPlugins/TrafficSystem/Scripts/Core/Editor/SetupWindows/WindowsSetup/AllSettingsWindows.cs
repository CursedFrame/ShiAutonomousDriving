using System.Linq;

namespace GleyTrafficSystem
{
    public static class AllSettingsWindows
    {
        static WindowProperties[] allWindows =
        {
            //main menu
            new WindowProperties(WindowType.SettingsWindow,"Settings Window",false,true,false,true,true,false,"https://youtube.com/playlist?list=PLKeb94eicHQtyL7nYgZ4De1htLs8lmz9C"),
            new WindowProperties(WindowType.ImportPackages,"Import Packages",true,true,true,false,true,false,"https://youtu.be/hjKXg6HtWPI"),
            new WindowProperties(WindowType.RoadSetup,"Road Setup",true,true,true,false,false,false,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(WindowType.WaypointSetup,"Waypoint Setup",true,true,false,true,true,false,"https://youtu.be/mKfnm5_QW8s"),
            new WindowProperties(WindowType.SceneSetup, "Scene Setup",true,true,false,true,false,false,"https://youtu.be/203UgxPlfNo"),
            new WindowProperties(WindowType.ExternalTools, "External Tools",true,true,true,false,false,false,""),
            new WindowProperties(WindowType.Debug, "Debug",true,true,true,false,false,false,"https://youtu.be/Bg-70Tum380"),

            //Road Setup
            new WindowProperties(WindowType.CreateRoad, "Create Road",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(WindowType.ConnectRoads, "Connect Roads",true,true,true,true,true,true,"https://youtu.be/EKTVqvYQ01A"),
            new WindowProperties(WindowType.ViewRoads, "View Roads",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),
            new WindowProperties(WindowType.EditRoad, "Edit Road",true,true,true,true,true,true,"https://youtu.be/-pJwE0Q34no"),

            //Waypoint Setup
             new WindowProperties(WindowType.ShowAllWaypoints, "All Waypoints",true,true,true,false,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(WindowType.ShowCarTypeEditedWaypoints, "Vehicle Edited Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(WindowType.ShowDisconnectedWaypoints, "Disconnected Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(WindowType.ShowGiveWayWaypoints, "Give Way Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(WindowType.ShowSpeedEditedWaypoints, "Speed Edited Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(WindowType.ShowStopWaypoints, "Stop Waypoints",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(WindowType.ShowVehiclePathProblems, "Path Problems",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),
             new WindowProperties(WindowType.EditWaypoint, "Edit Waypoint",true,true,true,true,true,true,"https://youtu.be/mKfnm5_QW8s"),

             //Scene Setup
             new WindowProperties(WindowType.GridSetupWindow, "Grid Setup",true,true,true,true,true,true,"https://youtu.be/203UgxPlfNo"),
             new WindowProperties(WindowType.SpeedRoutesSetupWindow, "Speed Routes",true,true,false,true,true,true,"https://youtu.be/WqrADi8mUcI"),
             new WindowProperties(WindowType.CarRoutesSetupWindow, "Vehicle Routes",true,true,false,true,true,true,"https://youtu.be/JNVwL9hcodw"),
             new WindowProperties(WindowType.LayerSetupWindow, "Layer Setup",true,true,true,false,true,false,"https://youtu.be/203UgxPlfNo"),

             //Intersection
             new WindowProperties(WindowType.IntersectionSetup, "Intersection Setup",true,true,true,true,true,true,"https://youtu.be/iSIE28UoAyY"),
             new WindowProperties(WindowType.PriorityIntersection, "Priority Intersection",true,true,true,true,true,true,"https://youtu.be/iSIE28UoAyY"),
             new WindowProperties(WindowType.TrafficLightsIntersection, "Traffic Lights Intersection",true,true,true,true,true,true,"https://youtu.be/8tOnYiIYxeU"),

             //Car setup
             new WindowProperties(WindowType.CarTypes, "Vehicle Types",true,true,true,true,true,false,"https://youtu.be/203UgxPlfNo"),

             //External Tools
             new WindowProperties(WindowType.EasyRoadsSetup, "Easy Roads Setup",true,true,true,true,true,false,"https://youtu.be/vTAW0jilybI"),
             new WindowProperties(WindowType.CidySetup, "Cidy Setup",true,true,true,true,true,false,"https://youtu.be/dBSGwxH5WoE"),
        };


        public static WindowProperties GetWindowProperties(WindowType windowType)
        {
            return allWindows.First(cond => cond.type == windowType);
        }


        public static string GetWindowName(WindowType windowType)
        {
            return allWindows.First(cond => cond.type == windowType).name;
        }
    }
}
