#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Rift.Story
{
    [CustomEditor(typeof(StoryNodeData))]
    public class StoryNodeDataEditor : Editor
    {
        #region 基础属性
        private SerializedProperty _titleProp;
        private SerializedProperty _descriptionProp;
        private SerializedProperty _typeProp;
        #endregion
        #region 对话属性
        private SerializedProperty _characterProp;
        private SerializedProperty _dislogContentProp;
        #endregion
        #region 选项属性
        private SerializedProperty _choicesContentProp;
        #endregion
        #region 事件属性
        private SerializedProperty _eventsProp;
        #endregion
        #region 自定义属性
        private SerializedProperty _customDataProp;
        #endregion
        private void OnEnable()
        {
            _titleProp = serializedObject.FindProperty("Title");
            _descriptionProp = serializedObject.FindProperty("Description");
            _typeProp = serializedObject.FindProperty("Type");
            _characterProp = serializedObject.FindProperty("Character");
            _dislogContentProp = serializedObject.FindProperty("DialogContents");
            _choicesContentProp = serializedObject.FindProperty("Choices");
            _customDataProp = serializedObject.FindProperty("CustomData");
            _eventsProp = serializedObject.FindProperty("StoryEvents");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawBasicProps();

            switch ((ENodeType)_typeProp.enumValueIndex)
            {
                case ENodeType.Dialog:
                    DrawDialogProps();
                    break;
                case ENodeType.Choice:
                    DrawChoicesProps();
                    break;
                case ENodeType.Event:
                    DrawEventsProps();
                    break;
                case ENodeType.None:
                default:
                    EditorGUILayout.HelpBox("选择一个节点类型", MessageType.Warning);
                    break;
            }

            DrawCustomDataProps();
            serializedObject.ApplyModifiedProperties();
        }
        private void DrawBasicProps()
        {
            EditorGUILayout.PropertyField(_titleProp, new GUIContent("节点标题"));
            EditorGUILayout.PropertyField(_descriptionProp, new GUIContent("节点描述"));
            EditorGUILayout.PropertyField(_typeProp, new GUIContent("节点类型"));
        }
        private void DrawDialogProps()
        {
            EditorGUILayout.PropertyField(_characterProp, new GUIContent("角色信息"));
            EditorGUILayout.PropertyField(_dislogContentProp, new GUIContent("对话内容"));
        }
        private void DrawChoicesProps()
        {
            EditorGUILayout.PropertyField(_choicesContentProp, new GUIContent("选项列表"));
        }
        private void DrawEventsProps()
        {
            _eventsProp.isExpanded = EditorGUILayout.Foldout(_eventsProp.isExpanded, "事件列表");
            if (!_eventsProp.isExpanded) return;

            EditorGUI.indentLevel++;
            for (int i = 0; i < _eventsProp.arraySize; i++)
            {
                SerializedProperty eventElement = _eventsProp.GetArrayElementAtIndex(i);
                SerializedProperty opValueProp = eventElement.FindPropertyRelative("OperationValue");
                SerializedProperty opModifyTypeProp = eventElement.FindPropertyRelative("OperationModifyType");
                SerializedProperty opModifyValueProp = eventElement.FindPropertyRelative("OperationModifyValue");

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.PropertyField(opValueProp, new GUIContent("操作值"));

                EditorGUILayout.PropertyField(opModifyValueProp, new GUIContent("操作修改值"));

                ValueType? valueType1 = GetValueType(opValueProp);
                ValueType? valueType2 = GetValueType(opModifyValueProp);

                if (valueType1 != null && valueType2 != null)
                {
                    if (valueType1 != valueType2)
                    {
                        EditorGUILayout.HelpBox(
                            $"类型警告: 操作值类型({valueType1})与修改值类型({valueType2})不一致!",
                            MessageType.Warning
                        );
                    }

                    bool valueTypeIsBoolean = valueType1 == ValueType.Boolean && valueType2 == ValueType.Boolean;
                    if (valueTypeIsBoolean)
                    {
                        // 隐藏操作符字段
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(opModifyTypeProp, new GUIContent("操作类型"));
                    }
                }
                else // 任一值为空时显示操作符
                {
                    EditorGUILayout.PropertyField(opModifyTypeProp, new GUIContent("操作类型"));
                }

                EditorGUILayout.EndVertical();
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加事件"))
            {
                _eventsProp.arraySize++;
            }
            if (GUILayout.Button("清除列表"))
            {
                _eventsProp.arraySize = 0;
            }
            EditorGUILayout.EndHorizontal();
        }
        private void DrawCustomDataProps()
        {
            EditorGUILayout.PropertyField(_customDataProp, new GUIContent("自定义数据"));
        }

        // 辅助方法：获取OperationValueData的类型
        private ValueType? GetValueType(SerializedProperty prop)
        {
            if (prop.objectReferenceValue == null) return null;

            SerializedObject serializedTarget = new SerializedObject(prop.objectReferenceValue);
            SerializedProperty valueTypeProp = serializedTarget.FindProperty("ValueType");
            serializedTarget.Update();
            return (ValueType)valueTypeProp.enumValueIndex;
        }
    }
}
#endif