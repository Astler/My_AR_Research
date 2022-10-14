using System;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.Screens.MainScreen
{
    public interface IMapUserInterface
    {
        event Action PortalsListClicked;
        event Action NearestPortalClicked;
        event Action MyPositionClicked;
    }
    
    public class MapUserInterfaceView : MonoBehaviour, IMapUserInterface
    {
        [SerializeField] private Button toMeButton;
        [SerializeField] private Button toNearestPortal;
        [SerializeField] private Button portalsListButton;

        public event Action PortalsListClicked;
        public event Action NearestPortalClicked;
        public event Action MyPositionClicked;
        
        private void Awake()
        {
            toMeButton.onClick.AddListener(() => MyPositionClicked?.Invoke());
            toNearestPortal.onClick.AddListener(() => NearestPortalClicked?.Invoke());
            portalsListButton.onClick.AddListener(() => PortalsListClicked?.Invoke());
        }
    }
}