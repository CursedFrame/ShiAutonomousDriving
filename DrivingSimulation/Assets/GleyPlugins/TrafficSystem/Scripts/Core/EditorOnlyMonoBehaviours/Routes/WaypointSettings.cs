using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Stores waypoint properties
    /// </summary>
    public class WaypointSettings : MonoBehaviour
    {
        public List<VehicleTypes> allowedCars;
        public List<WaypointSettings> neighbors;
        public List<WaypointSettings> prev;
        public List<WaypointSettings> otherLanes;
        public ConnectionCurve connection;
        public int maxSpeed;
        public bool stop;
        public bool giveWay;
        public bool enter;
        public bool exit;
        public bool speedLocked;
        public bool carsLocked;
        public bool draw = true;

        public void EditorSetup()
        {
            neighbors = new List<WaypointSettings>();
            prev = new List<WaypointSettings>();
            otherLanes = new List<WaypointSettings>();
        }


        public void Initialize()
        {
        }


        public void SetText()
        {
            TextMesh text = transform.Find("New Text").GetComponent<TextMesh>();
            text.text = maxSpeed.ToString();
            for (int i = 0; i < allowedCars.Count; i++)
            {
                text.text += "\n " + allowedCars[i];
            }
        }
    }
}