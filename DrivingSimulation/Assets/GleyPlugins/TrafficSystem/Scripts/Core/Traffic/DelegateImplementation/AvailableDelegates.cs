using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Use this delegate to create your own spawn waypoint selection
    /// </summary>
    /// <param name="neighbors">list of available squares</param>
    /// <param name="position">position of the player</param>
    /// <param name="direction">heading direction of the player</param>
    /// <returns>an available waypoint to spawn a vehicle</returns>
    public delegate int SpawnWaypointSelector(List<Vector2Int> neighbors, Vector3 position, Vector3 direction, VehicleTypes carType);


    public delegate SpecialDriveActionTypes EnvironmentInteraction();


    public delegate void TrafficLightsBehaviour(TrafficLightsColor currentRoadColor, List<GameObject> redLightObjects, List<GameObject> yellowLightObjects, List<GameObject> greenLightObjects, string name);
}
