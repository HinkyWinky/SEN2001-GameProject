using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Scene Data", menuName = "Scriptable Object/Scene Data")]
    public class SceneData : ScriptableObject
    {
        [SerializeField] private new string name = default;
        [SerializeField] private int buildIndex = default;

        public string Name => name;
        public int BuildIndex => buildIndex;
    }
}
