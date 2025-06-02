#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
namespace Rift.Story
{
    public class RegistryWindow : EditorWindow
    {
        private Vector2 _scrollPos1;
        private Vector2 _scrollPos2;
        private List<CharacterData> _characterDataList = new List<CharacterData>();
        private List<OperationValueData> _operationValueDataList = new List<OperationValueData>();
        private Dictionary<EGlobalVariablesType, string> _targetPath;
        private GlobalStoryRegistry _registry;

        // 由父窗口调用时设置路径
        public void Init(Dictionary<EGlobalVariablesType, string> paths, GlobalStoryRegistry registry)
        {
            _targetPath = paths;
            _registry = registry;
            _characterDataList = _registry.CharacterDataList;
            _operationValueDataList = _registry.OperationValueDataList;
        }

        void OnGUI()
        {
            GUILayout.Label("注册表", EditorStyles.boldLabel);

            // Character Data列表
            GUILayout.Label("角色数据注册表:");
            _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1, GUILayout.Height(200));
            foreach (var data in _characterDataList)
            {
                EditorGUILayout.LabelField(data.name);
            }
            EditorGUILayout.EndScrollView();

            // Operation Value Data列表
            GUILayout.Label("可操作值注册表");
            _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, GUILayout.Height(200));
            foreach (var data in _operationValueDataList)
            {
                EditorGUILayout.LabelField(data.name);
            }
            EditorGUILayout.EndScrollView();

            // 刷新按钮
            if (GUILayout.Button("刷新注册表", GUILayout.Height(30)))
            {
                RefreshRegistry();
            }
        }

        private void RefreshRegistry()
        {
            if (_targetPath.Count == 0)
            {
                Debug.LogError("Target path not set!");
                return;
            }

            ClearLists();
            LoadAssets<CharacterData>(_characterDataList);
            LoadAssets<OperationValueData>(_operationValueDataList);

            ValidateAssetsGuid();

            _registry.CharacterDataList = _characterDataList;
            _registry.OperationValueDataList = _operationValueDataList;

            EditorUtility.SetDirty(_registry);
            AssetDatabase.SaveAssetIfDirty(_registry);
            AssetDatabase.Refresh();
        }

        private void ValidateAssetsGuid()
        {
            // 当前角色和操作值的创建并未被插件统一管理，所以在创建时并未初始化Guid
            // 作为临时修复方案（并不临时，大概要用很久）
            // 在刷新注册表的时候统一对空的Guid进行初始化
            foreach (var character in _characterDataList)
            {
                if (string.IsNullOrEmpty(character.CharacterID))
                {
                    character.CharacterID = Guid.NewGuid().ToString();
                }
            }
            foreach (var operation in _operationValueDataList)
            {
                operation.Guid = Guid.NewGuid().ToString();
            }
        }

        private void LoadAssets<T>(List<T> list) where T : ScriptableObject
        {
            string typeName = typeof(T).Name;
            EGlobalVariablesType variableType = EGlobalVariablesType.None;
            switch (typeName)
            {
                case "CharacterData":
                    variableType = EGlobalVariablesType.ECharacter;
                    break;
                case "OperationValueData":
                    variableType = EGlobalVariablesType.EOperationValue;
                    break;
            }
            string filter = $"t:{typeName}";
            string[] guids = AssetDatabase.FindAssets(filter, new[] { _targetPath[variableType] });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    list.Add(asset);
                }
            }

            Debug.Log($"Loaded {list.Count} {typeName} assets from {_targetPath}");
        }

        private void ClearLists()
        {
            _characterDataList.Clear();
            _operationValueDataList.Clear();
        }

        // 在父窗口中打开的方法示例
        public static void ShowWindow(Dictionary<EGlobalVariablesType, string> path, GlobalStoryRegistry registry)
        {
            var window = GetWindow<RegistryWindow>("Registry");
            window.Init(path, registry);
            window.Show();
        }
    }
}
#endif