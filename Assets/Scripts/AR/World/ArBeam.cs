using Data.Objects;
using UnityEngine;
using Utils;

namespace AR.World
{
    public class ArBeam: ARAnchorFollower
    {
        private BeamData _beamData;

        public void SetBeamData(BeamData data) => _beamData = data;
        
        public BeamData GetBeamData() => _beamData;
        
        public bool CanBeCollected(Vector2 playerPosition)
        {
            double distance = CoordinatesUtils.Distance(playerPosition.x,
                playerPosition.y,
                WorldCoordinates.Value.x,
                WorldCoordinates.Value.y);
            
            return distance * 1000 < GlobalConstants.CollectDistance || IsCollectable;
        }
    }
}