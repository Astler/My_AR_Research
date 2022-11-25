using System;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Views
{
    public interface IMapUserInterface
    {
        event Action PortalsListClicked;
        event Action NearestPortalClicked;
        event Action MyPositionClicked;
        public event Action MapCloseClicked;
    }
    
    public class MapUserInterfaceView : MonoBehaviour, IMapUserInterface
    {
        [SerializeField] private Button toMeButton;
        [SerializeField] private Button toNearestPortal;
        [SerializeField] private Button portalsListButton;
        [SerializeField] private Button closeMap;

        public event Action PortalsListClicked;
        public event Action NearestPortalClicked;
        public event Action MyPositionClicked;
        public event Action MapCloseClicked;
        
        private void Awake()
        {
            toMeButton.onClick.AddListener(() => MyPositionClicked?.Invoke());
            toNearestPortal.onClick.AddListener(() => NearestPortalClicked?.Invoke());
            portalsListButton.onClick.AddListener(() => PortalsListClicked?.Invoke());
            closeMap.onClick.AddListener(() => MapCloseClicked?.Invoke());
        }
    }
}