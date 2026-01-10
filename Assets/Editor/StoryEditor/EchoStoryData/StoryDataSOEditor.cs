#if UNITY_EDITOR
using Rift.Echo.Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Rift.Story
{
    [CustomEditor(typeof(StoryDataSO))]
    public class StoryDataSOEditor : Editor
    {
        #region 序列化属性
        private SerializedProperty _sceneIdProp;
        private SerializedProperty _sceneNameProp;
        private SerializedProperty _backgroundProp;
        private SerializedProperty _characterSlotsProp;
        private SerializedProperty _audioProp;
        private SerializedProperty _effectsProp;
        private SerializedProperty _dialogueEntriesProp;
        private SerializedProperty _metadataProp;
        #endregion

        #region 折叠状态
        private bool _showBackground = true;
        private bool _showCharacterSlots = true;
        private bool _showAudio = true;
        private bool _showEffects = true;
        private bool _showDialogueEntries = true;
        private bool _showMetadata = true;
        private bool _showTools = true;
        #endregion

        private void OnEnable()
        {
            _sceneIdProp = serializedObject.FindProperty("sceneId");
            _sceneNameProp = serializedObject.FindProperty("sceneName");
            _backgroundProp = serializedObject.FindProperty("background");
            _characterSlotsProp = serializedObject.FindProperty("characterSlots");
            _audioProp = serializedObject.FindProperty("audio");
            _effectsProp = serializedObject.FindProperty("effects");
            _dialogueEntriesProp = serializedObject.FindProperty("dialogueEntries");
            _metadataProp = serializedObject.FindProperty("metadata");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("故事数据编辑器", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            DrawBasicInfo();
            DrawBackgroundSection();
            // DrawCharacterSlotsSection();
            DrawAudioSection();
            DrawEffectsSection();
            DrawDialogueEntriesSection();
            // DrawMetadataSection();
            DrawToolbar();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBasicInfo()
        {
            EditorGUILayout.LabelField("基础信息", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(_sceneIdProp, new GUIContent("场景ID"));
            EditorGUILayout.PropertyField(_sceneNameProp, new GUIContent("场景名称"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);
        }

        private void DrawBackgroundSection()
        {
            _showBackground = EditorGUILayout.BeginFoldoutHeaderGroup(_showBackground, "背景设置");
            if (_showBackground)
            {
                EditorGUI.indentLevel++;
                DrawBackgroundData(_backgroundProp);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawBackgroundData(SerializedProperty backgroundProp)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(backgroundProp.FindPropertyRelative("backgroundId"),
                new GUIContent("背景ID"));
            EditorGUILayout.PropertyField(backgroundProp.FindPropertyRelative("backgroundPath"),
                new GUIContent("背景路径"));

            EditorGUILayout.LabelField("视觉效果", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            DrawColorProperty(backgroundProp.FindPropertyRelative("tintColor"), "色调");
            EditorGUILayout.PropertyField(backgroundProp.FindPropertyRelative("brightness"),
                new GUIContent("亮度"));
            EditorGUILayout.PropertyField(backgroundProp.FindPropertyRelative("blurAmount"),
                new GUIContent("模糊程度"));
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("动画效果", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("过渡效果", EditorStyles.boldLabel);
            EditorGUI.indentLevel--;
            EditorGUILayout.PropertyField(backgroundProp.FindPropertyRelative("transitionType"),
                new GUIContent("过渡类型"));
            EditorGUILayout.PropertyField(backgroundProp.FindPropertyRelative("transitionDuration"),
                new GUIContent("过渡时长"));
            EditorGUI.indentLevel++;

            EditorGUILayout.EndVertical();
        }

        private void DrawCharacterSlotsSection()
        {
            _showCharacterSlots = EditorGUILayout.BeginFoldoutHeaderGroup(_showCharacterSlots,
                $"立绘槽位 ({_characterSlotsProp.arraySize}/3)");
            if (_showCharacterSlots)
            {
                EditorGUI.indentLevel++;

                // 确保只有3个槽位
                if (_characterSlotsProp.arraySize != 3)
                {
                    _characterSlotsProp.arraySize = 3;
                }

                for (int i = 0; i < _characterSlotsProp.arraySize; i++)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField($"槽位 {i + 1}", EditorStyles.boldLabel);

                    SerializedProperty slotProp = _characterSlotsProp.GetArrayElementAtIndex(i);
                    DrawCharacterSlot(slotProp, i);
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawCharacterSlot(SerializedProperty slotProp, int slotIndex)
        {
            EditorGUILayout.BeginVertical("box");

            // 基本属性
            slotProp.FindPropertyRelative("slotId").intValue = slotIndex;
            EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("isActive"),
                new GUIContent("是否激活"));

            if (slotProp.FindPropertyRelative("isActive").boolValue)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("characterId"),
                    new GUIContent("角色ID"));
                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("expressionId"),
                    new GUIContent("表情ID"));
                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("position"),
                    new GUIContent("位置"));

                EditorGUILayout.LabelField("资源路径", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("portraitPath"),
                    new GUIContent("立绘路径"));
                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("avatarPath"),
                    new GUIContent("头像路径"));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("显示属性", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawVector2Property(slotProp.FindPropertyRelative("positionOffset"), "位置偏移");
                DrawVector2Property(slotProp.FindPropertyRelative("scale"), "缩放");
                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("alpha"),
                    new GUIContent("透明度"));
                DrawColorProperty(slotProp.FindPropertyRelative("tintColor"), "色调");
                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("sortingOrder"),
                    new GUIContent("渲染顺序"));
                EditorGUILayout.PropertyField(slotProp.FindPropertyRelative("isHighlighted"),
                    new GUIContent("高亮显示"));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("动画设置", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                DrawAnimationData(slotProp.FindPropertyRelative("entranceAnimation"), "入场动画");
                DrawAnimationData(slotProp.FindPropertyRelative("exitAnimation"), "退场动画");
                DrawAnimationData(slotProp.FindPropertyRelative("idleAnimation"), "闲置动画");
                DrawAnimationData(slotProp.FindPropertyRelative("highlightAnimation"), "高亮动画");
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("特效", EditorStyles.boldLabel);
                DrawEffectsList(slotProp.FindPropertyRelative("effects"));

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawAudioSection()
        {
            _showAudio = EditorGUILayout.BeginFoldoutHeaderGroup(_showAudio, "音频设置");
            if (_showAudio)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.LabelField("背景音乐", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_audioProp.FindPropertyRelative("backgroundMusicPath"),
                    new GUIContent("BGM路径"));
                EditorGUILayout.PropertyField(_audioProp.FindPropertyRelative("backgroundMusicVolume"),
                    new GUIContent("BGM音量"));
                EditorGUILayout.PropertyField(_audioProp.FindPropertyRelative("loopBackgroundMusic"),
                    new GUIContent("循环播放"));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("环境音效", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_audioProp.FindPropertyRelative("ambientSoundPath"),
                    new GUIContent("环境音路径"));
                EditorGUILayout.PropertyField(_audioProp.FindPropertyRelative("ambientSoundVolume"),
                    new GUIContent("环境音音量"));
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("音效列表", EditorStyles.boldLabel);
                DrawSoundEffectsList(_audioProp.FindPropertyRelative("soundEffects"));

                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawSoundEffectsList(SerializedProperty listProp)
        {
            listProp.isExpanded = EditorGUILayout.Foldout(listProp.isExpanded,
                $"音效 ({listProp.arraySize})");

            if (!listProp.isExpanded) return;

            EditorGUI.indentLevel++;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("box");
                SerializedProperty effectProp = listProp.GetArrayElementAtIndex(i);

                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("id"),
                    new GUIContent("音效ID"));
                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("clipPath"),
                    new GUIContent("音频路径"));
                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("volume"),
                    new GUIContent("音量"));
                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("loop"),
                    new GUIContent("循环"));
                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("delay"),
                    new GUIContent("延迟"));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加音效", GUILayout.Width(100)))
            {
                listProp.arraySize++;
            }
            if (GUILayout.Button("移除音效", GUILayout.Width(100)) && listProp.arraySize > 0)
            {
                listProp.arraySize--;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        private void DrawEffectsSection()
        {
            _showEffects = EditorGUILayout.BeginFoldoutHeaderGroup(_showEffects,
                $"全局特效 ({_effectsProp.arraySize})");
            if (_showEffects)
            {
                EditorGUI.indentLevel++;
                DrawEffectsList(_effectsProp);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawEffectsList(SerializedProperty listProp)
        {
            listProp.isExpanded = EditorGUILayout.Foldout(listProp.isExpanded,
                $"特效列表 ({listProp.arraySize})");

            if (!listProp.isExpanded) return;

            EditorGUI.indentLevel++;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("box");
                SerializedProperty effectProp = listProp.GetArrayElementAtIndex(i);

                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("type"),
                    new GUIContent("特效类型"));
                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("intensity"),
                    new GUIContent("强度"));
                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("duration"),
                    new GUIContent("时长"));
                EditorGUILayout.PropertyField(effectProp.FindPropertyRelative("loop"),
                    new GUIContent("循环"));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加特效", GUILayout.Width(100)))
            {
                listProp.arraySize++;
            }
            if (GUILayout.Button("移除特效", GUILayout.Width(100)) && listProp.arraySize > 0)
            {
                listProp.arraySize--;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        private void DrawDialogueEntriesSection()
        {
            _showDialogueEntries = EditorGUILayout.BeginFoldoutHeaderGroup(_showDialogueEntries,
                $"对话序列 ({_dialogueEntriesProp.arraySize})");
            if (_showDialogueEntries)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < _dialogueEntriesProp.arraySize; i++)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField($"对话 {i + 1}", EditorStyles.boldLabel);

                    SerializedProperty entryProp = _dialogueEntriesProp.GetArrayElementAtIndex(i);
                    DrawDialogueEntry(entryProp, i);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("添加对话", GUILayout.Width(100)))
                {
                    _dialogueEntriesProp.arraySize++;
                }
                if (GUILayout.Button("移除对话", GUILayout.Width(100)) && _dialogueEntriesProp.arraySize > 0)
                {
                    _dialogueEntriesProp.arraySize--;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawDialogueEntry(SerializedProperty entryProp, int index)
        {
            EditorGUILayout.BeginVertical("box");

            // 基本属性
            entryProp.FindPropertyRelative("entryId").stringValue = $"dialog_{index}";
            entryProp.FindPropertyRelative("index").intValue = index;

            EditorGUILayout.LabelField("说话者信息", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(entryProp.FindPropertyRelative("character"),
                new GUIContent("角色SO"));
            var characterSO = entryProp.FindPropertyRelative("character").objectReferenceValue as CharacterData;
            if (characterSO != null)
            {
                EditorGUI.indentLevel++;
                entryProp.FindPropertyRelative("speakerId").stringValue = characterSO.CharacterID;
                entryProp.FindPropertyRelative("speakerName").stringValue = characterSO.CharacterName;
                EditorGUILayout.LabelField("角色ID", characterSO.CharacterID);
                EditorGUILayout.LabelField("显示名称", characterSO.CharacterName);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(entryProp.FindPropertyRelative("speakerSlot"),
                new GUIContent("槽位(0-2)"));
            EditorGUILayout.PropertyField(entryProp.FindPropertyRelative("speakerExpression"),
                new GUIContent("表情"));
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("对话内容", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(entryProp.FindPropertyRelative("dialogueText"),
                new GUIContent("文本"), GUILayout.Height(60));
            EditorGUILayout.PropertyField(entryProp.FindPropertyRelative("voiceClipPath"),
                new GUIContent("语音路径"));
            EditorGUILayout.PropertyField(entryProp.FindPropertyRelative("voiceVolume"),
                new GUIContent("语音音量"));
            EditorGUI.indentLevel--;

            // EditorGUILayout.LabelField("触发事件", EditorStyles.boldLabel);
            // DrawTriggerActionsList(entryProp.FindPropertyRelative("triggers"));

            // EditorGUILayout.LabelField("元数据", EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();
        }

        private void DrawTriggerActionsList(SerializedProperty listProp)
        {
            listProp.isExpanded = EditorGUILayout.Foldout(listProp.isExpanded,
                $"触发事件 ({listProp.arraySize})");

            if (!listProp.isExpanded) return;

            EditorGUI.indentLevel++;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical("box");
                SerializedProperty triggerProp = listProp.GetArrayElementAtIndex(i);

                EditorGUILayout.PropertyField(triggerProp.FindPropertyRelative("type"),
                    new GUIContent("事件类型"));
                EditorGUILayout.PropertyField(triggerProp.FindPropertyRelative("targetId"),
                    new GUIContent("目标ID"));

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加事件", GUILayout.Width(100)))
            {
                listProp.arraySize++;
            }
            if (GUILayout.Button("移除事件", GUILayout.Width(100)) && listProp.arraySize > 0)
            {
                listProp.arraySize--;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        private void DrawMetadataSection()
        {
            _showMetadata = EditorGUILayout.BeginFoldoutHeaderGroup(_showMetadata, "元数据");
            if (_showMetadata)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.PropertyField(_metadataProp.FindPropertyRelative("author"),
                    new GUIContent("作者"));
                EditorGUILayout.PropertyField(_metadataProp.FindPropertyRelative("version"),
                    new GUIContent("版本"));
                EditorGUILayout.PropertyField(_metadataProp.FindPropertyRelative("description"),
                    new GUIContent("描述"), GUILayout.Height(40));

                EditorGUILayout.LabelField("标签", EditorStyles.boldLabel);
                DrawTagsList(_metadataProp.FindPropertyRelative("tags"));

                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space(5);
        }

        private void DrawTagsList(SerializedProperty listProp)
        {
            listProp.isExpanded = EditorGUILayout.Foldout(listProp.isExpanded,
                $"标签 ({listProp.arraySize})");

            if (!listProp.isExpanded) return;

            EditorGUI.indentLevel++;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                EditorGUILayout.PropertyField(listProp.GetArrayElementAtIndex(i),
                    new GUIContent($"标签 {i + 1}"));
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("添加标签", GUILayout.Width(100)))
            {
                listProp.arraySize++;
            }
            if (GUILayout.Button("移除标签", GUILayout.Width(100)) && listProp.arraySize > 0)
            {
                listProp.arraySize--;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        private void DrawToolbar()
        {
            _showTools = EditorGUILayout.BeginFoldoutHeaderGroup(_showTools, "工具");
            if (_showTools)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("导出为JSON", GUILayout.Height(30)))
                {
                    StoryDataSO dataSO = (StoryDataSO)target;
                    dataSO.SaveToJsonFile();
                }

                if (GUILayout.Button("从JSON导入", GUILayout.Height(30)))
                {
                    StoryDataSO dataSO = (StoryDataSO)target;
                    dataSO.LoadFromJsonFile();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("复制JSON到剪贴板", GUILayout.Height(25)))
                {
                    StoryDataSO dataSO = (StoryDataSO)target;
                    string json = dataSO.ToJson(true);
                    GUIUtility.systemCopyBuffer = json;
                    Debug.Log("JSON已复制到剪贴板");
                }

                if (GUILayout.Button("预览JSON", GUILayout.Height(25)))
                {
                    StoryDataSO dataSO = (StoryDataSO)target;
                    string json = dataSO.ToJson(true);
                    ShowJsonPreviewWindow(json);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        #region 辅助绘制方法
        private void DrawColorProperty(SerializedProperty colorProp, string label)
        {
            SerializedProperty r = colorProp.FindPropertyRelative("r");
            SerializedProperty g = colorProp.FindPropertyRelative("g");
            SerializedProperty b = colorProp.FindPropertyRelative("b");
            SerializedProperty a = colorProp.FindPropertyRelative("a");

            Color currentColor = new Color(r.floatValue, g.floatValue, b.floatValue, a.floatValue);
            Color newColor = EditorGUILayout.ColorField(label, currentColor);

            if (newColor != currentColor)
            {
                r.floatValue = newColor.r;
                g.floatValue = newColor.g;
                b.floatValue = newColor.b;
                a.floatValue = newColor.a;
            }
        }

        private void DrawVector2Property(SerializedProperty vectorProp, string label)
        {
            SerializedProperty x = vectorProp.FindPropertyRelative("x");
            SerializedProperty y = vectorProp.FindPropertyRelative("y");

            Vector2 currentVector = new Vector2(x.floatValue, y.floatValue);
            Vector2 newVector = EditorGUILayout.Vector2Field(label, currentVector);

            if (newVector != currentVector)
            {
                x.floatValue = newVector.x;
                y.floatValue = newVector.y;
            }
        }

        private void DrawAnimationData(SerializedProperty animProp, string label = null)
        {
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
            }

            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("type"),
                new GUIContent("动画类型"));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("duration"),
                new GUIContent("时长"));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("easing"),
                new GUIContent("缓动类型"));

            DrawVector2Property(animProp.FindPropertyRelative("from"), "起始位置");
            DrawVector2Property(animProp.FindPropertyRelative("to"), "结束位置");

            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("fromAlpha"),
                new GUIContent("起始透明度"));
            EditorGUILayout.PropertyField(animProp.FindPropertyRelative("toAlpha"),
                new GUIContent("结束透明度"));

            if (!string.IsNullOrEmpty(label))
            {
                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region 工具窗口
        private void ShowJsonPreviewWindow(string json)
        {
            JsonPreviewWindow.ShowWindow(json, _sceneIdProp.stringValue);
        }
        #endregion
    }

    // JSON预览窗口
    public class JsonPreviewWindow : EditorWindow
    {
        private string _jsonContent;
        private Vector2 _scrollPosition;
        private string _windowTitle;

        public static void ShowWindow(string json, string title = "JSON预览")
        {
            var window = GetWindow<JsonPreviewWindow>(true, title, true);
            window._jsonContent = json;
            window._windowTitle = title;
            window.minSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(_windowTitle, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.TextArea(_jsonContent, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("复制到剪贴板", GUILayout.Height(30)))
            {
                GUIUtility.systemCopyBuffer = _jsonContent;
                Debug.Log("JSON已复制到剪贴板");
            }

            if (GUILayout.Button("关闭", GUILayout.Height(30)))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif