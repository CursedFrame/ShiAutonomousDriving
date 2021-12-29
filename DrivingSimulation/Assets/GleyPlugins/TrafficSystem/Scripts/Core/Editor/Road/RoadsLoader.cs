using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class RoadsLoader : Editor
    {
        static RoadsLoader instance;


        public static RoadsLoader Initialize()
        {
            if (instance == null)
            {
                instance = CreateInstance<RoadsLoader>();
            }
            return instance;
        }


        public List<Road> LoadAllRoads()
        {
            List<Road> allRoads;
            if (GleyPrefabUtilities.EditingInsidePrefab())
            {
                GameObject prefabRoot = GleyPrefabUtilities.GetScenePrefabRoot();
                allRoads = prefabRoot.GetComponentsInChildren<Road>().ToList();
                for (int i = 0; i < allRoads.Count; i++)
                {
                    allRoads[i].positionOffset = prefabRoot.transform.position;
                    allRoads[i].rotationOffset = prefabRoot.transform.localEulerAngles;
                }
            }
            else
            {
                allRoads = FindObjectsOfType<Road>().ToList();
                for (int i = 0; i < allRoads.Count; i++)
                {
                    allRoads[i].isInsidePrefab = GleyPrefabUtilities.IsInsidePrefab(allRoads[i].gameObject);
                    if (allRoads[i].isInsidePrefab)
                    {
                        allRoads[i].positionOffset = GleyPrefabUtilities.GetInstancePrefabRoot(allRoads[i].gameObject).transform.position;
                        allRoads[i].rotationOffset = GleyPrefabUtilities.GetInstancePrefabRoot(allRoads[i].gameObject).transform.localEulerAngles;
                    }
                }
            }
            return allRoads;
        }
    }
}
