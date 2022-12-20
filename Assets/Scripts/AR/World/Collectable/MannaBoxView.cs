using System;
using Data.Objects;
using UnityEngine;

namespace AR.World.Collectable
{
    public class MannaBoxView: CollectableItem
    {
        private BeamData _beamData;

        public void SetBeamData(BeamData data) => _beamData = data;
        
        public BeamData GetBeamData() => _beamData;

        public void SetBoxName(string dataName)
        {
            gameObject.name = dataName;
        }

        private void Update()
        {
            Debug.DrawRay(new Vector3(transform.position.x, 0f, transform.position.z), Vector3.down, Color.red);
        }
    }
}