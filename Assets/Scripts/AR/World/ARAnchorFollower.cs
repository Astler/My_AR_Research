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

        public void SetupClick(Action clicked)
        {
            if (!gift) return;

            gift.Interacted += _ =>
            {
                double distance = CoordinatesUtils.Distance(Input.location.lastData.latitude,
                    Input.location.lastData.longitude,
                    WorldCoordinates.Value.x,
                    WorldCoordinates.Value.y);
                            
                Debug.Log($"gift Hit {distance}");
                        
                if (distance * 1000 > GlobalConstants.CollectDistance) return;
                
                Debug.Log($"collect!");
                clicked?.Invoke();
            };
        }

        public void SetZoneScale(float scale)
        {
            zoneTransform.localScale = new Vector3(1f, 5f, 1f) * (scale / 10);
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

            double distance = CoordinatesUtils.Distance(Input.location.lastData.latitude,
                Input.location.lastData.longitude,
                WorldCoordinates.Value.x,
                WorldCoordinates.Value.y);

            distanceText.text = distance.DistanceToHuman();

            if (!gift) return;

            gift.ShowOutline(distance * 1000 <= GlobalConstants.CollectDistance);
        }

        public void Interact()
        {
            if (!gift) return;
            
            gift.Interact();
        }
    }
}