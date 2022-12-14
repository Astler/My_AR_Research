using System;
using Prototype.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Screens.PortalsListScreen
{
    public class PortalCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI portalName;
        [SerializeField] private TextMeshProUGUI distance;
        [SerializeField] private Button moveToButton;
        private Vector2 _coordinates;

        public event Action<Vector2> MoveToClicked;

        public void ConfigureView(PortalViewInfo viewInfo)
        {
            portalName.text = viewInfo.Name;
            distance.text = viewInfo.Distance;
            _coordinates = viewInfo.Coordinates;
        }

        private void Awake()
        {
            moveToButton.onClick.AddListener(() => MoveToClicked?.Invoke(_coordinates));
        }

        public void DestroyCard()
        {
            Destroy(gameObject);
        }
    }
}