using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Converts editor waypoints to production waypoints
    /// </summary>
    public class GridEditor : Editor
    {
        static List<WaypointSettings> allEditorWaypoints;
        static GenericIntersectionSettings[] allEditorIntersections;


        public static void GenerateGrid(CurrentSceneData currentSceneData)
        {
            System.DateTime startTime = System.DateTime.Now;
            int nrOfColumns;
            int nrOfRows;
            Bounds b = new Bounds();
            foreach (Renderer r in FindObjectsOfType<Renderer>())
            {
                b.Encapsulate(r.bounds);
            }
            nrOfColumns = Mathf.CeilToInt(b.size.x / currentSceneData.gridCellSize);
            nrOfRows = Mathf.CeilToInt(b.size.z / currentSceneData.gridCellSize);
            if (nrOfRows == 0 || nrOfColumns == 0)
            {
                Debug.LogError("Your scene seems empty. Please add some geometry inside your scene before setting up traffic");
                return;
            }
            Debug.Log("Center: " + b.center + " size: " + b.size + " nrOfColumns " + nrOfColumns + " nrOfRows " + nrOfRows);
            Vector3 corner = new Vector3(b.center.x - b.size.x / 2 + currentSceneData.gridCellSize / 2, 0, b.center.z - b.size.z / 2 + currentSceneData.gridCellSize / 2);
            int nr = 0;
            currentSceneData.grid = new GridRow[nrOfRows];
            for (int row = 0; row < nrOfRows; row++)
            {
                currentSceneData.grid[row] = new GridRow(nrOfColumns);
                for (int column = 0; column < nrOfColumns; column++)
                {
                    nr++;
                    currentSceneData.grid[row].row[column] = new GridCell(column, row, new Vector3(corner.x + column * currentSceneData.gridCellSize, 0, corner.z + row * currentSceneData.gridCellSize), currentSceneData.gridCellSize);
                }
            }
            currentSceneData.gridCorner = currentSceneData.grid[0].row[0].center - currentSceneData.grid[0].row[0].size / 2;
            EditorUtility.SetDirty(currentSceneData);
            Debug.Log("Done generate grid in " + (System.DateTime.Now - startTime));
        }


        public static bool AssignWaypoints(CurrentSceneData currentSceneData)
        {
            if (currentSceneData == null || currentSceneData.grid == null || currentSceneData.grid.Length == 0)
            {
                Debug.LogError("Grid is null. Go to Window->Gley->Traffic System->Scene Setup->Grid Setup and set up your grid");
                return false;
            }

            System.DateTime startTime = System.DateTime.Now;
            SetTags();
            ClearAllWaypoints(currentSceneData);
            List<WaypointSettings> allWaypoints = FindObjectsOfType<WaypointSettings>().ToList();
            if (allWaypoints.Count <= 0)
            {
                Debug.LogError("No waypoints found. Go to Window->Gley->Traffic System->Road Setup and create a road");
                return false;
            }

            //reset intersection waypoints
            for(int i=0;i<allWaypoints.Count;i++)
            {
                allWaypoints[i].enter = allWaypoints[i].exit = false;
            }

            allEditorWaypoints = new List<WaypointSettings>();
            allEditorIntersections = FindObjectsOfType<GenericIntersectionSettings>();
            for (int i = 0; i < allEditorIntersections.Length; i++)
            {
                List<IntersectionStopWaypointsSettings> intersectionWaypoints = allEditorIntersections[i].GetAssignedWaypoints();
                for (int j = 0; j < intersectionWaypoints.Count; j++)
                {
                    for (int k = 0; k < intersectionWaypoints[j].roadWaypoints.Count; k++)
                    {
                        if (intersectionWaypoints[j].roadWaypoints[k] == null)
                        {
                            Debug.LogError(allEditorIntersections[i].name + " has null waypoints assigned, please check it.");
                            continue;
                        }
                        else
                        {
                            intersectionWaypoints[j].roadWaypoints[k].enter = true;
                        }
                    }
                }
                List<WaypointSettings> exitWaypoints = allEditorIntersections[i].GetExitWaypoints();
                for (int j = 0; j < exitWaypoints.Count; j++)
                {
                    if (exitWaypoints[j] == null)
                    {
                        Debug.LogError(allEditorIntersections[i].name + " has null waypoints assigned, please check it.");
                    }
                    else
                    {
                        exitWaypoints[j].exit = true;
                    }
                }
            }

            for (int i = allWaypoints.Count - 1; i >= 0; i--)
            {
                if (allWaypoints[i].allowedCars.Count != 0)
                {
                    allEditorWaypoints.Add(allWaypoints[i]);
                    GridCell cell = currentSceneData.GetCell(allWaypoints[i].transform.position);
                    cell.AddWaypoint(allEditorWaypoints.Count - 1, allWaypoints[i].name, allWaypoints[i].allowedCars, allWaypoints[i].enter|| allWaypoints[i].exit);

                }
            }
            currentSceneData.allWaypoints = allEditorWaypoints.ToPlayWaypoints(allEditorWaypoints).ToArray();
            AssignIntersections(currentSceneData);
            EditorUtility.SetDirty(currentSceneData);
            Debug.Log("Done assign waypoints in " + (System.DateTime.Now - startTime));
            return true;
        }


        private static void SetTags()
        {
            ConnectionPool[] allWaypointHolders = FindObjectsOfType<ConnectionPool>();
            for (int i = 0; i < allWaypointHolders.Length; i++)
            {
                allWaypointHolders[i].gameObject.SetTag(Constants.editorTag);
            }
        }


        private static void AssignIntersections(CurrentSceneData currentSceneData)
        {

            List<PriorityIntersection> priorityIntersections = new List<PriorityIntersection>();
            List<TrafficLightsIntersection> lightsIntersections = new List<TrafficLightsIntersection>();
            currentSceneData.allIntersections = new IntersectionData[allEditorIntersections.Length];
            for (int i = 0; i < allEditorIntersections.Length; i++)
            {
                if (allEditorIntersections[i].GetType().Equals(typeof(TrafficLightsIntersectionSettings)))
                {
                    lightsIntersections.Add(((TrafficLightsIntersectionSettings)allEditorIntersections[i]).ToPlayModeIntersection(allEditorWaypoints));
                    currentSceneData.allIntersections[i] = new IntersectionData(IntersectionType.TrafficLights, lightsIntersections.Count - 1);
                }

                if (allEditorIntersections[i].GetType().Equals(typeof(PriorityIntersectionSettings)))
                {
                    priorityIntersections.Add(((PriorityIntersectionSettings)allEditorIntersections[i]).ToPlayModeIntersection(allEditorWaypoints));
                    currentSceneData.allIntersections[i] = new IntersectionData(IntersectionType.Priority, priorityIntersections.Count - 1);
                }

                List<IntersectionStopWaypointsSettings> intersectionWaypoints = allEditorIntersections[i].GetAssignedWaypoints();
                for (int j = 0; j < intersectionWaypoints.Count; j++)
                {
                    for (int k = 0; k < intersectionWaypoints[j].roadWaypoints.Count; k++)
                    {
                        if (intersectionWaypoints[j].roadWaypoints[k] == null)
                        {
                            intersectionWaypoints[j].roadWaypoints.RemoveAt(k);
                        }
                        else
                        {
                            GridCell intersectionCell = currentSceneData.GetCell(intersectionWaypoints[j].roadWaypoints[k].transform.position);
                            intersectionCell.AddIntersection(i);
                        }
                    }
                }
            }
            currentSceneData.allPriorityIntersections = priorityIntersections.ToArray();
            currentSceneData.allLightsIntersections = lightsIntersections.ToArray();
        }


        private static void ClearAllWaypoints(CurrentSceneData currentSceneData)
        {
            if (currentSceneData.grid != null)
            {
                for (int i = 0; i < currentSceneData.grid.Length; i++)
                {
                    for (int j = 0; j < currentSceneData.grid[i].row.Length; j++)
                    {
                        currentSceneData.grid[i].row[j].ClearReferences();
                    }
                }
            }
        }
    }
}

