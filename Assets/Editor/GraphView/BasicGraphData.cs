#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
namespace Rift.Story
{
    [System.Serializable]
    public class NodeData
    {
        public string guid;
        public string title;
        public Rect position;
        public List<PortData> inputPorts = new List<PortData>();
        public List<PortData> outputPorts = new List<PortData>();
    }

    [System.Serializable]
    public class PortData
    {
        public string portGuid;
        public List<string> connectedPortGuids = new List<string>();
    }

    [System.Serializable]
    public class EdgeData
    {
        public string outputPortGuid;
        public string inputPortGuid;
    }
}
#endif