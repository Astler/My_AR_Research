using Data.Objects;
using TMPro;
using UnityEngine;
using Utils;

namespace AR.World.Collectable
{
    public class MannaBoxView : CollectableItem
    {
        [SerializeField] private TMP_Text rewardName;
        [SerializeField] private TMP_Text distanceToPlayer;

        private BeamData _beamData;

        public void SetBeamData(BeamData data)
        {
            rewardName.text = data.Name;
            _beamData = data;
        }

        public BeamData GetBeamData() => _beamData;

        public void SetBoxName(string dataName)
        {
            gameObject.name = "";
        }

        protected override void OnUpdate()
        {
            Vector3 playerPosition = Camera.transform.position;
            double distance = Vector3.Distance(playerPosition, Transform.position) / 1000;
            distanceToPlayer.text = distance.DistanceToHuman();

            Vector3 position = Transform.position;

            Debug.DrawRay(new Vector3(position.x, 0f, position.z), Vector3.down, Color.red);
        }
    }
}