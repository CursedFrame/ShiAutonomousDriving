using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class LayerOperations
    {
        public static LayerSetup LoadOrCreateLayers()
        {
            LayerSetup layerSetup = (LayerSetup)AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/TrafficSystem/Resources/LayerSetupData.asset", typeof(LayerSetup));
            if (layerSetup == null)
            {
                LayerSetup asset = ScriptableObject.CreateInstance<LayerSetup>();
                FileCreator.CreateFolder("Assets/GleyPlugins/TrafficSystem/Resources");
                AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/TrafficSystem/Resources/LayerSetupData.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                layerSetup = (LayerSetup)AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/TrafficSystem/Resources/LayerSetupData.asset", typeof(LayerSetup));
            }

            return layerSetup;
        }


        public static LayerMask LoadRoadLayers()
        {
            LayerSetup layerSetup = LoadOrCreateLayers();
            return layerSetup.roadLayers;
        }
    }
}
