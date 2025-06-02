#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Rift.Story
{
    public class BasicGraphWindow : EditorWindow
    {
        private BasicGraphView _graphView;
        private int _nodeCounter = 1;
        private const string _parentPath = "Assets/Configs";
        private const string _subFolderName = "GraphData";
        private const string _categoryFolderName = "";

        protected Toolbar _toolbar;
        protected GraphData _graphData;
        protected virtual string _folderParentPath => _parentPath;
        protected virtual string _subFolder => _subFolderName;
        protected virtual string _catrgoryFolder => _categoryFolderName;

        protected ObservableCollection<Guid> _selectionNodes;
        protected virtual string _windowTitle => "节点编辑器";
        public static void OpenWindow()
        {
            var window = GetWindow<BasicGraphWindow>();
            window.InitWindow(window);
        }
        public virtual void InitWindow(BasicGraphWindow window)
        {
            window.titleContent = new GUIContent(_windowTitle);
            float width = 800;
            float height = 600;
            var mainWindow = EditorGUIUtility.GetMainWindowPosition();
            float x = (mainWindow.width - width) * 0.5f + mainWindow.x;
            float y = (mainWindow.height - height) * 0.5f + mainWindow.y;
            window.position = new Rect(x, y, width, height);
            _graphView.OnSelectionElementsDelete += OnSelectionElementsDeleted;
            LoadGraphData();
        }

        private void OnEnable()
        {
            BuildGraphView();
            AddToolbar();
            RegisterCustomMenuItem();

            _selectionNodes = new ObservableCollection<Guid>();
            _selectionNodes.CollectionChanged += OnSelectionNodesChanged;
        }

        private void RegisterCustomMenuItem()
        {
            _graphView.RegisterContextMenu(CreateNewNode);
        }
        private void OnDisable()
        {
            SaveGraphData(); // 窗口关闭时自动保存
            foreach (var node in _graphView.nodes.ToList().Cast<BasicNode>())
            {
                node.OnNodeSelected -= OnNodeSelected;
                node.OnNodeUnselected -= OnNodeUnSelected;
            }
            _selectionNodes.CollectionChanged -= OnSelectionNodesChanged;
        }

        private void CreateNewNode(ContextualMenuPopulateEvent eventArgs)
        {
            Vector2 mousePos = eventArgs.localMousePosition;
            Vector2 graphMousePos = _graphView.contentViewContainer.WorldToLocal(
                eventArgs.mousePosition);
            eventArgs.menu.AppendAction("新建节点", _ =>
            {
                CreateNode("Node " + _nodeCounter++, mousePos);
            });
        }

        private void BuildGraphView()
        {
            // 创建自定义的 GraphView 子类
            _graphView = new BasicGraphView
            {
                name = "节点画布"
            };

            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);

            // 添加网格背景
            var grid = new GridBackground();
            _graphView.Insert(0, grid);
        }

        protected virtual void AddToolbar()
        {
            if (_toolbar == null)
                _toolbar = new Toolbar();

            // 保存/加载按钮
            var saveBtn = new Button(OnSaveBtnClicked) { text = "保存" };
            var loadBtn = new Button(OnLoadBtnClicked) { text = "加载" };

            _toolbar.Add(new ToolbarSpacer() { style = { flexGrow = 1 } });
            _toolbar.Add(saveBtn);
            _toolbar.Add(loadBtn);

            rootVisualElement.Add(_toolbar);
        }
        protected virtual void OnSaveBtnClicked()
        {
            SaveGraphData();
        }
        protected virtual void OnLoadBtnClicked()
        {
            LoadGraphData();
        }
        protected virtual void SaveGraphData()
        {
            if (_graphData != null)
            {
                _graphData.nodes.Clear();
                _graphData.edges.Clear();
            }
            else
            {
                _graphData = CreateGraphDataAsset<GraphData>();
            }
            // 序列化节点
            foreach (var node in _graphView.nodes.ToList().Cast<Node>())
            {
                var nodeData = new NodeData
                {
                    guid = node.viewDataKey,
                    title = node.title,
                    position = node.GetPosition()
                };

                // 序列化端口连接
                SerializePorts(node.inputContainer, nodeData.inputPorts);
                SerializePorts(node.outputContainer, nodeData.outputPorts);

                _graphData.nodes.Add(nodeData);
            }

            // 序列化连线
            foreach (var edge in _graphView.edges.ToList())
            {
                _graphData.edges.Add(new EdgeData
                {
                    outputPortGuid = (edge.output as Port)?.viewDataKey,
                    inputPortGuid = (edge.input as Port)?.viewDataKey
                });
            }

            EditorUtility.SetDirty(_graphData);
            AssetDatabase.SaveAssets();
        }
        protected T CreateGraphDataAsset<T>(string parentPath, string subFolder = "", string categoryFolder = "", string fileName = null) where T : ScriptableObject
        {
            var so = ScriptableObjectUtility.CreateSO<T>(parentPath, subFolder, categoryFolder, fileName);
            return so;
        }
        protected T CreateGraphDataAsset<T>() where T : ScriptableObject
        {
            var so = CreateGraphDataAsset<T>(_folderParentPath, _subFolder, _catrgoryFolder, DateTime.UtcNow.Ticks.ToString());
            return so;
        }

        private void SerializePorts(VisualElement container, List<PortData> targetList)
        {
            foreach (var port in container.Children().Cast<Port>())
            {
                var portData = new PortData
                {
                    portGuid = port.viewDataKey
                };

                foreach (var edge in port.connections)
                {
                    var connectedPort = port.direction == Direction.Input ? edge.output : edge.input;
                    portData.connectedPortGuids.Add(connectedPort.viewDataKey);
                }

                targetList.Add(portData);
            }
        }
        protected virtual void LoadGraphData()
        {
            // 创建或加载 SO
            var path = EditorUtility.OpenFilePanel("加载图形数据", "Assets/", "asset");
            if (!string.IsNullOrEmpty(path))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                _graphData = AssetDatabase.LoadAssetAtPath<GraphData>(path);

                ReBuildGraphViewByGraphData();
            }
            else if (_graphData == null)
            {
                Debug.LogError("没有找到项目文件，强制退出");
                Close();
            }
        }

        protected void ReBuildGraphViewByGraphData()
        {
            // 清空当前视图
            _graphView.DeleteElements(_graphView.graphElements.ToList());
            // 节点重建
            var nodeMap = new Dictionary<string, Node>();
            foreach (var nodeData in _graphData.nodes)
            {
                var node = CreateNode(nodeData.title, nodeData.position.position, nodeData.guid);
                node.viewDataKey = nodeData.guid;
                nodeMap.Add(nodeData.guid, node);

                // 恢复端口GUID
                RestorePorts(node.inputContainer, nodeData.inputPorts);
                RestorePorts(node.outputContainer, nodeData.outputPorts);
            }

            // 连接重建
            foreach (var edgeData in _graphData.edges)
            {
                var outputPort = FindPortByGuid(edgeData.outputPortGuid);
                var inputPort = FindPortByGuid(edgeData.inputPortGuid);

                if (outputPort != null && inputPort != null)
                {
                    var edge = outputPort.ConnectTo(inputPort);
                    _graphView.AddElement(edge);
                }
            }

            Port FindPortByGuid(string guid)
            {
                return _graphView.Query<Port>().Where(p => p.viewDataKey == guid).First();
            }
        }

        private void RestorePorts(VisualElement container, List<PortData> portDatas)
        {
            var ports = container.Children().Cast<Port>().ToList();
            for (int i = 0; i < ports.Count; i++)
            {
                if (i < portDatas.Count)
                {
                    ports[i].viewDataKey = portDatas[i].portGuid;
                }
            }
        }
        protected virtual Node CreateNode(string title, Vector2 position = default, string guid = "")
        {
            var node = new BasicNode
            {
                title = title,
                viewDataKey = string.IsNullOrEmpty(guid) ? Guid.NewGuid().ToString() : guid
            };

            node.SetPosition(new Rect(position, new Vector2(200, 150)));

            // 输入端口
            var inputPort = CreatePort(node, Direction.Input);
            inputPort.portName = "输入";
            node.inputContainer.Add(inputPort);

            // 输出端口
            var outputPort = CreatePort(node, Direction.Output);
            outputPort.portName = "输出";
            node.outputContainer.Add(outputPort);

            node.RefreshPorts();
            node.RefreshExpandedState();

            _graphView.AddElement(node);
            node.OnNodeSelected += OnNodeSelected;
            node.OnNodeUnselected += OnNodeUnSelected;
            return node;
        }

        private Port CreatePort(Node node, Direction direction)
        {
            var port = Port.Create<UnityEditor.Experimental.GraphView.Edge>(
                Orientation.Horizontal,
                direction,
                Port.Capacity.Multi,
                typeof(float)
            );

            port.viewDataKey = Guid.NewGuid().ToString();
            return port;
        }
        #region Callback
        protected virtual void OnNodeSelected(Guid guid)
        {
            _selectionNodes.Add(guid);
        }
        protected virtual void OnNodeUnSelected(Guid guid)
        {
            _selectionNodes.Remove(guid);
        }
        protected virtual void OnSelectionNodesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        protected virtual void OnSelectionElementsDeleted(IEnumerable<Node> enumerable)
        {

        }
        #endregion
    }
}
#endif