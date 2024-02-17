using System.Collections.Generic;
using UnityEngine;

namespace Connect4.Scripts.Services.BundleLoader
{
    public class BundleLoader : IBundleLoader
    {
        private readonly string _assetBundlesPath = Application.streamingAssetsPath + "/AssetBundles/";
        
        public AssetBundle LoadBundle (string name)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(_assetBundlesPath + name);

            if (assetBundle == null)
            {
                Debug.LogError("Failed to load Asset Bundle: " + name);
                return null;
            }
            
            AssetBundle bundle = assetBundle;

            return bundle;
        }
        
        public List<T> LoadAssets<T> (string bundleName)
        {
            List<T> resources = new List<T>();

            var assetBundle = LoadBundle(bundleName);

            Object[] assets = assetBundle.LoadAllAssets();
            assetBundle.Unload(false);

            foreach (Object asset in assets)
                if (asset is T t)
                    resources.Add(t);

            return resources;
        }

        public T LoadAsset<T>(string bundleName, string assetName) where T : Object
        {
            var assetBundle = LoadBundle(bundleName);

            if (assetBundle == null)
            {
                Debug.LogError("Failed to load Asset Bundle from file: " + _assetBundlesPath);
                return null;
            }

            T loadedObject = assetBundle.LoadAsset<T>(assetName);
            assetBundle.Unload(false);

            if (loadedObject != null)
                return loadedObject;
            
            Debug.LogError("Failed to load asset from Asset Bundle.");
            
            return null;
        }
    }

    public interface IBundleLoader
    {
        AssetBundle LoadBundle(string name);
        List<T> LoadAssets<T>(string bundleName);
        T LoadAsset<T>(string bundleName, string assetName) where T : Object;
    }
}