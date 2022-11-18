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
        private bool _arCollectable;

        public void SetupClick(Action clicked)
        {
            if (!gift) return;

            Vector2 playerPosition = Application.isEditor
                ? GlobalConstants.MockPosition
                : new Vector2(Input.location.lastData.latitude,
                    Input.location.lastData.longitude);

            gift.Interacted += _ =>
            {
                double distance = CoordinatesUtils.Distance(playerPosition.x,
                    playerPosition.y,
                    WorldCoordinates.Value.x,
                    WorldCoordinates.Value.y);

                Debug.Log($"gift Hit {distance}");

                if (distance * 1000 > GlobalConstants.CollectDistance && !_arCollectable) return;

                Debug.Log($"collect!");
                clicked?.Invoke();
            };
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
            if (WorldCoordinates == null) return;

            Vector2 playerPosition = Application.isEditor
                ? GlobalConstants.MockPosition
                : new Vector2(Input.location.lastData.latitude,
                    Input.location.lastData.longitude);

            double worldDistance = CoordinatesUtils.Distance(playerPosition.x,
                playerPosition.y,
                WorldCoordinates.Value.x,
                WorldCoordinates.Value.y);
            
            distanceText.text = worldDistance.DistanceToHuman();

            if (!gift) return;

            gift.ShowOutline(worldDistance * 1000 <= GlobalConstants.CollectDistance || _arCollectable);
        }

        public void Interact()
        {
            if (!gift) return;

            gift.Interact();
        }

        public void SetIsArDistanceSelected(bool arCollectable)
        {
            _arCollectable = arCollectable;
        }
    }
}