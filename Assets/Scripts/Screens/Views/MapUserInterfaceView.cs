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
        event Action MapCloseClicked;
        event Action RewardsListClicked;
    }

    public class MapUserInterfaceView : MonoBehaviour, IMapUserInterface
    {
        [SerializeField] private Button toMeButton;
        [SerializeField] private Button toNearestPortal;
        [SerializeField] private Button zonesListButton;
        [SerializeField] private Button rewardsListButton;
        [SerializeField] private Button closeMap;

        [SerializeField] private GameObject mapUIContent;
        [SerializeField] private GameObject mapUIBottomBar;

        public event Action PortalsListClicked;
        public event Action NearestPortalClicked;
        public event Action MyPositionClicked;
        public event Action MapCloseClicked;
        public event Action RewardsListClicked;

        public void SetIsRewardsButtonActive(bool hasZone) => rewardsListButton.gameObject.SetActive(hasZone);

        public void SetActive(bool isMapActive)
        {
            mapUIContent.SetActive(isMapActive);
            mapUIBottomBar.SetActive(isMapActive);
        }

        private void Awake()
        {
            toMeButton.onClick.AddListener(() => MyPositionClicked?.Invoke());
            toNearestPortal.onClick.AddListener(() => NearestPortalClicked?.Invoke());
            zonesListButton.onClick.AddListener(() => PortalsListClicked?.Invoke());
            rewardsListButton.onClick.AddListener(() => RewardsListClicked?.Invoke());
            closeMap.onClick.AddListener(() => MapCloseClicked?.Invoke());
        }
    }
}