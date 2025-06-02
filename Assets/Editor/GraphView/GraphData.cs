#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
namespace Rift.Story
{
    // CreateAssetMenu 仅供测试使用
    //[CreateAssetMenu(fileName = "NewGraphData", menuName = "Graph/Graph Data")]
    public class GraphData : ScriptableObject
    {
        public List<NodeData> nodes = new List<NodeData>();
        public List<EdgeData> edges = new List<EdgeData>();
    }
}
#endif