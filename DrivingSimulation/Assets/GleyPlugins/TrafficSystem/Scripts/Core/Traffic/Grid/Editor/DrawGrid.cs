namespace GleyTrafficSystem
{
    /// <summary>
    /// Draw grid in editor
    /// </summary>
    public class DrawGrid
    {
        public static void Draw(GridRow[] grid)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                for (int j = 0; j < grid[i].row.Length; j++)
                {
                    DrawGridCell.Draw(grid[i].row[j]);
                }
            }
        }
    }
}