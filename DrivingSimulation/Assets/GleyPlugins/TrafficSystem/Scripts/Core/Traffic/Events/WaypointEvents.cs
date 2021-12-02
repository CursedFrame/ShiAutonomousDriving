namespace GleyTrafficSystem
{
    public static class WaypointEvents
    {
        /// <summary>
        /// Triggered to change the stop value of the waypoint
        /// </summary>
        /// <param name="waypointIndex"></param>
        public delegate void StopIndicatorChanged(int waypointIndex, bool stop);
        public static event StopIndicatorChanged onStopIndicatorChanged;
        public static void TriggerStopIndicatorChangedEvent(int waypointIndex, bool stop)
        {
            if (onStopIndicatorChanged != null)
            {
                onStopIndicatorChanged(waypointIndex, stop);
            }
        }


        /// <summary>
        /// Triggered to notify vehicle about stop state and give way state of the waypoint
        /// </summary>
        /// <param name="index">vehicle index</param>
        /// <param name="stopState">stop in point needed</param>
        /// <param name="giveWayState">give way needed</param>
        public delegate void WaypointStateChanged(int index, bool stopState, bool giveWayState);
        public static event WaypointStateChanged onWaypointStateChanged;
        public static void TriggerWaypointStateChangedEvent(int index, bool stopState, bool giveWayState)
        {
            if (onWaypointStateChanged != null)
            {
                onWaypointStateChanged(index, stopState, giveWayState);
            }
        }
    }
}
