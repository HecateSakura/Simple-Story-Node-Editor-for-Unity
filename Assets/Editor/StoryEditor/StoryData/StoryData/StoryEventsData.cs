#if UNITY_EDITOR
using System;
using UnityEngine;
namespace Rift.Story
{
    public enum EOperationModifyType
    {
        None = 0,
        Plus = 1,
        Subtruct = 2,
        Cross = 3,
        Divide = 4
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