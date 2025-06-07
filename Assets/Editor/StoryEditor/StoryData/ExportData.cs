#if UNITY_EDITOR
using System.Collections.Generic;
namespace Rift.Story
{
    [System.Serializable]
    public class ExportData
    {
        public List<SerializableCharacterData> characterData;
        public List<SerializableOperationValueData> operationValues;
        public List<SerializableStoryNodeData> storyNodes;
    }

    [System.Serializable]
    public class SerializableCharacterData
    {
        public string CharacterID;
        public string CharacterName;
        public string PortraitPath;
        public string AvatarPath;
    }

    [System.Serializable]
    public class SerializableOperationValueData
    {
        public string Guid;
        public ValueType ValueType;
        public int IntValue;
        public float FloatValue;
        public bool BoolValue;
        public void SetValue(object value)
        {
            switch (ValueType)
            {
                case ValueType.Boolean:
                    BoolValue = (bool)value;
                    break;
                case ValueType.Float:
                    FloatValue = (float)value;
                    break;
                case ValueType.Integer:
                    IntValue = (int)value;
                    break;
            }
        }
    }

    [System.Serializable]
    public class SerializableStoryNodeData
    {
        public string Guid;
        public List<string> ParentNodesGuid;
        public List<string> ChildNodesGuid;
        public ENodeType Type;
        public string CharacterID;
        public List<string> DialogContents;
        public List<SerializableChoiceData> Choices;
        public List<SerializableStoryEventData> StoryEvents;
        public string CustomData;
    }

    [System.Serializable]
    public class SerializableChoiceData
    {
        public string ChoiceTitle;
        public string NavigateToNodeID;
    }

    [System.Serializable]
    public class SerializableStoryEventData
    {
        public string OperationValueID;
        public EOperationModifyType OperationModifyType;
        public string OperationModifyValueID;
    }
}
#endif