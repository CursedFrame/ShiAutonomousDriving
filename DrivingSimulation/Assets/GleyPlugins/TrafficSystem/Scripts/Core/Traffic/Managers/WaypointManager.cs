using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Performs waypoint operations
    /// </summary>
    public class WaypointManager : MonoBehaviour
    {
        private PositionValidator positionValidator;
        private CurrentSceneData waypointsGrid;
        private Waypoint[] allWaypoints;
        private List<Waypoint> disabledWaypoints;

        private int[] target;//contains the car index that has the waypoint as current waypoint
        private bool debugWaypoints;
        private bool debugDisabledWaypoints;


        /// <summary>
        /// Initialize waypoints
        /// </summary>
        /// <param name="positionValidator"></param>
        /// <param name="waypointsGrid"></param>
        /// <param name="nrOfVehicles"></param>
        /// <param name="debugWaypoints"></param>
        /// <param name="debugDisabledWaypoints"></param>
        /// <returns></returns>
        public WaypointManager Initialize(PositionValidator positionValidator, CurrentSceneData waypointsGrid, int nrOfVehicles, bool debugWaypoints, bool debugDisabledWaypoints)
        {
            this.waypointsGrid = waypointsGrid;
            this.debugWaypoints = debugWaypoints;
            this.debugDisabledWaypoints = debugDisabledWaypoints;
            this.positionValidator = positionValidator;
            allWaypoints = waypointsGrid.allWaypoints;
            disabledWaypoints = new List<Waypoint>();
            target = new int[nrOfVehicles];
            WaypointEvents.onStopIndicatorChanged += ChangeStopValue;
            return this;
        }


        #region NextWaypoint
        //TODO Methods from this region can be simplified


        /// <summary>
        /// Directly set the target waypoint for the vehicle at index.
        /// Used to set first waypoint after vehicle initialization
        /// </summary>
        /// <param name="index"></param>
        /// <param name="freeWaypointIndex"></param>
        public void SetTargetWaypoint(int index, int freeWaypointIndex)
        {
            GetWaypoint(target[index]).Passed(index);
            target[index] = freeWaypointIndex;
        }


        /// <summary>
        /// Set next waypoint for the vehicle index
        /// </summary>
        /// <param name="index">vehicle index</param>
        /// <param name="vehicleType">vehicle type</param>
        /// <param name="blink">blink required</param>
        /// <returns>true if waypoint was changed</returns>
        public int GetCurrentLaneWaypointIndex(int index, VehicleTypes vehicleType)
        {
            int waypointIndex = -1;
            Waypoint oldWaypoint = GetWaypoint(target[index]);

            //if waypoint has multiple neighbors it might be possible to change lanes
            //bool possibleLaneChange = false;
            //if (oldWaypoint.neighbors.Count > 1)
            //{
            //    possibleLaneChange = true;
            //}
            //oldWaypoint.Passed();

            //check direct neighbors
            if (oldWaypoint.neighbors.Count > 0)
            {
                Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.neighbors).Where(cond => cond.allowedCars.Contains(vehicleType) && cond.temporaryDisabled == false).ToArray();
                if (possibleWaypoints.Length > 0)
                {
                    waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                }
            }

            //check other lanes
            if (waypointIndex == -1)
            {
                if (oldWaypoint.otherLanes.Count > 0)
                {
                    Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.otherLanes).Where(cond => cond.allowedCars.Contains(vehicleType) && cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            //check neighbors that are not allowed
            if (waypointIndex == -1)
            {
                if (oldWaypoint.neighbors.Count > 0)
                {
                    Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.neighbors).Where(cond => cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            //check other lanes that are not allowed
            if (waypointIndex == -1)
            {
                if (oldWaypoint.otherLanes.Count > 0)
                {
                    Waypoint[] possibleWaypoints = GetAllWaypoints(oldWaypoint.otherLanes).Where(cond => cond.temporaryDisabled == false).ToArray();
                    if (possibleWaypoints.Length > 0)
                    {
                        waypointIndex = possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                    }
                }
            }

            //Waypoint newWaypoint = GetWaypoint(target[index]);
            ////if waypoint changed
            //if (returnValue == true)
            //{
            //    if (newWaypoint.stop == true || newWaypoint.giveWay == true)
            //    {
            //        WaypointEvents.TriggerWaypointStateChangedEvent(index, newWaypoint.stop, newWaypoint.giveWay);
            //    }
            //}
            //stop blinking


            //if (blink == false)
            //{
            //    Blink(possibleLaneChange, index, oldWaypoint, newWaypoint, Vector3.zero, Vector3.zero);
            //}
            return waypointIndex;
        }


        public bool IsInIntersection(int index)
        {
            return GetWaypoint(target[index]).IsInIntersection();
        }


        public bool CanEnterIntersection(int index)
        {
            return GetWaypoint(target[index]).CanChange();
        }


        /// <summary>
        /// Check if a change of lane is possible
        /// Used to overtake and give way
        /// </summary>
        /// <param name="index"></param>
        /// <param name="vehicleType"></param>
        /// <param name="freeDistanceToCheck"></param>
        /// <returns></returns>
        public int GetOtherLaneWaypointIndex(int index, VehicleTypes vehicleType)
        {
            Waypoint[] possibleWaypoints = new Waypoint[0];
            Waypoint currentWaypoint = GetWaypoint(target[index]);

            if (currentWaypoint.otherLanes.Count > 0)
            {
                possibleWaypoints = GetAllWaypoints(currentWaypoint.otherLanes).Where(cond => cond.allowedCars.Contains(vehicleType)).ToArray();
                if (possibleWaypoints.Length > 0)
                {
                    return possibleWaypoints[Random.Range(0, possibleWaypoints.Length)].listIndex;
                }
                return currentWaypoint.otherLanes[Random.Range(0, currentWaypoint.otherLanes.Count)];
            }

            return -1;
        }



        /// <summary>
        /// Convert list index to play waypoint
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Waypoint[] GetAllWaypoints(List<int> index)
        {
            Waypoint[] result = new Waypoint[index.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetWaypoint(index[i]);
            }
            return result;
        }


        /// <summary>
        /// Check if the previous waypoints are free
        /// </summary>
        /// <param name="index"></param>
        /// <param name="freeWaypointsNeeded"></param>
        /// <param name="possibleWaypoints"></param>
        /// <returns></returns>
        public bool AllPreviousWaypointsAreFree(int index, int freeWaypointsNeeded, int waypointToCheck)
        {
            return IsTargetFree(GetWaypoint(waypointToCheck), freeWaypointsNeeded, GetWaypoint(target[index]), index);
        }


        /// <summary>
        /// Check if previous waypoints are free
        /// </summary>
        /// <param name="waypoint"></param>
        /// <param name="level"></param>
        /// <param name="initialWaypoint"></param>
        /// <returns></returns>
        private bool IsTargetFree(Waypoint waypoint, int level, Waypoint initialWaypoint, int currentCarIndex)
        {
#if UNITY_EDITOR
            if (debugWaypoints)
            {
                Debug.DrawLine(waypoint.position, initialWaypoint.position, Color.green, 1);
            }
#endif
            if (level == 0)
            {
                return true;
            }
            if (waypoint == initialWaypoint)
            {
                return true;
            }
            if (target.Contains(waypoint.listIndex))
            {
                if (target[currentCarIndex] == waypoint.listIndex)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (waypoint.prev.Count <= 0)
                {
                    return true;
                }
                level--;
                for (int i = 0; i < waypoint.prev.Count; i++)
                {
                    if (!IsTargetFree(GetWaypoint(waypoint.prev[i]), level, initialWaypoint, currentCarIndex))
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Set a waypoint as target
        /// </summary>
        /// <param name="index"></param>
        /// <param name="waypoint"></param>
        public void SetNextWaypoint(int index, int waypointIndex)
        {
            SetTargetWaypoint(index, waypointIndex);
            Waypoint targetWaypoint = GetWaypoint(target[index]);
            if (targetWaypoint.stop == true || targetWaypoint.giveWay == true)
            {
                WaypointEvents.TriggerWaypointStateChangedEvent(index, targetWaypoint.stop, targetWaypoint.giveWay);
            }
        }

        public int GetCurrentCarWaypointIndex(int index)
        {
            return target[index];
        }
        #endregion


        /// <summary>
        /// Converts index to waypoint
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Waypoint GetWaypoint(int index)
        {
            return allWaypoints[index];
        }


        /// <summary>
        /// Check what vehicle is in front
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns>
        /// 1-> if 1 is in front of 2
        /// 2-> if 2 is in front of 1
        /// 0-> if it is not possible to determine
        /// </returns>
        public int IsInFront(int index1, int index2)
        {
            //compares waypoints to determine which vehicle is in front 
            int distance = 0;
            //if no neighbors are available -> not possible to determine
            if (GetWaypoint(target[index1]).neighbors.Count == 0)
            {
                return 0;
            }

            //check next 10 waypoints to find waypoint 2
            int startWaypointIndex = GetWaypoint(target[index1]).neighbors[0];
            while (startWaypointIndex != target[index2] && distance < 10)
            {
                distance++;
                if (GetWaypoint(startWaypointIndex).neighbors.Count == 0)
                {
                    //if not found -> not possible to determine
                    return 0;
                }
                startWaypointIndex = GetWaypoint(startWaypointIndex).neighbors[0];
            }


            int distance2 = 0;
            if (GetWaypoint(target[index2]).neighbors.Count == 0)
            {
                return 0;
            }

            startWaypointIndex = GetWaypoint(target[index2]).neighbors[0];
            while (startWaypointIndex != target[index1] && distance2 < 10)
            {
                distance2++;
                if (GetWaypoint(startWaypointIndex).neighbors.Count == 0)
                {
                    //if not found -> not possible to determine
                    return 0;
                }
                startWaypointIndex = GetWaypoint(startWaypointIndex).neighbors[0];
            }

            //if no waypoints found -> not possible to determine
            if (distance == 10 && distance2 == 10)
            {
                return 0;
            }

            if (distance2 > distance)
            {
                return 2;
            }

            return 1;
        }


        /// <summary>
        /// Check if 2 vehicles have the same target
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public bool IsSameTarget(int index1, int index2)
        {
            return target[index1] == target[index2];
        }


        /// <summary>
        /// Get a random free waypoint from a grid cell
        /// </summary>
        /// <param name="firstTime"></param>
        /// <param name="currentGridRow"></param>
        /// <param name="currentGridColumn"></param>
        /// <param name="carType"></param>
        /// <param name="halfCarLength"></param>
        /// <param name="halfCarHeight"></param>
        /// <returns></returns>
        public int GetFreeWaypointToInstantiate(bool firstTime, int currentGridRow, int currentGridColumn, VehicleTypes carType, float halfCarLength, float halfCarHeight, float halfCarWidth, float frontWheelOffset, Vector3 playerPosition, Vector3 playerDirection)
        {
            int freeWaypointIndex;

            //get a free waypoint with the specified characteristics
            if (firstTime)
            {
                freeWaypointIndex = waypointsGrid.GetNeighborCellWaypoint(currentGridRow, currentGridColumn, Random.Range(0, 2), carType, playerPosition, playerDirection);
            }
            else
            {
                freeWaypointIndex = waypointsGrid.GetNeighborCellWaypoint(currentGridRow, currentGridColumn, 1, carType, playerPosition, playerDirection);
            }

            if (freeWaypointIndex != -1)
            {
                //if a valid waypoint was found, check if it was not manually disabled
                if (GetWaypoint(freeWaypointIndex).temporaryDisabled)
                {
                    return -1;
                }

                //check if the car type can be instantiated on selected waypoint
                if (positionValidator.IsValid(GetWaypoint(freeWaypointIndex).position, halfCarLength, halfCarHeight, halfCarWidth, firstTime, frontWheelOffset, GetOrientation(freeWaypointIndex)))
                {
                    return freeWaypointIndex;
                }
            }

            return -1;
        }

        internal bool IsWaypointFree(int waypointIndex, float halfCarLength, float halfCarHeight, float halfCarWidth, float frontWheelOffset)
        {
            return positionValidator.IsValid(GetWaypoint(waypointIndex).position, halfCarLength, halfCarHeight, halfCarWidth, true, frontWheelOffset, GetOrientation(waypointIndex));
        }

        internal int GetClosestWayoint(Vector3 position, VehicleTypes type)
        {
            List<SpawnWaypoint> possibleWaypoints = waypointsGrid.GetCell(position.x, position.z).spawnWaypoints.Where(cond1 => cond1.allowedVehicles.Contains(type)).ToList();

            if (possibleWaypoints.Count == 0)
                return -1;

            float distance = float.MaxValue;
            int waypointIndex = -1;
            for (int i = 0; i < possibleWaypoints.Count; i++)
            {
                float newDistance = Vector3.SqrMagnitude(GetWaypoint(possibleWaypoints[i].waypointIndex).position - position);
                if (newDistance < distance)
                {
                    distance = newDistance;
                    waypointIndex = possibleWaypoints[i].waypointIndex;
                }
            }
            return waypointIndex;
        }


        /// <summary>
        /// Get position of the target waypoint
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetTargetPosition(int index)
        {
            return GetWaypoint(target[index]).position;
        }


        /// <summary>
        /// Get rotation of the target waypoint
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Quaternion GetTargetRotation(int index)
        {
            if (GetWaypoint(target[index]).neighbors.Count == 0)
            {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(GetWaypoint(GetWaypoint(target[index]).neighbors[0]).position - GetWaypoint(target[index]).position);
        }


        Quaternion GetOrientation(int index)
        {
            if (GetWaypoint(index).neighbors.Count == 0)
            {
                return Quaternion.identity;
            }
            return Quaternion.LookRotation(GetWaypoint(GetWaypoint(index).neighbors[0]).position - GetWaypoint(index).position);
        }

        /// <summary>
        /// Get waypoint speed
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float GetMaxSpeed(int index)
        {
            return GetWaypoint(target[index]).maxSpeed;
        }


        /// <summary>
        /// Remove target waypoint for the vehicle at index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveTargetWaypoint(int index)
        {
            GetWaypoint(target[index]).Passed(index);
            target[index] = 0;
        }


        /// <summary>
        /// Converts distance to waypoint number
        /// </summary>
        /// <param name="index"></param>
        /// <param name="lookDistance"></param>
        /// <returns></returns>
        public int GetNrOfWaypointsToCheck(int index, float lookDistance)
        {
            Waypoint currentWaypoint = GetWaypoint(target[index]);
            float waypointDistance = 4;
            if (currentWaypoint.neighbors.Count > 0)
            {
                waypointDistance = Vector3.Distance(currentWaypoint.position, GetWaypoint(currentWaypoint.neighbors[0]).position);
            }
            return Mathf.CeilToInt(lookDistance / waypointDistance);
        }


        /// <summary>
        /// Switch the stop value of a waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        private void ChangeStopValue(int waypointIndex, bool newValue)
        {
            if (allWaypoints[waypointIndex].stop != newValue)
            {
                allWaypoints[waypointIndex].stop = newValue;
                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i] == waypointIndex)
                    {
                        WaypointEvents.TriggerWaypointStateChangedEvent(i, GetWaypoint(waypointIndex).stop, GetWaypoint(waypointIndex).giveWay);
                    }
                }
            }
        }


        /// <summary>
        /// Update the camera for position validator
        /// </summary>
        /// <param name="activeCameras"></param>
        public void UpdateCamera(Transform[] activeCameras)
        {
            positionValidator.UpdateCamera(activeCameras);
        }


        /// <summary>
        /// Makes waypoints on a given radius unavailable
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void DisableAreaWaypoints(Vector3 center, float radius)
        {
            GridCell cell = waypointsGrid.GetCell(center);
            List<Vector2Int> neighbors = waypointsGrid.GetCellNeighbors(cell.row, cell.column, 1, false);
            for (int i = neighbors.Count - 1; i >= 0; i--)
            {
                cell = waypointsGrid.GetCell(neighbors[i]);
                for (int j = 0; j < cell.waypointsInCell.Count; j++)
                {
                    Waypoint waypoint = GetWaypoint(cell.waypointsInCell[j]);
                    if (Vector3.SqrMagnitude(center - waypoint.position) < radius)
                    {
                        disabledWaypoints.Add(waypoint);
                        waypoint.temporaryDisabled = true;
                    }
                }
            }
        }


        /// <summary>
        /// Enables unavailable waypoints
        /// </summary>
        public void EnableAllWaypoints()
        {
            for (int i = 0; i < disabledWaypoints.Count; i++)
            {
                disabledWaypoints[i].temporaryDisabled = false;
            }
            disabledWaypoints = new List<Waypoint>();
        }


        /// <summary>
        /// Cleanup
        /// </summary>
        private void OnDestroy()
        {
            WaypointEvents.onStopIndicatorChanged -= ChangeStopValue;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (debugWaypoints)
            {
                for (int i = 0; i < target.Length; i++)
                {
                    if (target[i] != 0)
                    {
                        Gizmos.color = Color.green;
                        Vector3 position = GetWaypoint(target[i]).position;
                        Gizmos.DrawSphere(position, 1);
                        position.y += 1.5f;
                        UnityEditor.Handles.Label(position, i.ToString());
                    }
                }
            }
            if (debugDisabledWaypoints)
            {
                for (int i = 0; i < disabledWaypoints.Count; i++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(disabledWaypoints[i].position, 1);
                }
            }
        }
#endif
    }
}