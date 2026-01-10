#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Rift.Echo.Story;
namespace Rift.Story
{
    [Serializable]
    public class StoryNodeData : ScriptableObject
    {
        public string Guid;
        [SerializeField] public string Title;
        [SerializeField] public string Description;
        [SerializeField] public ENodeType Type;
        #region Type == ENodeType.Dialog
#if RIFT_ENABLE_ECHO
        [SerializeField] public StoryDataSO DialogData;
#else
        [SerializeField] public CharacterData Character;
        [SerializeField] public List<string> DialogContents;
#endif
        #endregion

        #region Type == ENodeType.Choice
        [SerializeField] public List<ChoiceData> Choices;
        [SerializeField] public bool AutoSelect;
        #endregion

        #region Type == ENodeType.Event
        [SerializeField] public List<StoryEventsData> StoryEvents;
        #endregion

        [SerializeField] public string CustomData;

    }
}
#endif