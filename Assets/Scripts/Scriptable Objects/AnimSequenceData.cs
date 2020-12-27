using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Anim Sequence Data", menuName = "Scriptable Object/Anim Sequence Data")]
    public class AnimSequenceData : ScriptableObject
    {
        public List<AnimData> animsData;
        [SerializeField] private string sequenceName = default;
        public bool isLoop;
        [Range(0f, 50f)] public float duration;
        [Range(0f, 1f)] public float normalizedFadeDuration;

        public string SequenceName => sequenceName;
        public float Length => animsData.Sum(t => t.Length);

        public AnimData ReturnAnimData(string animName)
        {
            return animsData.Find(t => t.AnimName == animName);
        }
    }
}
