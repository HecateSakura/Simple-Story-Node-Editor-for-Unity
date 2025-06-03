#if UNITY_EDITOR
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Rift.Story
{
    public enum ENodeDirection
    {
        Parent,
        Child
    }
    // CreateAssetMenu 仅供测试使用
    //[CreateAssetMenu(fileName = "NewGraphData", menuName = "Graph/Graph Data")]
    public class GraphData : ScriptableObject
    {
        public List<NodeData> nodes = new List<NodeData>();
        public List<EdgeData> edges = new List<EdgeData>();
        public List<string> GetConnectedNodes(Guid guid,ENodeDirection direction)
        {
            bool isFindParent = direction == ENodeDirection.Parent;
            List<string> result = null;
            NodeData nodeData = nodes.Find(node => node.guid.Equals(guid.ToString()));
            // 目前可以确定的是，我们所有的node都有且只有一个输入/输出端口，但是考虑到未来的拓展，暂时保留List数据结构
            List<string> allConnectedPortId = isFindParent
                ? nodeData.inputPorts[0].connectedPortGuids
                : nodeData.outputPorts[0].connectedPortGuids;
            if(allConnectedPortId == null || allConnectedPortId.Count == 0)
            {
                return result;
            }

            List<NodeData> allConnectedNodes = new List<NodeData>();
            for(int i = 0;i < allConnectedPortId.Count;i++)
            {
                for(int j = 0; j < nodes.Count;j++)
                {
                    string portGuid = "null";
                    if ((nodes[j].outputPorts != null && nodes[j].inputPorts != null)
                        && (nodes[j].outputPorts.Count != 0 && nodes[j].inputPorts.Count != 0))
                    {
                        portGuid = isFindParent ? nodes[j].outputPorts[0].portGuid : nodes[j].inputPorts[0].portGuid;
                        if (portGuid.Equals(allConnectedPortId[i]))
                        {
                            allConnectedNodes.Add(nodes[j]);
                        }
                    }
                }
            }
            if(allConnectedNodes == null || allConnectedNodes.Count == 0)
            {
                return result;
            }

            result = new List<string>();
            for(int i = 0; i < allConnectedNodes.Count;i++)
            {
                result.Add(allConnectedNodes[i].guid);
            }
            return result;
        }
    }
}
#endif