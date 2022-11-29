using System;
using TMPro;
using UnityEngine;
using Utils;

namespace AR.World
{
    public class ARAnchorFollower : MonoBehaviour
    {
        [SerializeField] private GiftView gift;
        [SerializeField] private TMP_Text distanceText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Transform zoneTransform;

        public Vector2? WorldCoordinates = null;
        private bool _isCollectable;
        private Action _clicked;
        private Camera _camera;
        private Transform _transform;

        public void Collect()
        {
            _clicked?.Invoke();
        }

        public bool IsCollectable => _isCollectable;

        public void SetupClick(Action clicked)
        {
            if (!gift) return;
            _clicked = clicked;
        }

        public void SetZoneScale(float scale)
        {
            zoneTransform.localScale =
                new Vector3(1f, 0f, 1f) * scale + new Vector3(0f, zoneTransform.lossyScale.y, 0f);
        }

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void SetName(string name)
        {
            gameObject.name = name;
            nameText.text = name;
        }

        private void Update()
        {
            if (!distanceText) return;
            
            if (WorldCoordinates == null || !_camera)
            {
                distanceText.text = "";
                return;
            }

            Vector3 playerPosition = _camera.transform.position;

            double distance = Vector3.Distance(playerPosition, _transform.position);
            distanceText.text = distance.DistanceToHuman();

            if (!gift) return;

            gift.ShowOutline(distance <= GlobalConstants.CollectDistance || _isCollectable);
        }

        public void Interact()
        {
            if (!gift) return;

            gift.Interact();
        }

        public void SetIsArDistanceSelected(bool arCollectable)
        {
            _isCollectable = arCollectable;
        }

        private void Awake()
        {
            _camera = Camera.main;
            _transform = transform;
        }
    }
}