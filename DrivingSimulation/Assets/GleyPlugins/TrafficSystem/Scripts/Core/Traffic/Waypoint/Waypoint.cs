using System.Collections.Generic;
using UnityEngine;

namespace GleyTrafficSystem
{
    [System.Serializable]
    public class Waypoint
    {
        public List<VehicleTypes> allowedCars;
        public List<int> neighbors;
        public List<int> prev;
        public List<int> otherLanes;
        public Vector3 position;
        public string name;
        public int listIndex;
        public int maxSpeed;
        public bool stop;
        public bool giveWay;
        public bool enter;
        public bool exit;
        public bool temporaryDisabled;

        private IIntersection associatedIntersection;


        /// <summary>
        /// Constructor used to convert from editor waypoint to runtime waypoint 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="listIndex"></param>
        /// <param name="position"></param>
        /// <param name="allowedCars"></param>
        /// <param name="neighbors"></param>
        /// <param name="prev"></param>
        /// <param name="otherLanes"></param>
        /// <param name="maxSpeed"></param>
        /// <param name="stop"></param>
        /// <param name="giveWay"></param>
        /// <param name="enter"></param>
        /// <param name="exit"></param>
        public Waypoint(string name,
            int listIndex,
            Vector3 position,
            List<VehicleTypes> allowedCars,
            List<int> neighbors,
            List<int> prev,
            List<int> otherLanes,
            int maxSpeed,
            bool giveWay)
        {
            this.name = name;
            this.listIndex = listIndex;
            this.position = position;
            this.allowedCars = allowedCars;
            this.neighbors = neighbors;
            this.prev = prev;
            this.otherLanes = otherLanes;
            this.maxSpeed = maxSpeed;
            this.stop = false;
            this.giveWay = giveWay;
            this.enter = false;
            this.exit = false;
            if (neighbors.Count == 0)
            {
#if !DEBUG_TRAFFIC
                this.giveWay = true;
#endif
            }
            temporaryDisabled = false;
        }


        public Waypoint()
        {

        }

        /// <summary>
        /// Initializes current waypoint properties
        /// Used by intersections
        /// </summary>
        /// <param name="intersection"></param>
        /// <param name="giveWay"></param>
        /// <param name="stop"></param>
        /// <param name="enter"></param>
        /// <param name="exit"></param>
        public void SetIntersection(IIntersection intersection, bool giveWay, bool stop, bool enter, bool exit)
        {
            associatedIntersection = intersection;
            this.stop = stop;
            this.giveWay = giveWay;
            this.exit = exit;
            this.enter = enter;
        }


        /// <summary>
        /// Check if the waypoint is free
        /// </summary>
        /// <returns>true if intersection allows passing through this waypoint</returns>
        public bool CanChange()
        {
            return associatedIntersection.IsPathFree(listIndex);
        }


        /// <summary>
        /// Check if the waypoint belongs to an intersection
        /// </summary>
        /// <returns></returns>
        public bool IsInIntersection()
        {
            return associatedIntersection != null;
        }


        /// <summary>
        /// Waypoint is no longer a target for the vehicle
        /// </summary>
        public void Passed(int vehicleIndex)
        {
            if (exit)
            {
                associatedIntersection.VehicleLeft(vehicleIndex);
                return;
            }
            if (enter)
            {
                associatedIntersection.VehicleEnter(vehicleIndex);
                return;
            }
        }
    }
}