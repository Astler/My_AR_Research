using Items;
using UnityEngine;

namespace Prototype.World
{
    public class PortalController : MonoBehaviour
    {
        [SerializeField] private GiftsController giftsController;
        [SerializeField] private int totalGiftsInPortal;
        [SerializeField] private float portalHeight = 3f;

        [Space, Header("Resources"), SerializeField]
        private GiftsPortalView portalPrefab;

        private GiftsPortalView _spawnedPortal;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _spawnedPortal = Instantiate(portalPrefab, _transform);
        }

        public void OpenPortalInPosition(Vector3 portalPosition)
        {
            Vector3 updatedPosition = portalPosition + Vector3.up * portalHeight;

            _spawnedPortal.SetPosition(updatedPosition);

            _spawnedPortal.OpenPortal(() =>
                giftsController.SpawnGifts(totalGiftsInPortal, updatedPosition, ClosePortal));
        }

        public void ClosePortal()
        {
            if (!_spawnedPortal) return;

            _spawnedPortal.ClosePortal();
        }
    }
}