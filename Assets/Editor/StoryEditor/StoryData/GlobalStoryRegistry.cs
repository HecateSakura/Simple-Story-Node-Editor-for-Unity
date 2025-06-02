#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
namespace Rift.Story
{
    public class GlobalStoryRegistry : ScriptableObject
    {
        public List<CharacterData> CharacterDataList = new List<CharacterData>();
        public List<OperationValueData> OperationValueDataList = new List<OperationValueData>();
    }
}
#endif