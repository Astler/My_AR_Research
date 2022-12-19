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
    }
}