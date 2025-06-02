#if UNITY_EDITOR
using System;
using UnityEngine;
namespace Rift.Story
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewVariableData", menuName = "Rift Story/Data/Variable Data")]
    public class OperationValueData : ScriptableObject
    {
        public string Guid;
        public ValueType ValueType;

        [HideInInspector] public int IntValue;
        [HideInInspector] public float FloatValue;
        [HideInInspector] public bool BoolValue;

        public object GetValue()
        {
            switch (ValueType)
            {
                case ValueType.Integer: return IntValue;
                case ValueType.Float: return FloatValue;
                case ValueType.Boolean: return BoolValue;
                default: return null;
            }
        }
    }
}
#endif