using Rift.Echo.Story;
using Rift.Story;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Rift.Story
{
    [CustomEditor(typeof(CharacterData))]
    public class CharacterDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认属性
            DrawDefaultInspector();

            GUILayout.Space(20);

            // 添加查找和刷新按钮
            if (GUILayout.Button("查找并刷新引用此角色的StoryDataSO", GUILayout.Height(30)))
            {
                FindAndRefreshStoryDataSOs();
            }

            // 可选：显示查找结果
            if (GUILayout.Button("仅查找引用此角色的StoryDataSO", GUILayout.Height(25)))
            {
                FindStoryDataSOs();
            }
        }

        private void FindAndRefreshStoryDataSOs()
        {
            CharacterData characterData = (CharacterData)target;
            List<StoryDataSO> storyDataSOs = FindStoryDataSOs(characterData);

            if (storyDataSOs.Count == 0)
            {
                Debug.Log($"未找到引用角色 '{characterData.CharacterName}' 的 StoryDataSO");
                return;
            }

            Debug.Log($"找到 {storyDataSOs.Count} 个引用了角色 '{characterData.CharacterName}' 的 StoryDataSO，开始刷新...");

            // 记录刷新前状态
            int refreshedCount = 0;

            foreach (StoryDataSO storyData in storyDataSOs)
            {
                if (storyData != null)
                {
                    // 调用刷新方法
                    storyData.RefreshCharacterInfo();
                    refreshedCount++;

                    // 标记为脏，确保保存修改
                    EditorUtility.SetDirty(storyData);
                }
            }

            // 保存所有修改的资源
            AssetDatabase.SaveAssets();

            Debug.Log($"成功刷新了 {refreshedCount} 个 StoryDataSO");

            // 选择所有刷新过的资源
            // Selection.objects = storyDataSOs.Cast<Object>().ToArray();
        }

        private void FindStoryDataSOs()
        {
            CharacterData characterData = (CharacterData)target;
            List<StoryDataSO> storyDataSOs = FindStoryDataSOs(characterData);

            if (storyDataSOs.Count == 0)
            {
                Debug.Log($"未找到引用角色 '{characterData.CharacterName}' 的 StoryDataSO");
                return;
            }

            Debug.Log($"找到 {storyDataSOs.Count} 个引用了角色 '{characterData.CharacterName}' 的 StoryDataSO:");
            foreach (StoryDataSO storyData in storyDataSOs)
            {
                Debug.Log($"- {AssetDatabase.GetAssetPath(storyData)}", storyData);
            }

            // 选中所有找到的 StoryDataSO
            // Selection.objects = storyDataSOs.Cast<Object>().ToArray();
        }

        private List<StoryDataSO> FindStoryDataSOs(CharacterData characterData)
        {
            List<StoryDataSO> result = new List<StoryDataSO>();

            string targetPath = AssetDatabase.GetAssetPath(characterData);
            string targetGuid = AssetDatabase.AssetPathToGUID(targetPath);

            // 查找所有 StoryDataSO 文件
            string[] guids = AssetDatabase.FindAssets("t:StoryDataSO");

            EditorUtility.DisplayProgressBar("查找引用", "扫描 StoryDataSO 文件...", 0);

            try
            {
                for (int i = 0; i < guids.Length; i++)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    EditorUtility.DisplayProgressBar("查找引用",
                        $"扫描中 ({i + 1}/{guids.Length}): {System.IO.Path.GetFileName(assetPath)}",
                        (float)i / guids.Length);

                    // 加载 StoryDataSO
                    StoryDataSO storyData = AssetDatabase.LoadAssetAtPath<StoryDataSO>(assetPath);
                    if (storyData == null) continue;

                    // 检查是否引用了目标 CharacterData
                    if (CheckStoryDataSOForReference(storyData, targetGuid))
                    {
                        result.Add(storyData);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            return result;
        }

        private bool CheckStoryDataSOForReference(StoryDataSO storyData, string targetGuid)
        {
            // 使用 SerializedObject 检查所有属性
            SerializedObject so = new SerializedObject(storyData);
            SerializedProperty prop = so.GetIterator();

            bool hasReference = false;

            while (prop.Next(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference)
                {
                    if (prop.objectReferenceValue != null)
                    {
                        string refPath = AssetDatabase.GetAssetPath(prop.objectReferenceValue);
                        string refGuid = AssetDatabase.AssetPathToGUID(refPath);

                        if (refGuid == targetGuid)
                        {
                            hasReference = true;
                            break;
                        }
                    }
                }

                // 如果是数组或列表，检查数组元素
                if (prop.isArray && prop.arraySize > 0)
                {
                    for (int i = 0; i < prop.arraySize; i++)
                    {
                        SerializedProperty element = prop.GetArrayElementAtIndex(i);
                        if (element.propertyType == SerializedPropertyType.ObjectReference)
                        {
                            if (element.objectReferenceValue != null)
                            {
                                string refPath = AssetDatabase.GetAssetPath(element.objectReferenceValue);
                                string refGuid = AssetDatabase.AssetPathToGUID(refPath);

                                if (refGuid == targetGuid)
                                {
                                    hasReference = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (hasReference) break;
            }

            return hasReference;
        }
    }
}