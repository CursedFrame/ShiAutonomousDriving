using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class RoadCreator
    {
        const string trafficWaypointsHolderName = "TrafficWaypointsHolder";
        static Transform roadWaypointsHolder;


        public Road Create(Vector3 startPosition)
        {
            int roadNumber = GetFreeRoadNumber();
            GameObject roadHolder = new GameObject("Road_" + roadNumber);
            roadHolder.tag = Constants.editorTag;
            roadHolder.transform.SetParent(GetRoadWaypointsHolder());
            roadHolder.transform.SetSiblingIndex(roadNumber);
            roadHolder.transform.position = startPosition;
            RoadDefaults roadDefaults = SettingsLoader.LoadRoadDefaultsSave();
            Road road = roadHolder.AddComponent<Road>().SetDefaults(roadDefaults.nrOfLanes, roadDefaults.laneWidth, roadDefaults.waypointDistance);

            EditorUtility.SetDirty(road);
            AssetDatabase.SaveAssets();
            return road;
        }


        public static Transform GetRoadWaypointsHolder()
        {
            bool editingInsidePrefab = GleyPrefabUtilities.EditingInsidePrefab();

            if (roadWaypointsHolder == null)
            {
                GameObject holder = null;
                if (editingInsidePrefab)
                {
                    GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                    Transform waypointsHolder = prefabRoot.transform.Find(trafficWaypointsHolderName);
                    if (waypointsHolder == null)
                    {
                        waypointsHolder = new GameObject(trafficWaypointsHolderName).transform;
                        waypointsHolder.SetParent(prefabRoot.transform);
                        waypointsHolder.gameObject.AddComponent<ConnectionPool>();
                        waypointsHolder.gameObject.tag = Constants.editorTag;
                    }
                    holder = waypointsHolder.gameObject;
                }
                else
                {
                    GameObject[] allObjects = Object.FindObjectsOfType<GameObject>().Where(obj => obj.name == trafficWaypointsHolderName).ToArray();
                    if (allObjects.Length > 0)
                    {
                        for (int i = 0; i < allObjects.Length; i++)
                        {
                            if (!GleyPrefabUtilities.IsInsidePrefab(allObjects[i]))
                            {
                                holder = allObjects[i];
                                break;
                            }
                        }
                    }
                    if (holder == null)
                    {
                        holder = new GameObject(trafficWaypointsHolderName);
                        holder.AddComponent<ConnectionPool>();
                    }
                }
                roadWaypointsHolder = holder.transform;
            }
            return roadWaypointsHolder;
        }


        private int GetFreeRoadNumber()
        {
            int nr = 0;
            for (int i = 0; i < GetRoadWaypointsHolder().childCount; i++)
            {
                if ("Road_" + nr != roadWaypointsHolder.GetChild(i).name)
                {
                    return nr;
                }
                nr++;
            }
            return nr;
        }
    }
}
