using Game.AI;
using Game.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class Subscene : MonoBehaviour
    {
        [SerializeField] private SceneTypes sceneType = default;
        public SceneTypes SceneType => sceneType;

        [SerializeField] private CanvasX canvas = default;
        public CanvasX Canvas => canvas;

        [SerializeField, HideIf("sceneType", SceneTypes.MAINMENU)]
        private CamCtrl camCtrl = default;
        public CamCtrl CamCtrl => camCtrl;

        [SerializeField, HideIf("sceneType", SceneTypes.MAINMENU)]
        private Player player = default;
        public Player Player => player;

        [SerializeField, HideIf("sceneType", SceneTypes.MAINMENU)]
        private StateMachine enemy = default;
        public StateMachine Enemy => enemy;

        private bool firstTime = false;

        private void Start()
        {
            Debug.Log("LEVEL SCENE: Start()");
            GameManager.Cur.SetSubscene(SceneType, this);
        }

        private void Update()
        {
            if (!firstTime)
            {
                firstTime = true;
                Debug.Log("LEVEL SCENE: Update()");
            }
        }
    }
}
