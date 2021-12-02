using System.Collections.Generic;
using System.Linq;
#if USE_GLEY_TRAFFIC
using Unity.Collections;
using Unity.Mathematics;
#endif
using UnityEngine;
namespace GleyTrafficSystem
{
    /// <summary>
    /// Stores all references 
    /// </summary>
    //TODO this class should be just for storing values
    public class CurrentSceneData : MonoBehaviour
    {
        public int gridCellSize = 50;
        public Vector3 gridCorner;
        public GridRow[] grid;
        public Waypoint[] allWaypoints;
        public IntersectionData[] allIntersections;
        public PriorityIntersection[] allPriorityIntersections;
        public TrafficLightsIntersection[] allLightsIntersections;
#if USE_GLEY_TRAFFIC
        private List<Vector2Int> activeCells;
        private List<GenericIntersection> activeIntersections;
        private List<Vector2Int> currentCells;
#endif
        private SpawnWaypointSelector spawnWaypointSelector;
        private SpawnWaypointSelector SpawnWaypointSelector
        {
            get
            {
                if (spawnWaypointSelector == null)
                {
                    spawnWaypointSelector = GetBestNeighbor.GetRandomSpawnWaypoint;
                }
                return spawnWaypointSelector;
            }
        }


        /// <summary>
        /// Get scene data object from active scene 
        /// </summary>
        /// <returns></returns>
        public static CurrentSceneData GetSceneInstance()
        {
            CurrentSceneData[] allSceneGrids = FindObjectsOfType<CurrentSceneData>();
            if (allSceneGrids.Length > 1)
            {
                Debug.LogError("Multiple Grid components exists in scene. Just one is required, delete extra components before continuing.");
                for (int i = 0; i < allSceneGrids.Length; i++)
                {
                    Debug.LogWarning("Grid component exists on: " + allSceneGrids[i].name, allSceneGrids[i]);
                }
            }

            if (allSceneGrids.Length == 0)
            {
                GameObject go = new GameObject(Constants.gleyTrafficHolderName);
                CurrentSceneData grid = go.AddComponent<CurrentSceneData>();
                return grid;
            }
            return allSceneGrids[0];
        }

#if USE_GLEY_TRAFFIC
        /// <summary>
        /// Initialize current cell
        /// </summary>
        /// <param name="positions"></param>
        public void Initialize(NativeArray<float3> positions)
        {
            activeIntersections = new List<GenericIntersection>();
            currentCells = new List<Vector2Int>();
            for (int i = 0; i < positions.Length; i++)
            {
                currentCells.Add(new Vector2Int());
            }
            UpdateActiveCells(positions, 1);
        }


        public void SetSpawnWaypointSelector(SpawnWaypointSelector spawnWaypointSelector)
        {
            this.spawnWaypointSelector = spawnWaypointSelector;
        }

        /// <summary>
        /// Update active cells based on player position
        /// </summary>
        /// <param name="positions">position to check</param>
        public void UpdateActiveCells(NativeArray<float3> positions, int level)
        {
            if (currentCells.Count != positions.Length)
            {
                currentCells = new List<Vector2Int>();
                for (int i = 0; i < positions.Length; i++)
                {
                    currentCells.Add(new Vector2Int());
                }
            }

            bool changed = false;
            for (int i = 0; i < positions.Length; i++)
            {
                Vector2Int temp = GetCellIndex(positions[i]);
                if (currentCells[i] != temp)
                {
                    currentCells[i] = temp;
                    changed = true;
                }
            }

            if (changed)
            {
                activeCells = new List<Vector2Int>();
                for (int i = 0; i < positions.Length; i++)
                {
                    activeCells.AddRange(GetCellNeighbors(currentCells[i].x, currentCells[i].y, level, false));
                }
                UpdateActiveIntersections();
            }
        }
#endif

        /// <summary>
        /// Returns a waypoint
        /// </summary>
        /// <param name="row">grid row</param>
        /// <param name="column">grid column</param>
        /// <param name="depth">haw far from the current cell should be the selected cell</param>
        /// <param name="carType"></param>
        /// <returns></returns>
        public int GetNeighborCellWaypoint(int row, int column, int depth, VehicleTypes carType, Vector3 playerPosition, Vector3 playerDirection)
        {
            //get all cell neighbors for the specified depth
            List<Vector2Int> neighbors = GetCellNeighbors(row, column, depth, true);
            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                if (grid[neighbors[i].x].row[neighbors[i].y].spawnWaypoints.Count == 0)
                {
                    neighbors.RemoveAt(i);
                }
            }
            //TODO This should be done inside waypoint manager
            //if neighbors exists
            if (neighbors.Count > 0)
            {
                try
                {
                    return SpawnWaypointSelector(neighbors, playerPosition, playerDirection, carType);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Your neighbor selector method has the following error: " + e.Message);
                    return GetBestNeighbor.GetRandomSpawnWaypoint(neighbors, playerPosition, playerDirection, carType);
                }
            }
            return -1;
        }


