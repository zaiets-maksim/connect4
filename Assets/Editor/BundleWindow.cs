using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Editor
{
    public class BundleWindow : EditorWindow
    {
        private ReorderableList _reorderableList;
        private readonly List<Object> _objectList = new List<Object>();
        private readonly string _assetBundlesPath = Application.streamingAssetsPath + "/AssetBundles/";
        private string _bundleName = "bundle";

        [MenuItem("Tools/Bundle Manager")]
        static void OpenWindow()
        {
            BundleWindow window = GetWindow<BundleWindow>();
            window.titleContent = new GUIContent("Bundle Manager");
            window.LoadData();
            window.Show();
        }

        private void OnEnable()
        {
            ShowListOfObjects();
        }

        private void ShowListOfObjects()
        {
            _reorderableList = new ReorderableList(_objectList, typeof(Object), true, true, true, true);

            _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Object obj = _objectList[index];

                rect.height = EditorGUIUtility.singleLineHeight;
                rect.width = 150f;
                _objectList[index] = EditorGUI.ObjectField(rect, _objectList[index], typeof(Object), true);

                rect.x += rect.width + 5;
                rect.width = EditorGUIUtility.labelWidth;

                string typeName = obj ? obj.GetType().Name : "No object selected";
                GUI.Label(rect, "Type: " + typeName);

                rect.x += EditorGUIUtility.labelWidth + 5;
                rect.width = EditorGUIUtility.labelWidth;

                long fileSize = obj ? GetFileSize(obj) : 0;
                GUI.Label(rect, "Size: " + FormatSize(fileSize));
            };

            _reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Object List");
            };
        }

        private void OnGUI()
        {
            _reorderableList.DoLayoutList();
            _bundleName = EditorGUILayout.TextField("Enter Bundle Name:", _bundleName);

            if (GUILayout.Button("Build bundle")) 
                BuildBundle();

            if (Event.current.type == EventType.Layout)
                SaveData();
        }

        private void BuildBundle()
        {
            if (!Directory.Exists(_assetBundlesPath))
                Directory.CreateDirectory(_assetBundlesPath);

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

            List<string> assetPaths = new List<string>();
            foreach (Object obj in _objectList)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                assetPaths.Add(assetPath);
            }

            buildMap[0] = new AssetBundleBuild
            {
                assetBundleName = _bundleName,
                assetNames = assetPaths.ToArray()
            };

            BuildPipeline.BuildAssetBundles(_assetBundlesPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
        }

        private long GetFileSize(Object obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            FileInfo fileInfo = new FileInfo(Application.dataPath + "/../" + assetPath);
            return fileInfo.Length;
        }

        private string FormatSize(long fileSize)
        {
            string[] sizes = {"B", "KB", "MB", "GB", "TB"};
            int order = 0;
        
            while (fileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                fileSize >>= 10;
            }

            return $"{fileSize:0.##} {sizes[order]}";
        }

        private void SaveData()
        {
            string serializedData = string.Join(",", _objectList.ConvertAll(o => o ? AssetDatabase.GetAssetPath(o) : ""));
            EditorPrefs.SetString("ObjectListData", serializedData);
        }

        private void LoadData()
        {
            string serializedData = EditorPrefs.GetString("ObjectListData", "");
            string[] assetPaths = serializedData.Split(',');

            _objectList.Clear();

            foreach (string assetPath in assetPaths)
            {
                if (!string.IsNullOrEmpty(assetPath))
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                    if (obj != null)
                        _objectList.Add(obj);
                }
            }
        }
    }
}