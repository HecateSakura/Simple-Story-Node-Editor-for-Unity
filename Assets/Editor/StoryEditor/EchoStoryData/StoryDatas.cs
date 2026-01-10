using System;
using System.Collections.Generic;
using UnityEngine;
using Rift.Story;
namespace Rift.Echo.Story
{
    [Serializable]
    public class StoryNodeData
    {
        // 基础信息
        public string sceneId; // 场景唯一标识
        public string sceneName; // 场景名称（可选）

        // 背景信息
        public BackgroundData background = new BackgroundData();

        // 立绘槽位（3个位置）
        // public List<CharacterSlotData> characterSlots = new List<CharacterSlotData>();

        // 音频设置
        public AudioData audio = new AudioData();

        // 特效和动画
        public List<EffectData> effects = new List<EffectData>();

        // 对话序列（每条对话记录）
        public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();
    }

    [Serializable]
    public class BackgroundData
    {
        public string backgroundId = ""; // 背景资源ID
        public string backgroundPath = ""; // 背景资源路径
        public ColorData tintColor = new ColorData(1, 1, 1, 1); // 背景色调
        public float brightness = 1.0f; // 亮度
        public float blurAmount = 0.0f; // 模糊程度
        public AnimationData animation = new AnimationData(); // 动画设置

        // 过渡效果
        public string transitionType = "fade"; // fade, slide, dissolve等
        public float transitionDuration = 1.0f;
    }

    [Serializable]
    public class CharacterSlotData
    {
        public int slotId; // 槽位ID：0, 1, 2
        public string position; // 位置标识：left, center, right

        // 立绘状态
        public bool isActive = false; // 是否激活（显示）
        public string characterId = ""; // 角色ID
        public string expressionId = "normal"; // 表情ID

        // 资源路径
        public string portraitPath = ""; // 立绘图片路径
        public string avatarPath = ""; // 小头像路径

        // 显示属性
        public Vector2Data positionOffset = new Vector2Data(0, 0);
        public Vector2Data scale = new Vector2Data(1, 1);
        public float alpha = 1.0f; // 透明度
        public ColorData tintColor = new ColorData(1, 1, 1, 1);
        public int sortingOrder = 0; // 渲染顺序

        // 动画
        public AnimationData entranceAnimation = new AnimationData(); // 入场动画
        public AnimationData exitAnimation = new AnimationData(); // 退场动画
        public AnimationData idleAnimation = new AnimationData(); // 闲置动画
        public AnimationData highlightAnimation = new AnimationData(); // 高亮动画

        // 特效
        public bool isHighlighted = false; // 是否高亮（当前说话者）
        public List<EffectData> effects = new List<EffectData>();
    }

    [Serializable]
    public class DialogueEntry
    {
        public string entryId; // 对话条目ID
        public int index; // 在序列中的索引

        // 说话者信息
        [SerializeField]
        private CharacterData character;
        public string speakerId = ""; // 说话者角色ID
        public string speakerName = ""; // 说话者显示名称
        public int speakerSlot = 0; // 说话者所在的槽位（0,1,2）
        public string speakerExpression = "normal"; // 说话时表情

        // 对话内容
        public string dialogueText = ""; // 对话文本（支持富文本）
        public string voiceClipPath = ""; // 语音文件路径
        public float voiceVolume = 1.0f; // 语音音量

        // 元数据
        public Dictionary<string, string> metadata = new Dictionary<string, string>();
    }

    [Serializable]
    public class AudioData
    {
        public string backgroundMusicPath = ""; // BGM路径
        public float backgroundMusicVolume = 0.7f;
        public bool loopBackgroundMusic = true;

        public string ambientSoundPath = ""; // 环境音效路径
        public float ambientSoundVolume = 0.3f;

        public List<SoundEffectData> soundEffects = new List<SoundEffectData>();
    }

    [Serializable]
    public class SoundEffectData
    {
        public string id;
        public string clipPath;
        public float volume = 1.0f;
        public bool loop = false;
        public float delay = 0f;
    }

    [Serializable]
    public class AnimationData
    {
        public string type = "none"; // none, fade, slide, scale, etc.
        public float duration = 1.0f;
        public string easing = "linear"; // linear, easeIn, easeOut, easeInOut
        public Vector2Data from = new Vector2Data(0, 0);
        public Vector2Data to = new Vector2Data(0, 0);
        public float fromAlpha = 1.0f;
        public float toAlpha = 1.0f;
    }

    [Serializable]
    public class EffectData
    {
        public string type; // shake, glow, pulse, etc.
        public float intensity = 1.0f;
        public float duration = 1.0f;
        public bool loop = false;
        public Dictionary<string, float> parameters = new Dictionary<string, float>();
    }

    // 辅助数据结构
    [Serializable]
    public class ColorData
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public ColorData(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    [Serializable]
    public class Vector2Data
    {
        public float x;
        public float y;

        public Vector2Data(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}