        /// <summary>
        /// Get all specified neighbors for the specified depth
        /// </summary>
        /// <param name="row">current row</param>
        /// <param name="column">current column</param>
        /// <param name="depth">how far the cells should be</param>
        /// <param name="justEdgeCells">ignore middle cells</param>
        /// <returns>Returns the neighbors of the given cells</returns>
        public List<Vector2Int> GetCellNeighbors(int row, int column, int depth, bool justEdgeCells)
        {
            List<Vector2Int> result = new List<Vector2Int>();

            int rowMinimum = row - depth;
            if (rowMinimum < 0)
            {
                rowMinimum = 0;
            }

            int rowMaximum = row + depth;
            if (rowMaximum >= grid.Length)
            {
                rowMaximum = grid.Length - 1;
            }


            int columnMinimum = column - depth;
            if (columnMinimum < 0)
            {
                columnMinimum = 0;
            }

            int columnMaximum = column + depth;
            if (columnMaximum >= grid[row].row.Length)
            {
                columnMaximum = grid[row].row.Length - 1;
            }
            for (int i = rowMinimum; i <= rowMaximum; i++)
            {
                for (int j = columnMinimum; j <= columnMaximum; j++)
                {
                    if (justEdgeCells)
                    {
                        if (i == row + depth || i == row - depth || j == column + depth || j == column - depth)
                        {
                            result.Add(new Vector2Int(i, j));
                        }
                    }
                    else
                    {
                        result.Add(new Vector2Int(i, j));
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Convert indexes to Grid cell
        /// </summary>
        /// <param name="xPoz"></param>
        /// <param name="zPoz"></param>
        /// <returns></returns>
        public GridCell GetCell(float xPoz, float zPoz)
        {
            int rowIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.z - zPoz) / gridCellSize));
            int columnIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.x - xPoz) / gridCellSize));
            return grid[rowIndex].row[columnIndex];
        }


        /// <summary>
        /// Convert position to Grid cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GridCell GetCell(Vector3 position)
        {
            return GetCell(position.x, position.z);
        }


        /// <summary>
        /// Convert cell index to Grid cell
        /// </summary>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public GridCell GetCell(Vector2Int cellIndex)
        {
            return grid[cellIndex.x].row[cellIndex.y];
        }

        public Vector3 GetCellPosition(Vector2Int cellIndex)
        {
            return GetCell(cellIndex).center;
        }

#if USE_GLEY_TRAFFIC
        /// <summary>
        /// Convert position to cell index
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector2Int GetCellIndex(Vector3 position)
        {
            int rowIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.z - position.z) / gridCellSize));
            int columnIndex = Mathf.FloorToInt(Mathf.Abs((gridCorner.x - position.x) / gridCellSize));
            return new Vector2Int(grid[rowIndex].row[columnIndex].row, grid[rowIndex].row[columnIndex].column);
        }
#endif

        #region Intersections
#if USE_GLEY_TRAFFIC
        /// <summary>
        /// Get active all intersections 
        /// </summary>
        /// <returns></returns>
        public List<GenericIntersection> GetActiveIntersections()
        {
            return activeIntersections;
        }


        /// <summary>
        /// Create a list of active intersections
        /// </summary>
        private void UpdateActiveIntersections()
        {
            List<int> intersectionIndexes = new List<int>();
            for (int i = 0; i < activeCells.Count; i++)
            {
                intersectionIndexes.AddRange(GetCell(activeCells[i]).intersectionsInCell.Except(intersectionIndexes));
            }

            List<GenericIntersection> result = new List<GenericIntersection>();
            for (int i = 0; i < intersectionIndexes.Count; i++)
            {
                switch (allIntersections[intersectionIndexes[i]].type)
                {
                    case IntersectionType.TrafficLights:
                        result.Add(allLightsIntersections[allIntersections[intersectionIndexes[i]].index]);
                        break;
                    case IntersectionType.Priority:
                        result.Add(allPriorityIntersections[allIntersections[intersectionIndexes[i]].index]);
                        break;
                }
            }

            if (activeIntersections.Count == result.Count && activeIntersections.All(result.Contains))
            {

            }
            else
            {
                activeIntersections = result;
                IntersectionEvents.TriggetActiveIntersectionsChangedEvent(activeIntersections);
            }
        }


        /// <summary>
        /// Return all intersections
        /// </summary>
        /// <returns></returns>
        public GenericIntersection[] GetAllIntersections()
        {
            GenericIntersection[] result = new GenericIntersection[allIntersections.Length];
            for (int i = 0; i < allIntersections.Length; i++)
            {
                switch (allIntersections[i].type)
                {
                    case IntersectionType.TrafficLights:
                        result[i] = allLightsIntersections[allIntersections[i].index];
                        break;
                    case IntersectionType.Priority:
                        result[i] = allPriorityIntersections[allIntersections[i].index];
                        break;
                }
            }
            return result;
        }
#endif
        #endregion
    }
}