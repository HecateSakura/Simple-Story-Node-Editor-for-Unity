#if UNITY_EDITOR
using System;
using UnityEngine;
namespace Rift.Story
{
    public enum EOperationModifyType
    {
        Plus = 0,
        Subtruct = 1,
        Cross = 2,
        Divide = 3
    }
    [Serializable]
    public struct StoryEventsData
    {
        [SerializeField] public OperationValueData OperationValue;
        [SerializeField] public EOperationModifyType OperationModifyType;
        [SerializeField] public OperationValueData OperationModifyValue;
    }

}
#endif