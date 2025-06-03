#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Rift.Story
{
    public class InitStoryEditorWindow : EditorWindow
    {
        private NameEditorWindow _nameEditorWindow;
        private GraphData _graphData;
        private StoryData _data;
        private const string _graphDataParentPath = "Assets/Configs/GraphData";
        private const string _projectParentPath = "Assets/Configs/StoryProjects";
        private const string _subFoldergGraphData = "StoryGraphData";
        private const string _subFoldergProjectData = "ProjectData";
        private const string _subFoldergRegistryData = "GlobalRegistry";

        [MenuItem("Tools/Story Editor Window")]
        public static void OpenInitWindow()
        {
            var window = GetWindow<InitStoryEditorWindow>();
            float width = 300;
            float height = 150;
            var mainWindow = EditorGUIUtility.GetMainWindowPosition();
            float x = (mainWindow.width - width) * 0.5f + mainWindow.x;
            float y = (mainWindow.height - height) * 0.5f + mainWindow.y;
            window.position = new Rect(x, y, width, height);
        }
        private void OnGUI()
        {
            EditorGUILayout.Space();

            if (GUILayout.Button("创建新项目", GUILayout.Height(30)))
            {
                OnCreateData();
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("读取已有项目", GUILayout.Height(30)))
            {
                OnLoadData();
            }

            EditorGUILayout.Space();
        }
        private void OnCreateData()
        {
            if (_nameEditorWindow == null)
            {
                _nameEditorWindow = NameEditorWindow.OpenNamerWindow();
            }
            _nameEditorWindow.OnNamerFinshed += OnNameFinished;
        }


        private void OnLoadData()
        {
            var path = EditorUtility.OpenFilePanel("加载已有剧情树项目", "Assets/", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                _data = AssetDatabase.LoadAssetAtPath<StoryData>(path);
            }
            else
            {
                return;
            }
            if (_data == null)
            {
                Debug.LogError("创建剧情树项目文件失败，请检查文件类型后重试");
            }
            else
            {
                Debug.Log("打开并初始化剧情树编辑器");
                var window = GetWindow<StoryEditorWindow>();
                window.SetStoryData(_data);
                window.InitWindow(window);
                Close();
            }
        }

        private void OnNameFinished(string name)
        {
            _nameEditorWindow.OnNamerFinshed -= OnNameFinished;
            string basicConfigPath = "Assets/Configs";
            string basicPath = _projectParentPath + "/" + _subFoldergRegistryData;
            string registryPath = _projectParentPath + "/" + _subFoldergRegistryData;
            string characterParentPath = registryPath + "/" + "Character";
            string operationValueParentPath = registryPath + "/" + "OperationValue";
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            ValidatePaths(basicConfigPath, basicPath, registryPath, characterParentPath, operationValueParentPath);

            _graphData = ScriptableObjectUtility.CreateSO<GraphData>(_graphDataParentPath, _subFoldergGraphData, name, name);
            _data = ScriptableObjectUtility.CreateSO<StoryData>(_projectParentPath, _subFoldergProjectData, fileName: name);

            if (_data != null && _graphData != null)
            {
                _data.ProjectName = name;
                Dictionary<EGlobalVariablesType, string> variables = new Dictionary<EGlobalVariablesType, string>();
                variables.Add(EGlobalVariablesType.ECharacter, characterParentPath);
                variables.Add(EGlobalVariablesType.EOperationValue, operationValueParentPath);

                _data.GlobalVariablesPaths = variables;
                _data.Graph = _graphData;

                EditorUtility.SetDirty(_data);
                AssetDatabase.SaveAssetIfDirty(_data);
                AssetDatabase.Refresh();

                // 打开并初始化剧情树编辑器
                var window = GetWindow<StoryEditorWindow>();
                window.SetStoryData(_data);
                window.InitWindow(window);
                Close();
            }
        }

        private static void ValidatePaths(string basicConfigPath, string basicPath, string registryPath, string characterParentPath, string operationValueParentPath)
        {
            if (!AssetDatabase.IsValidFolder(basicConfigPath))
            {
                AssetDatabase.CreateFolder("Assets", "Configs");
            }
            if (!AssetDatabase.IsValidFolder(basicPath))
            {
                AssetDatabase.CreateFolder(basicConfigPath, "StoryProjects");
            }
            if (!AssetDatabase.IsValidFolder(_graphDataParentPath))
            {
                AssetDatabase.CreateFolder(basicConfigPath, "GraphData");
            }
            if (!AssetDatabase.IsValidFolder(registryPath))
            {
                AssetDatabase.CreateFolder(_projectParentPath, "GlobalRegistry");
            }
            if (!AssetDatabase.IsValidFolder(characterParentPath))
            {
                AssetDatabase.CreateFolder(registryPath, "Character");
            }
            if (!AssetDatabase.IsValidFolder(operationValueParentPath))
            {
                AssetDatabase.CreateFolder(registryPath, "OperationValue");
            }
        }
    }
    public class NameEditorWindow : EditorWindow
    {
        public Action<string> OnNamerFinshed;
        private string _name = "ProjectName";
        public static NameEditorWindow OpenNamerWindow()
        {
            var window = GetWindow<NameEditorWindow>();
            float width = 300;
            float height = 150;
            var mainWindow = EditorGUIUtility.GetMainWindowPosition();
            float x = (mainWindow.width - width) * 0.5f + mainWindow.x;
            float y = (mainWindow.height - height) * 0.5f + mainWindow.y;
            window.position = new Rect(x, y, width, height);
            return window;
        }
        private void OnGUI()
        {
            EditorGUILayout.Space();

            _name = GUILayout.TextField(_name, GUILayout.Height(30));

            EditorGUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("创建", GUILayout.Height(30)))
            {
                OnNamerFinshed?.Invoke(_name);
                Close();
            }
            if (GUILayout.Button("取消", GUILayout.Height(30)))
            {
                OnNamerFinshed?.Invoke("");
                Close();
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
    public class StoryEditorWindow : BasicGraphWindow
    {
        private StoryData _data;
        private GlobalStoryRegistry _registry;
        private bool _enableCustomNodeEditorWindow = false;

        private const string _graphDataParentPath = "Assets/Configs/GraphData";
        private const string _projectParentPath = "Assets/Configs/StoryProjects";
        private const string _subFoldergGraphData = "StoryGraphData";
        private const string _subFoldergProjectData = "ProjectData";
        private const string _subFoldergGlobalRegistry = "GlobalRegistry";
        private const string _nodesFolder = "Nodes";
        protected override string _folderParentPath => _graphDataParentPath;
        protected override string _subFolder => _subFoldergGraphData;
        protected override string _catrgoryFolder => _data != null ? _data.name : "";
        protected override string _windowTitle => "剧情树编辑器";
        public static StoryEditorWindow OpenStoryEditorWindow()
        {
            var window = GetWindow<StoryEditorWindow>();
            window.InitWindow(window);
            return window;
        }
        public void SetStoryData(StoryData data)
        {
            _data = data;
        }
        protected override void AddToolbar()
        {
            if (_toolbar == null)
                _toolbar = new Toolbar();

            var createNodeBtn = new Button(() => OnRegistryButtonClicked())
            {
                text = "注册表管理"
            };
            var exportNodeBtn = new Button(() => OnExportProjectButtonClicked())
            {
                text = "导出项目"
            };
            _toolbar.Add(createNodeBtn);
            _toolbar.Add(exportNodeBtn);
            base.AddToolbar();
        }
        protected override void LoadGraphData()
        {
            if (_data == null)
            {
                Debug.LogError("初始化剧情树编辑器失败");
                return;
            }
            else
            {
                _graphData = _data.Graph;
                ReBuildGraphViewByGraphData();
            }
        }
        protected override Node CreateNode(string title, Vector2 position = default, string guid = "")
        {
            var node = base.CreateNode(title, position, guid);
            StoryNodeData data = null;
            if (string.IsNullOrEmpty(guid))
            {
                string parentPath = _projectParentPath + "/" + _subFoldergProjectData;
                data = CreateGraphDataAsset<StoryNodeData>(parentPath, _data.name, _nodesFolder, "node_" + node.viewDataKey);
                data.Title = node.title;
                data.Description = "test content";
                data.Guid = node.viewDataKey;
                _data.AddNode(new Guid(data.Guid), data);
            }
            else
            {
                data = _data.Data[new Guid(node.viewDataKey)];
            }

            var editBtn = new Button(() => OnEditButtonClicked(data))
            {
                text = "编辑",
                style = { width = 60, height = 20 }
            };
            node.titleButtonContainer.Add(editBtn);

            return node;
        }
        protected override void OnSelectionElementsDeleted(IEnumerable<Node> enumerable)
        {
            base.OnSelectionElementsDeleted(enumerable);
            foreach (var node in enumerable)
            {
                string parentPath = _projectParentPath + "/" + _subFoldergProjectData;
                ScriptableObjectUtility.DeleteSO<StoryNodeData>(parentPath, _data.name, _nodesFolder, "node_" + node.viewDataKey);
                _data.RemoveNode(new Guid(node.viewDataKey));
            }
        }
        protected override void SaveGraphData()
        {
            base.SaveGraphData();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        protected override void OnLoadBtnClicked()
        {
            StoryData newData;
            var path = EditorUtility.OpenFilePanel("加载已有剧情树项目", "Assets/", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                newData = AssetDatabase.LoadAssetAtPath<StoryData>(path);
            }
            else
            {
                return;
            }
            if (newData == null)
            {
                Debug.LogError("创建剧情树项目文件失败，请检查文件类型后重试");
            }
            else if (newData.Equals(_data))
            {
                return;
            }
            else
            {
                Debug.Log("打开并初始化剧情树编辑器");
                _data = newData;
                _graphData = _data.Graph;
                ReBuildGraphViewByGraphData();
            }
        }
        private void OnEditButtonClicked(StoryNodeData node)
        {
            if (_enableCustomNodeEditorWindow)
            {
                var window = GetWindow<NodeEditorWindow>();
                window.ShowWindow(node);
            }
            else
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = node;
            }
        }
        private void OnRegistryButtonClicked()
        {
            if (_registry == null)
            {
                _registry = AssetDatabase.LoadAssetAtPath(_projectParentPath + "/" + _subFoldergGlobalRegistry + "/" + "GlobalRegistry.asset", typeof(ScriptableObject)) as GlobalStoryRegistry;
            }

            if (_registry == null)
            {
                _registry = ScriptableObjectUtility.CreateSO<GlobalStoryRegistry>(_projectParentPath, _subFoldergGlobalRegistry, fileName: "GlobalRegistry");
            }
            RegistryWindow.ShowWindow(_data.GlobalVariablesPaths, _registry);
        }
        #region 导出相关
        private void OnExportProjectButtonClicked()
        {
            SaveGraphData();
            ExportWindow.OpenWindow(this);
        }

        public GraphData GetGraphData()
        {
            return _graphData;
        }

        public List<CharacterData> GetCharacterData()
        {
            if (_registry == null)
            {
                _registry = AssetDatabase.LoadAssetAtPath(_projectParentPath + "/" + _subFoldergGlobalRegistry + "/" + "GlobalRegistry.asset", typeof(ScriptableObject)) as GlobalStoryRegistry;
            }
            if (_registry == null)
            {
                Debug.LogException(new NullReferenceException("没有找到角色和可操作值全局注册表！导出失败"));
            }
            return _registry.CharacterDataList;
        }

        public List<OperationValueData> GetOperationValues()
        {
            if (_registry == null)
            {
                _registry = AssetDatabase.LoadAssetAtPath(_projectParentPath + "/" + _subFoldergGlobalRegistry + "/" + "GlobalRegistry.asset", typeof(ScriptableObject)) as GlobalStoryRegistry;
            }
            if (_registry == null)
            {
                Debug.LogException(new NullReferenceException("没有找到角色和可操作值全局注册表！导出失败"));
            }
            return _registry.OperationValueDataList;
        }
        public Dictionary<Guid, StoryNodeData> GetStoryNodes()
        {
            return _data.Data;
        }
        public List<string> GetConnectedNodes(Guid guid, ENodeDirection direction)
        {
            return _data.GetConnectedNodes(guid, direction);
        }
        #endregion
    }
}
#endif