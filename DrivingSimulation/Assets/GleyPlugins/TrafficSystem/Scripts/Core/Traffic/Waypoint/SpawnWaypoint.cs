using System.Collections.Generic;

namespace GleyTrafficSystem
{
    /// <summary>
    /// This type of waypoint can spawn a vehicle, 
    /// used to store waypoint properties 
    /// </summary>
    [System.Serializable]
    public struct SpawnWaypoint
    {
        public int waypointIndex;
        public List<VehicleTypes> allowedVehicles;
        public SpawnWaypoint(int waypointIndex, List<VehicleTypes> allowedVehicles)
        {
            this.waypointIndex = waypointIndex;
            this.allowedVehicles = allowedVehicles;
        }
    }
}
