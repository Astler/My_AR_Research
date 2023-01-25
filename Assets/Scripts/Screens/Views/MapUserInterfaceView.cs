using System;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Views
{
    public interface IMapUserInterface
    {
        event Action NearestPortalClicked;
    }

    public class MapUserInterfaceView : MonoBehaviour, IMapUserInterface
    {
        [SerializeField] private Button toNearestPortal;

        public event Action NearestPortalClicked;
        
        private void Awake()
        {
            toNearestPortal.onClick.AddListener(() => NearestPortalClicked?.Invoke());
        }
    }
}