#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace Rift.Story
{
    public class ExportWindow : EditorWindow
    {
        private bool _compressAndEncode = false;
        private string _exportPath = "";
        private StoryEditorWindow _parentWindow;

        public static void OpenWindow(StoryEditorWindow parent)
        {
            var window = GetWindow<ExportWindow>("导出项目");
            window._parentWindow = parent;
            window.minSize = new Vector2(400, 150);
        }

        private void OnGUI()
        {
            GUILayout.Label("导出设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // 压缩加密选项
            _compressAndEncode = EditorGUILayout.Toggle("Base64加密压缩", _compressAndEncode);
            EditorGUILayout.HelpBox("启用后将对导出的JSON文件进行GZip压缩和Base64编码", MessageType.Info);

            EditorGUILayout.Space(15);

            // 路径选择区域
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("导出路径:", GUILayout.Width(70));
                EditorGUILayout.TextField(_exportPath);

                if (GUILayout.Button("浏览...", GUILayout.Width(60)))
                {
                    string extension = _compressAndEncode ? "story" : "json";
                    string path = EditorUtility.SaveFilePanel(
                        "选择导出位置",
                        Application.dataPath,
                        "StoryProject_" + DateTime.Now.ToString("yyyyMMdd_HHmm"),
                        extension);

                    if (!string.IsNullOrEmpty(path))
                    {
                        _exportPath = path;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(20);

            // 导出按钮
            GUI.enabled = !string.IsNullOrEmpty(_exportPath);
            if (GUILayout.Button("导出项目", GUILayout.Height(30)))
            {
                ExportProject();
            }
            GUI.enabled = true;
        }

        private void ExportProject()
        {
            try
            {
                // 1. 收集所有需要导出的数据
                ExportData exportData = PopulateExportData();
                // 2. 序列化为JSON
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(exportData);
                byte[] data = Encoding.UTF8.GetBytes(json);

                // 3. 处理压缩/编码
                if (_compressAndEncode)
                {
                    // 先压缩
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var gzipStream = new GZipStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal))
                        {
                            gzipStream.Write(data, 0, data.Length);
                        }
                        data = memoryStream.ToArray();
                    }

                    // 再Base64编码
                    string base64 = Convert.ToBase64String(data);
                    File.WriteAllText(_exportPath, base64);
                }
                else
                {
                    File.WriteAllBytes(_exportPath, data);
                }

                EditorUtility.DisplayDialog("导出成功", $"项目已导出到:\n{_exportPath}", "确定");
                Close();
            }
            catch (Exception ex)
            {
                Debug.LogError($"导出失败: {ex.Message}");
                EditorUtility.DisplayDialog("导出错误", $"导出过程中发生错误:\n{ex.Message}", "关闭");
            }
        }

        private ExportData PopulateExportData()
        {
            ExportData exportData = new ExportData();
            // 收集角色数据
            List<CharacterData> originalCharacter = _parentWindow.GetCharacterData();
            List<SerializableCharacterData> serializableCharacter = new List<SerializableCharacterData>();
            for (int i = 0; i < originalCharacter.Count; i++)
            {
                serializableCharacter.Add(
                    new SerializableCharacterData()
                    {
                        CharacterID = originalCharacter[i].CharacterID.ToString(),
                        CharacterName = originalCharacter[i].CharacterName,
                        PortraitPath = AssetDatabase.GetAssetPath(originalCharacter[i].Portrait),
                        AvatarPath = AssetDatabase.GetAssetPath(originalCharacter[i].Avatar)
                    });
            }
            exportData.characterData = serializableCharacter;

            // 收集操作值数据
            List<OperationValueData> originalOperationValue = _parentWindow.GetOperationValues();
            List<SerializableOperationValueData> serializableOperationValue = new List<SerializableOperationValueData>();
            for (int i = 0; i < originalOperationValue.Count; i++)
            {
                serializableOperationValue.Add(
                    new SerializableOperationValueData()
                    {
                        Guid = originalOperationValue[i].Guid.ToString(),
                        ValueType = originalOperationValue[i].ValueType,
                    });
                serializableOperationValue[i].SetValue(originalOperationValue[i].GetValue());
            }
            exportData.operationValues = serializableOperationValue;

            // 收集故事节点数据
            Dictionary<Guid, StoryNodeData> originalStoryNodes = _parentWindow.GetStoryNodes();
            List<SerializableStoryNodeData> serializableNodeData = new List<SerializableStoryNodeData>();
            int index = 0;
            foreach (var key in originalStoryNodes.Keys)
            {
                var node = originalStoryNodes[key];
                serializableNodeData.Add(new SerializableStoryNodeData()
                {
                    Guid = key.ToString(),
                    Type = node.Type,
                    CustomData = node.CustomData,
                });

                switch (node.Type)
                {
                    case ENodeType.Dialog:
                        serializableNodeData[index].CharacterID = node.Character.CharacterID;
                        serializableNodeData[index].DialogContent = node.DialogContent;
                        break;
                    case ENodeType.Event:
                        PopulateEventData(serializableNodeData[index], node);
                        break;
                    case ENodeType.Choice:
                        PopulateChoiceData(serializableNodeData[index], node);
                        break;
                }
                index++;
            }
            exportData.storyNodes = serializableNodeData;
            return exportData;
        }
        private void PopulateEventData(SerializableStoryNodeData serializable, StoryNodeData originalData)
        {
            serializable.StoryEvents = new List<SerializableStoryEventData>();
            foreach (var eventData in originalData.StoryEvents)
            {
                serializable.StoryEvents.Add(new SerializableStoryEventData()
                {
                    OperationModifyType = eventData.OperationModifyType,
                    OperationValueID = eventData.OperationValue.Guid,
                    OperationModifyValueID = eventData.OperationModifyValue.Guid,
                });
            }
        }
        private void PopulateChoiceData(SerializableStoryNodeData serializable, StoryNodeData originalData)
        {
            serializable.Choices = new List<SerializableChoiceData>();
            foreach (var choice in originalData.Choices)
            {
                serializable.Choices.Add(new SerializableChoiceData()
                {
                    ChoiceTitle = choice.ChoiceTitle,
                    NavigateToNodeID = choice.NavigateToNode.Guid,
                });
            }
        }
    }
}
#endif