using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Anim Data", menuName = "Scriptable Object/Anim Data")]
    public class AnimData : ScriptableObject
    {
        public bool isClip;
        [ShowIf("isClip")] public AnimationClip clip;
        [SerializeField, HideIf("isClip")] private string clipName = default;
        public bool isLoop;
        [Range(0f, 50f)] public float duration;
        [Range(0f, 1f)] public float normalizedFadeDuration;

        public string AnimName => isClip ? clip.name : clipName;
        public float Length => isClip ? clip.length : 0f;
    }
}
