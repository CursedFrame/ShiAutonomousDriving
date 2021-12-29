using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public static class SettingsLoader
    {
        private static SettingsWindowData LoadSettingsAsset()
        {
            SettingsWindowData settingsWindowData = (SettingsWindowData)AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/TrafficSystem/Scripts/Editor/EditorSave/SettingsWindowData.asset", typeof(SettingsWindowData));
            if (settingsWindowData == null)
            {
                SettingsWindowData asset = ScriptableObject.CreateInstance<SettingsWindowData>();
                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins"))
                {
                    AssetDatabase.CreateFolder("Assets/", "GleyPlugins");
                    AssetDatabase.Refresh();
                }

                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/TrafficSystem"))
                {
                    AssetDatabase.CreateFolder("Assets/GleyPlugins", "TrafficSystem");
                    AssetDatabase.Refresh();
                }

                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/TrafficSystem/Scripts"))
                {
                    AssetDatabase.CreateFolder("Assets/GleyPlugins/TrafficSystem", "Scripts");
                    AssetDatabase.Refresh();
                }

                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/TrafficSystem/Scripts/Editor"))
                {
                    AssetDatabase.CreateFolder("Assets/GleyPlugins/TrafficSystem/Scripts", "Editor");
                    AssetDatabase.Refresh();
                }

                if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/TrafficSystem/Scripts/Editor/EditorSave"))
                {
                    AssetDatabase.CreateFolder("Assets/GleyPlugins/TrafficSystem/Scripts/Editor", "EditorSave");
                    AssetDatabase.Refresh();
                }

                AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/TrafficSystem/Scripts/Editor/EditorSave/SettingsWindowData.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                settingsWindowData = (SettingsWindowData)AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/TrafficSystem/Scripts/Editor/EditorSave/SettingsWindowData.asset", typeof(SettingsWindowData));
            }

            return settingsWindowData;
        }


        public static void SaveCreateRoadSettings(CreateRoadSave createRoadSave, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.createRoadSave = createRoadSave;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static CreateRoadSave LoadCreateRoadSave()
        {
            return LoadSettingsAsset().createRoadSave;
        }


        public static void SaveViewRoadsSettings(ViewRoadsSave viewRoadsSave, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.roadColors = roadColors;
            settingsWindowData.viewRoadsSave = viewRoadsSave;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ViewRoadsSave LoadViewRoadsSave()
        {
            return LoadSettingsAsset().viewRoadsSave;
        }


        public static void SaveConnectRoadsSettings(ConnectRoadsSave connectRoadsSave, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.connectRoadsSave = connectRoadsSave;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ConnectRoadsSave LoadConnectRoadsSave()
        {
            return LoadSettingsAsset().connectRoadsSave;
        }


        public static void SaveEditRoadSettings(EditRoadSave editRoadSave, bool[] allowedCarIndex, RoadColors roadColors, RoadDefaults roadDefaults)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            editRoadSave.globalCarList = new List<VehicleTypes>();
            for (int i=0;i<allowedCarIndex.Length;i++)
            {
                if(allowedCarIndex[i]==true)
                {
                    editRoadSave.globalCarList.Add((VehicleTypes)i);
                }
            }
            settingsWindowData.editRoadSave = editRoadSave;
            settingsWindowData.roadColors = roadColors;
            settingsWindowData.roadDefaults = roadDefaults;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static EditRoadSave LoadEditRoadSave()
        {
            return LoadSettingsAsset().editRoadSave;
        }


        public static void SaveAllWaypointsSettings(ViewWaypointsSettings allWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.allWaypointsSettings = allWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static RoadDefaults LoadRoadDefaultsSave()
        {
            return LoadSettingsAsset().roadDefaults;
        }


        public static ViewWaypointsSettings LoadAllWaypointsSave()
        {
            return LoadSettingsAsset().allWaypointsSettings;
        }


        public static void SaveDisconnectedWaypointsSettings(ViewWaypointsSettings disconnectedWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.disconnectedWaypointsSettings = disconnectedWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ViewWaypointsSettings LoadDisconnectedWaypointsSave()
        {
            return LoadSettingsAsset().disconnectedWaypointsSettings;
        }


        public static void SaveGiveWayWaypointsSettings(ViewWaypointsSettings giveWayWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.giveWayWaypointsSettings = giveWayWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ViewWaypointsSettings LoadGiveWayWaypointsSave()
        {
            return LoadSettingsAsset().giveWayWaypointsSettings;
        }


        public static void SaveCarEditedWaypointsSettings(ViewWaypointsSettings carEditedWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.carEditedWaypointsSettings = carEditedWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ViewWaypointsSettings LoadCarEditedWaypointsSave()
        {
            return LoadSettingsAsset().carEditedWaypointsSettings;
        }


        public static void SaveSpeedEditedWaypointsSettings(ViewWaypointsSettings speedEditedWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.speedEditedWaypointsSettings = speedEditedWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ViewWaypointsSettings LoadSpeedEditedWaypointsSave()
        {
            return LoadSettingsAsset().speedEditedWaypointsSettings;
        }


        public static void SaveStopWaypointsSettings(ViewWaypointsSettings stopWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.stopWaypointsSettings = stopWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ViewWaypointsSettings LoadStopdWaypointsSave()
        {
            return LoadSettingsAsset().stopWaypointsSettings;
        }


        public static void SavePathProblemsWaypointsSettings(ViewWaypointsSettings pathProblemsWaypointsSettings, RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.pathProblemsWaypointsSettings = pathProblemsWaypointsSettings;
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }


        public static ViewWaypointsSettings LoadPathProblemsWaypointsSave()
        {
            return LoadSettingsAsset().pathProblemsWaypointsSettings;
        }


        public static RoadColors LoadRoadColors()
        {
            return LoadSettingsAsset().roadColors;
        }


        public static void SaveRoadColors(RoadColors roadColors)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.roadColors = roadColors;
            EditorUtility.SetDirty(settingsWindowData);
        }

        public static void SaveSpeedRoutes(SpeedRoutesSave speedRoutesSave)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.speedRoutesSave = speedRoutesSave;
            EditorUtility.SetDirty(settingsWindowData);
        }

        public static SpeedRoutesSave LoadSpeedRoutes()
        {
            return LoadSettingsAsset().speedRoutesSave;
        }

        public static void SaveCarRoutes(CarRoutesSave carRoutesSave)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.carRoutesSave = carRoutesSave;
            EditorUtility.SetDirty(settingsWindowData);
        }

        public static CarRoutesSave LoadCarRoutes()
        {
            return LoadSettingsAsset().carRoutesSave;
        }

        public static void SaveIntersectionsSettings(IntersectionSave intersectionSave)
        {
            SettingsWindowData settingsWindowData = LoadSettingsAsset();
            settingsWindowData.intersectionSave = intersectionSave;
            EditorUtility.SetDirty(settingsWindowData);
        }

        public static IntersectionSave LoadIntersectionsSettings()
        {
            return LoadSettingsAsset().intersectionSave;
        }
    }
}