using TMPro;
using UnityEngine;

namespace Screens.Views
{
    public class LocationInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI zoneNameText;

        public void SetActiveZoneName(string zoneName) => zoneNameText.text = zoneName;

        public void ShowResponse(string status)
        {
            
        }
    }
}