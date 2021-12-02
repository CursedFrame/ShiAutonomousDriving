using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class GridSetupWindow : SetupWindowBase
    {
        private CurrentSceneData grid;
        private Color oldColor;
        private bool viewGrid;


        public override ISetupWindow Initialize(WindowProperties windowProperties)
        {
            grid = CurrentSceneData.GetSceneInstance();
            return base.Initialize(windowProperties);
        }


        public override void DrawInScene()
        {
            if (viewGrid)
            {
                DrawGrid.Draw(grid.grid);
            }
            base.DrawInScene();
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("The grid is used to improve the performance. Vehicles are generated in the cells adjacent to player cell.\n\n" +
                "The cell size should be smaller if your player speed is low and should increase if your speed is high.\n\n" +
                "You can experiment with this settings until you get the result you want.");
        }


        protected override void ScrollPart(float width, float height)
        {
            grid.gridCellSize = EditorGUILayout.IntField("Grid Cell Size: ", grid.gridCellSize);
            if (GUILayout.Button("Regenerate Grid"))
            {
                GridEditor.GenerateGrid(grid);
            }
            EditorGUILayout.Space();

           

            oldColor = GUI.backgroundColor;
            if (viewGrid == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Grid"))
            {
                viewGrid = !viewGrid;
                SceneView.RepaintAll();
            }
            GUI.backgroundColor = oldColor;
            base.ScrollPart(width, height);
        }
    }
}
