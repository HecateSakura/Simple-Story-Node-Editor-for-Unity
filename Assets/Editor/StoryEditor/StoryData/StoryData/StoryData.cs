#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
namespace Rift.Story
{
    [System.Serializable]
    public class StoryData : ScriptableObject
    {
        [HideInInspector] public string ProjectName;
        [SerializeField] public GraphData Graph;
        private bool _errorCheck => (_keys.Count == _values.Count)
                && (_keys.Count == _data.Count);
        public Dictionary<EGlobalVariablesType, string> GlobalVariablesPaths { get => GetGlobalVariablesPaths(); set => _globalVariablesPaths = value; }
        private Dictionary<EGlobalVariablesType, string> _globalVariablesPaths;
        [HideInInspector]
        private List<EGlobalVariablesType> _types;
        [HideInInspector]
        private List<string> _paths;
        public Dictionary<Guid, StoryNodeData> Data => GetData();
        private Dictionary<Guid, StoryNodeData> _data;
        [SerializeField, ReadOnly(true)]
        private List<string> _keys;
        [SerializeField, ReadOnly(true)]
        private List<StoryNodeData> _values;
        public void AddNode(Guid guid, StoryNodeData data)
        {
            if (_data == null)
            {
                _data = new Dictionary<Guid, StoryNodeData>();
            }
            if (_keys == null)
            {
                _keys = new List<string>();
            }
            if (_values == null)
            {
                _values = new List<StoryNodeData>();
            }
            if (_data.ContainsKey(guid) && _data[guid].Equals(data))
            {
                Debug.LogWarning($"节点: {guid} 已经在Data中，不能重复添加节点");
                return;
            }
            _data.Add(guid, data);
            _keys.Add(guid.ToString());
            _values.Add(data);
            CountCheck();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        public void RemoveNode(Guid guid)
        {
            if (_data == null)
            {
                return;
            }
            if (!_data.ContainsKey(guid))
            {
                return;
            }
            _keys.Remove(guid.ToString());
            _values.Remove(_data[guid]);
            _data.Remove(guid);
            CountCheck();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CountCheck()
        {
            if (!_errorCheck)
            {
                Debug.LogError("数量不一致");
                Debug.LogError("======");
                Debug.LogError($"_keys {_keys.Count}");
                Debug.LogError($"_data {_data.Count}");
                Debug.LogError($"_values {_values.Count}");
                Debug.LogError("======");
            }
        }
        private Dictionary<Guid, StoryNodeData> GetData()
        {
            if (_data == null)
            {
                _data = new Dictionary<Guid, StoryNodeData>();
                for (int i = 0; i < _keys.Count; i++)
                {
                    _data.Add(new Guid(_keys[i]), _values[i]);
                }
                return _data;
            }
            else
            {
                return _data;
            }
        }
        private Dictionary<EGlobalVariablesType, string> GetGlobalVariablesPaths()
        {
            if (_globalVariablesPaths == null)
            {
                _globalVariablesPaths = new Dictionary<EGlobalVariablesType, string>();
                for (int i = 0; i < _types.Count; i++)
                {
                    _globalVariablesPaths.Add(_types[i], _paths[i]);
                }
                return _globalVariablesPaths;
            }
            else
            {
                return _globalVariablesPaths;
            }
        }
        public void DebugLogAllData()
        {
            foreach (var data in _data)
            {
                Debug.Log(data.ToString());
            }
        }
    }
}
#endif