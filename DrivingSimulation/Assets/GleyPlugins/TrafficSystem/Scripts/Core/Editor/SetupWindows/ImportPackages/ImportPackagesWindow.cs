using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class ImportPackagesWindow : SetupWindowBase
    {
        private string message;


        protected override void TopPart()
        {
            EditorGUILayout.LabelField("Required Packages:");
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Burst");
            if(GUILayout.Button("Install"))
            {
                ImportRequiredPackages.ImportPackages(UpdateMethod);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Install All"))
            {
                ImportRequiredPackages.ImportPackages(UpdateMethod);
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(message);

            base.TopPart();
        }


        private void UpdateMethod(string message)
        {
            this.message = message;
        }
    }
}
