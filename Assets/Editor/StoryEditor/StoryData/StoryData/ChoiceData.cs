#if UNITY_EDITOR
using System;
using UnityEngine;
namespace Rift.Story
{
    [Serializable]
    public struct ChoiceData
    {
        [SerializeField] public string ChoiceTitle;
        [SerializeField] public string ChoiceDescription;
        [SerializeField] public StoryNodeData NavigateToNode;
    }
}
#endif