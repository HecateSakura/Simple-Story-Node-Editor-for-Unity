#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Rift.Story
{
    [CustomEditor(typeof(OperationValueData))]
    public class OperationValueDataEditor : UnityEditor.Editor
    {
        private SerializedProperty _valueTypeProp;
        private SerializedProperty _intValueProp;
        private SerializedProperty _floatValueProp;
        private SerializedProperty _boolValueProp;

        private void OnEnable()
        {
            _valueTypeProp = serializedObject.FindProperty("ValueType");
            _intValueProp = serializedObject.FindProperty("IntValue");
            _floatValueProp = serializedObject.FindProperty("FloatValue");
            _boolValueProp = serializedObject.FindProperty("BoolValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_valueTypeProp);

            switch ((ValueType)_valueTypeProp.enumValueIndex)
            {
                case ValueType.Integer:
                    EditorGUILayout.PropertyField(_intValueProp, new GUIContent("Integer Value"));
                    break;
                case ValueType.Float:
                    EditorGUILayout.PropertyField(_floatValueProp, new GUIContent("Float Value"));
                    break;
                case ValueType.Boolean:
                    EditorGUILayout.PropertyField(_boolValueProp, new GUIContent("Boolean Value"));
                    break;
                case ValueType.Unsupported:
                default:
                    EditorGUILayout.HelpBox("不支持的数值类型", MessageType.Warning);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif