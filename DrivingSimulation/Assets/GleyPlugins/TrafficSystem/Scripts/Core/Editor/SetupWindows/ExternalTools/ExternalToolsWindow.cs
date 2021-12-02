using UnityEditor;
using UnityEngine;
namespace GleyTrafficSystem
{
    public class ExternalToolsWindow : SetupWindowBase
    {
        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.Space();
            if (GUILayout.Button("Easy Roads"))
            {
                SettingsWindow.SetActiveWindow(WindowType.EasyRoadsSetup, true);
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Cidy 2"))
            {
                SettingsWindow.SetActiveWindow(WindowType.CidySetup, true);
            }
            EditorGUILayout.Space();
        }
    }
}
