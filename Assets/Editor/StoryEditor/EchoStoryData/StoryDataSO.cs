using UnityEngine;
using System.Collections.Generic;
using Rift.Story;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Rift.Echo.Story
{
    [CreateAssetMenu(fileName = "NewStoryData", menuName = "Rift Story/Echo Story Data")]
    public class StoryDataSO : ScriptableObject
    {
        [Header("基础信息")]
        public string sceneId;
        public string sceneName;

        [Header("背景设置")]
        public BackgroundData background = new BackgroundData();

        [Header("音频设置")]
        public AudioData audio = new AudioData();

        [Header("特效设置")]
        public List<EffectData> effects = new List<EffectData>();

        [Header("对话序列")]
        public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();

        [Header("元数据")]
        public StoryMetadata metadata = new StoryMetadata();

        #region 序列化/反序列化方法
        public string ToJson(bool prettyPrint = true)
        {
            var nodeData = new StoryNodeData
            {
                sceneId = this.sceneId,
                sceneName = this.sceneName,
                background = this.background,
                audio = this.audio,
                effects = this.effects,
                dialogueEntries = this.dialogueEntries
            };
            return JsonUtility.ToJson(nodeData, prettyPrint);
        }

        public void FromJson(string json)
        {
            var nodeData = JsonUtility.FromJson<StoryNodeData>(json);
            if (nodeData != null)
            {
                this.sceneId = nodeData.sceneId;
                this.sceneName = nodeData.sceneName;
                this.background = nodeData.background;
                this.audio = nodeData.audio;
                this.effects = nodeData.effects;
                this.dialogueEntries = nodeData.dialogueEntries;
            }
        }
        #endregion

        #region 编辑器工具方法
#if UNITY_EDITOR
        public void RefreshCharacterInfo()
        {
            for(int i = 0; i < dialogueEntries.Count;i ++)
            {
                if(dialogueEntries[i].GetCharacter() == null)
                {
                    continue;
                }
                dialogueEntries[i].speakerId = dialogueEntries[i].GetCharacter().CharacterID;
                dialogueEntries[i].speakerName = dialogueEntries[i].GetCharacter().CharacterName;
            }
        }
        public void SaveToJsonFile()
        {
            string path = EditorUtility.SaveFilePanel(
                "保存故事数据为JSON",
                Application.dataPath,
                $"{sceneId}_story.json",
                "json"
            );

            if (!string.IsNullOrEmpty(path))
            {
                string json = ToJson(true);
                System.IO.File.WriteAllText(path, json);
                Debug.Log($"故事数据已保存到: {path}");
                AssetDatabase.Refresh();
            }
        }

        public void LoadFromJsonFile()
        {
            string path = EditorUtility.OpenFilePanel(
                "加载JSON故事数据",
                Application.dataPath,
                "json"
            );

            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                string json = System.IO.File.ReadAllText(path);
                FromJson(json);
                EditorUtility.SetDirty(this);
                Debug.Log($"故事数据已从: {path} 加载");
            }
        }
#endif
        #endregion
    }

    #region 辅助类
    [System.Serializable]
    public class StoryMetadata
    {
        public string author = "";
        public string version = "1.0";
        public string createdDate = "";
        public string lastModified = "";
        public List<string> tags = new List<string>();
        public string description = "";
    }
    #endregion
}