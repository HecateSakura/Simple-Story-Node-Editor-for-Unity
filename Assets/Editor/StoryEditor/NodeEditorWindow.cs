#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace Rift.Story
{
    public class NodeEditorWindow : EditorWindow
    {
        private SerializedObject _serializedObject;
        private SerializedProperty _nodeDataProperty;

        [SerializeField] private Object _nodeData;

        private bool _isDirty; // 标记数据是否被修改
        private string _originalJson; // 用于比较数据是否改变
        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _nodeDataProperty = _serializedObject.FindProperty("_nodeData");
            _originalJson = JsonUtility.ToJson(_nodeData);

            // 监听窗口关闭事件
            wantsMouseMove = true;
            EditorApplication.wantsToQuit += OnWantsToQuit;
        }

        private void OnDisable()
        {
            EditorApplication.wantsToQuit -= OnWantsToQuit;
        }

        private bool OnWantsToQuit()
        {
            if (_isDirty)
            {
                return EditorUtility.DisplayDialog(
                    "未保存的更改",
                    "有未保存的更改，确定要退出吗？",
                    "确定",
                    "取消");
            }
            return true;
        }

        public void ShowWindow(Object data)
        {
            _nodeData = data;
            GetWindow<NodeEditorWindow>("Node Editor");
            _serializedObject = new SerializedObject(this);
            _nodeDataProperty = _serializedObject.FindProperty("_nodeData");
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            _serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.PropertyField(_nodeDataProperty, new GUIContent("节点数据"));

                if (_nodeData != null)
                {
                    EditorGUILayout.Space();
                    EditorGUI.indentLevel++;
                    DrawNodeDataFields();
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // 保存按钮区域
            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = _isDirty; // 只有数据被修改时才启用按钮

                if (GUILayout.Button("保存", GUILayout.Height(30)))
                {
                    SaveData();
                }

                GUI.enabled = true; // 恢复GUI状态
            }
            EditorGUILayout.EndHorizontal();

            // 检查数据是否被修改
            if (EditorGUI.EndChangeCheck())
            {
                string currentJson = JsonUtility.ToJson(_nodeData);
                _isDirty = !string.Equals(_originalJson, currentJson);
                _serializedObject.ApplyModifiedProperties();
            }

            // 显示未保存提示
            if (_isDirty)
            {
                EditorGUILayout.HelpBox("有未保存的更改", MessageType.Warning);
            }
        }

        private void SaveData()
        {
            if (_nodeData == null) return;

            EditorUtility.SetDirty(_nodeData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 更新原始数据快照
            _originalJson = JsonUtility.ToJson(_nodeData);
            _isDirty = false;

            ShowNotification(new GUIContent("保存成功"));
        }

        private void DrawNodeDataFields()
        {
            SerializedObject dataSerializedObject = new SerializedObject(_nodeData);
            dataSerializedObject.Update();

            SerializedProperty iterator = dataSerializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (iterator.name == "m_Script") continue;
                EditorGUILayout.PropertyField(iterator, true);
            }

            dataSerializedObject.ApplyModifiedProperties();
        }
    }
}
#endif