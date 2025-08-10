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
        private SerializedProperty _autoSelectProp;
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
            _autoSelectProp = serializedObject.FindProperty("AutoSelect");
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
            // 绘制自动选择属性
            EditorGUILayout.PropertyField(_autoSelectProp, new GUIContent("是否自动选择"));

            // 展开选项列表
            _choicesContentProp.isExpanded = EditorGUILayout.Foldout(_choicesContentProp.isExpanded, "选项列表");
            if (!_choicesContentProp.isExpanded) return;

            EditorGUI.indentLevel++;
            for (int i = 0; i < _choicesContentProp.arraySize; i++)
            {
                SerializedProperty choiceProp = _choicesContentProp.GetArrayElementAtIndex(i);
                SerializedProperty conditionProp = choiceProp.FindPropertyRelative("Condition");

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.PropertyField(choiceProp.FindPropertyRelative("ChoiceTitle"), new GUIContent("选项标题"));
                EditorGUILayout.PropertyField(choiceProp.FindPropertyRelative("ChoiceDescription"), new GUIContent("选项描述"));
                EditorGUILayout.PropertyField(choiceProp.FindPropertyRelative("NavigateToNode"), new GUIContent("跳转节点"));
                EditorGUILayout.PropertyField(choiceProp.FindPropertyRelative("EnableCondition"), new GUIContent("启用条件"));

                if(choiceProp.FindPropertyRelative("EnableCondition").boolValue)
                {
                    // 绘制条件属性
                    DrawConditionalProps(conditionProp);
                }

                EditorGUILayout.EndVertical();
            }

            // 添加/移除选项按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加选项"))
            {
                _choicesContentProp.arraySize++;
            }
            if (GUILayout.Button("移除选项"))
            {
                if (_choicesContentProp.arraySize > 0)
                {
                    _choicesContentProp.arraySize--;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
        }

        // 绘制条件属性
        private void DrawConditionalProps(SerializedProperty conditionProp)
        {
            SerializedProperty leftValueProp = conditionProp.FindPropertyRelative("LeftValue");
            SerializedProperty rightValueProp = conditionProp.FindPropertyRelative("RightValue");
            SerializedProperty compareTypeProp = conditionProp.FindPropertyRelative("CompareType");

            EditorGUILayout.LabelField("条件设置", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            // 绘制左右值
            EditorGUILayout.PropertyField(leftValueProp, new GUIContent("左值"));
            EditorGUILayout.PropertyField(rightValueProp, new GUIContent("右值"));

            // 获取值类型
            ValueType? leftType = GetValueType(leftValueProp);
            ValueType? rightType = GetValueType(rightValueProp);

            // 类型检查
            if (leftType != null && rightType != null && leftType != rightType)
            {
                EditorGUILayout.HelpBox(
                    $"类型警告: 左值类型({leftType})与右值类型({rightType})不一致!",
                    MessageType.Warning
                );
            }

            // 根据值类型限制比较运算符
            if (leftType == ValueType.Boolean || rightType == ValueType.Boolean)
            {
                EditorGUILayout.HelpBox(
                    "当ValueType为Boolean时，选择Equal和NotEqual以外的比较类型可能会导致程序错误",
                    MessageType.Info
                );
                EditorGUILayout.PropertyField(compareTypeProp, new GUIContent("比较类型"));
            }
            else
            {
                // 其他类型显示全部比较运算符
                EditorGUILayout.PropertyField(compareTypeProp, new GUIContent("比较类型"));
            }

            EditorGUI.indentLevel--;
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