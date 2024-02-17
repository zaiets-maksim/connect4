using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Services.BundleLoader
{
    public class BundleLoader : IBundleLoader
    {
        private readonly string _assetBundlesPath = Application.streamingAssetsPath + "/AssetBundles/";

        public List<T> LoadBundle<T> (string name)
        {
            List<T> resources = new List<T>();

            AssetBundle assetBundle = AssetBundle.LoadFromFile(_assetBundlesPath + name);

            if (assetBundle == null)
            {
                Debug.LogError("Failed to load Asset Bundle: " + name);
                return null;
            }

            Object[] assets = assetBundle.LoadAllAssets();

            foreach (Object asset in assets)
                if (asset is T t)
                    resources.Add(t);

            assetBundle.Unload(false);

            return resources;
        }

        public T LoadAsset<T>(string bundleName, string assetName) where T : Object
        {
            string bundlePath = _assetBundlesPath + bundleName;

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
            {
                Debug.LogError("Failed to load Asset Bundle from file: " + bundlePath);
                return null;
            }

            T loadedObject = bundle.LoadAsset<T>(assetName);
            bundle.Unload(false);

            if (loadedObject != null)
                return loadedObject;
            
            Debug.LogError("Failed to load asset from Asset Bundle.");

            return null;
        }
    }

    public interface IBundleLoader
    {
        List<T> LoadBundle<T>(string name);
        T LoadAsset<T>(string bundleName, string assetName) where T : Object;
    }
}