using System;
using Prototype.Assets;
using TMPro;
using UnityEngine;

namespace Prototype.Location
{
    public class LocationInfoView: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI responseText;
        [SerializeField] private TextMeshProUGUI zoneNameText;
        [SerializeField] private TextMeshProUGUI allZonesText;
        
        public void ShowResponse(string text)
        {
            responseText.text = text;
        }

        public void SetActiveZoneName(string zoneName)
        {
            zoneNameText.text = zoneName;
        }

        public void ShowAllZones(string zoneName)
        {
            allZonesText.gameObject.SetActive(true);
            allZonesText.text = zoneName;
        }
        
        public void HideAllZonesList()
        {
            allZonesText.gameObject.SetActive(false);
        }

        private void Awake()
        {
            allZonesText.text = "";
            responseText.text = "";
        }
    }
}