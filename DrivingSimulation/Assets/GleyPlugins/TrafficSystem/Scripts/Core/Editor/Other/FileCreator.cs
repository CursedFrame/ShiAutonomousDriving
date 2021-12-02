using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace GleyTrafficSystem
{
    public class FileCreator
    {
        public static void CreateVehicleTypesFile(List<string> carCategories)
        {
            if (carCategories == null)
            {
                carCategories = new List<string>();
                var allCarTypes = Enum.GetValues(typeof(VehicleTypes)).Cast<VehicleTypes>();
                foreach (VehicleTypes car in allCarTypes)
                {
                    carCategories.Add(car.ToString());
                }
            }

            CreateFolder("Assets/GleyPlugins/TrafficSystem/Resources");

            string text =
            "#if USE_GLEY_TRAFFIC\n" +
            "namespace GleyTrafficSystem\n" +
            "{\n" +
            "\tpublic enum VehicleTypes\n" +
            "\t{\n";
            for (int i = 0; i < carCategories.Count; i++)
            {
                text += "\t\t" + carCategories[i] + ",\n";
            }
            text += "\t}\n" +
                "}\n" +
                "#endif";

            File.WriteAllText(Application.dataPath + "/GleyPlugins/TrafficSystem/Resources/VehicleTypes.cs", text);
            Gley.Common.PreprocessorDirective.AddToCurrent(Gley.Common.Constants.USE_GLEY_TRAFFIC, false);

            AssetDatabase.Refresh();
        }

        public static void CreateFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] folders = path.Split('/');
                string tempPath = "";
                for (int i = 0; i < folders.Length - 1; i++)
                {
                    tempPath += folders[i];
                    if (!AssetDatabase.IsValidFolder(tempPath + "/" + folders[i + 1]))
                    {
                        AssetDatabase.CreateFolder(tempPath, folders[i + 1]);
                        AssetDatabase.Refresh();
                    }
                    tempPath += "/";
                }
            }
        }
    }
}
