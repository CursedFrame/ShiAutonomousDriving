using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    /// <summary>
    /// Draw a single grid cell in editor
    /// </summary>
    public class DrawGridCell
    {
        public static void Draw(GridCell gridCell)
        {
            Handles.color = Color.white;
            if (gridCell.waypointsInCell.Count != 0)
            {
                Handles.color = Color.green;
            }
            Handles.DrawWireCube(gridCell.center, gridCell.size);
            Handles.Label(gridCell.center - new Vector3(gridCell.size.x / 2 - 1, 0, 0), "r" + gridCell.row + " c" + gridCell.column);
        }
    }
}