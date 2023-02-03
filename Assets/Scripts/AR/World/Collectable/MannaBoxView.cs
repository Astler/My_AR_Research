using TMPro;
using UnityEngine;
using Utils;

namespace AR.World.Collectable
{
    public class MannaBoxView : CollectableItem
    {
        [SerializeField] private TMP_Text distanceToPlayer;

        private int _id;
        public int DropId => _id;

        public void SetBeamData(int id)
        {
            _id = id;
        }

        protected override void OnUpdate()
        {
            Vector3 position = Transform.position;
            
            Vector3 playerPosition = Camera.transform.position;
            double distance = Vector3.Distance(playerPosition, position) / 1000;
            distanceToPlayer.text = distance.DistanceToHuman();
            
            Debug.DrawRay(new Vector3(position.x, 0f, position.z), Vector3.down, Color.red);
        }
    }
}