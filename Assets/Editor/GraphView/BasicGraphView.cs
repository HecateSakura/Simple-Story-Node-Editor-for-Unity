#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
namespace Rift.Story
{
    public class BasicGraphView : GraphView
    {
        private ContextualMenuManipulator _contextManipulator;
        public event Action<ContextualMenuPopulateEvent> OnContextMenuCreated;
        public event Action<IEnumerable<Node>> OnSelectionElementsDelete;

        public BasicGraphView()
        {
            // 添加交互控制器
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentZoomer());

            _contextManipulator = new ContextualMenuManipulator(BuildContextMenu);
            _contextManipulator.target = this;
            deleteSelection = CustomDeleteSelection;
        }

        private void CustomDeleteSelection(string operationName, AskUser askUser)
        {
            List<Node> nodes = new List<Node>();
            foreach (var selected in selection)
            {
                Node node = selected as Node;
                if (node != null)
                {
                    nodes.Add(node);
                }
                else
                {
                    continue;
                }
            }
            OnSelectionElementsDelete?.Invoke(nodes);
            DeleteSelection();
        }

        public void RegisterContextMenu(System.Action<ContextualMenuPopulateEvent> callback)
        {
            OnContextMenuCreated += callback;
        }
        public void UnRegisterContextMenu(System.Action<ContextualMenuPopulateEvent> callback)
        {
            OnContextMenuCreated -= callback;
        }
        private void BuildContextMenu(ContextualMenuPopulateEvent eventArgs)
        {
            OnContextMenuCreated?.Invoke(eventArgs);
        }

        // 生成节点连接的可接受端口列表
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            ports.ForEach(port =>
            {
                if (startPort != port && startPort.node != port.node && startPort.direction != port.direction)
                    compatiblePorts.Add(port);
            });
            return compatiblePorts;
        }
        public override EventPropagation DeleteSelection()
        {

            return base.DeleteSelection();
        }
    }
}
#endif