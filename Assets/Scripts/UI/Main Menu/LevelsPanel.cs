using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.UI
{
    public class LevelsPanel : Panel
    {
        [Header("Levels Buttons")]
        public CanvasGroup levelButtonsGroup;
        [HideInInspector] public List<ButtonX> levelButtons = new List<ButtonX>();
        public ButtonX backButton;

        [Header("Animation")]
        [SerializeField] private float timeOpenCloseAnimation = 0.5f;

        #region Mono
        public override void Awake()
        {
            base.Awake();
            levelButtons = levelButtonsGroup.GetComponentsInChildren<ButtonX>().ToList();
        }
        private void Start()
        {
            backButton.AddListeners(BackButtonOnDown, BackButtonOnUp);
            for (int i = 0; i < levelButtons.Count; i++)
            {
                int levelNo = i + 1;
                levelButtons[i].AddListeners(delegate { LevelButtonOnDown(levelNo); }, delegate { LevelButtonOnUp(levelNo); });
            }

            SetLevelButtons();
            Activate(false);
        }
        private void OnDestroy()
        {
            backButton.RemoveAllListeners();
            for (int i = 0; i < levelButtons.Count; i++)
                levelButtons[i].RemoveAllListeners();
        }
        #endregion

        private void SetLevelButtons()
        {
            for (int i = 0; i < levelButtons.Count; i++)
            {
                if (i == 0)
                {
                    levelButtons[i].interactable = true;
                    continue;
                }
                levelButtons[i].interactable = GameManager.Cur.GameDatabase.levelsCompletionStatue[i]; // look level x - 1 value for level x button
            }
        }

        public override void Activate(bool value)
        {
            gameObject.SetActive(value);
        }

        #region Level Buttons
        private void LevelButtonOnDown(int levelNo)
        {

        }
        private void LevelButtonOnUp(int levelNo)
        {
            StartCoroutine(GameManager.Cur.SceneCtrl.LoadLevelScene(levelNo, true));
        }
        #endregion

        #region Back Button
        private void BackButtonOnDown()
        {

        }
        private void BackButtonOnUp()
        {
            StartCoroutine(BackButtonOnUpCor());
        }
        private IEnumerator BackButtonOnUpCor()
        {
            GameManager.Cur.MainMenuCanvas.mainMenuPanel.Activate(true);
            yield return StartCoroutine(StartCloseAnimation(true));
            Activate(false);
        }
        #endregion

        #region Animations
        public override IEnumerator OpenAnimation()
        {
            GameManager.Cur.Canvas.ActivateUIInput(false);
            yield return StartCoroutine(ScalePanel(
                Vector3.zero, rectTransform.localScale, timeOpenCloseAnimation));
            GameManager.Cur.Canvas.ActivateUIInput(true);
        }
        public override IEnumerator CloseAnimation()
        {
            GameManager.Cur.Canvas.ActivateUIInput(false);
            yield return StartCoroutine(ScalePanel(
                rectTransform.localScale, Vector3.zero, timeOpenCloseAnimation));
            GameManager.Cur.Canvas.ActivateUIInput(true);
        }
        #endregion
    }
}

