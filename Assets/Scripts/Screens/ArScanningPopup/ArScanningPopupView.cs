using TMPro;
using UnityEngine;

namespace Screens.ArScanningPopup
{
    public interface IArScanningPopupView : IScreenView
    {
        void SetScannedProgressValue(float areaCoefficient);
    }

    public class ArScanningPopupView : ScreenView, IArScanningPopupView
    {
        [SerializeField] private TextMeshProUGUI areaText;

        public void SetScannedProgressValue(float areaCoefficient)
        {
            areaText.gameObject.SetActive(areaCoefficient is < 1 and >= 0);
            areaText.text = $"Scanned: {Mathf.RoundToInt(areaCoefficient * 100)}%";
        }
    }
}