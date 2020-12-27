using System;
using UnityEngine;

namespace Game
{
    public class SettingCtrl : MonoBehaviour
    {
        [SerializeField] private FrameRateSetting frameRateSetting = FrameRateSetting.DEFAULT;

        #region Frame Rate Settings
        public void SetFrameRate()
        {
            switch (frameRateSetting)
            {
                case FrameRateSetting.DEFAULT:
                    DefaultFrameRate();
                    break;
                case FrameRateSetting.RATE_30:
                    ConstantFrameRate(30);
                    break;
                case FrameRateSetting.RATE_45:
                    ConstantFrameRate(45);
                    break;
                case FrameRateSetting.RATE_60:
                    ConstantFrameRate(60);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        // Default Frame Rate
        private void DefaultFrameRate()
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
            Time.fixedDeltaTime = 1 / 60f;
        }
        // Cpu Gpu Constant Frame Rate
        private void ConstantFrameRate(int targetFrameRate)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
            Time.fixedDeltaTime = 1 / (float)targetFrameRate;
        }
        #endregion
    }
}
