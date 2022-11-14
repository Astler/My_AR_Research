using UnityEngine;

namespace AR.World
{
    public class PortalController : MonoBehaviour
    {
        [SerializeField] private GiftsController giftsController;
        [SerializeField] private int totalGiftsInPortal;
        [SerializeField] private float placementHeight = 3f;

        [Space, Header("Resources"), SerializeField]
        private GiftsPortalView portalPrefab;

        private GiftsPortalView _spawnedPortal;
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            _spawnedPortal = Instantiate(portalPrefab, _transform);
        }

        public void OpenPortalInPosition(Vector3 portalPosition, Vector3 ceilPosition)
        {
            float height = placementHeight;

            if (ceilPosition != Vector3.zero)
            {
                float distanceToCeil = Vector3.Distance(portalPosition, ceilPosition);

                if (placementHeight > distanceToCeil)
                {
                    height = distanceToCeil - 0.5f;
                }
            }

            Vector3 updatedPosition = portalPosition + Vector3.up * height;

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