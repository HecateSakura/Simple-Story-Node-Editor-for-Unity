#if UNITY_EDITOR
using System;
using UnityEngine;
namespace Rift.Story
{
    [Serializable]
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Rift Story/Data/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public string CharacterID;
        [SerializeField] public string CharacterName;
        [SerializeField] public string CharacterDescription;
        [SerializeField] public Texture2D Portrait;
        [SerializeField] public Texture2D Avatar;
    }
}
#endif