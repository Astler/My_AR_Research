using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

namespace AR.Map
{
    public class SpawnOnMapAR : MonoBehaviour
    {
        public AbstractMap Map;
        public Vector2d SpawnPoint;

        [SerializeField] float spawnScale = 1f;

        private Transform _transform;

        void Start()
        {
            _transform = transform;

            if (!Map) return;

            _transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);
        }

        private void Update()
        {
            if (!Map) return;

            _transform.localPosition = Map.GeoToWorldPosition(SpawnPoint, true);
            _transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);
        }
    }
}