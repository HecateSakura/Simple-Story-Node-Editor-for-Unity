#if UNITY_EDITOR
using System;
using UnityEngine;
namespace Rift.Story
{
    public enum CompareType
    {
        Equal = 0,          // ==
        NotEqual = 1,       // !=
        Greater = 2,        // >
        Less = 3,           // <
        GreaterOrEqual = 4, // >=
        LessOrEqual = 5     // <=
    }

    [Serializable]
    public struct Conditional
    {
        [SerializeField] public OperationValueData LeftValue;
        [SerializeField] public OperationValueData RightValue;
        [SerializeField] public CompareType CompareType;
    }

    [Serializable]
    public struct ChoiceData
    {
        [SerializeField] public string ChoiceTitle;
        [SerializeField] public string ChoiceDescription;
        [SerializeField] public StoryNodeData NavigateToNode;
        [SerializeField] public bool EnableCondition;
        [SerializeField] public Conditional Condition;
    }
}
#endif