using System;

namespace Prototype.Assets
{
    [Serializable]
    public class PortalZoneModel
    {
        public string name = "Zone";
        public double longitude;
        public double latitude;
        public float radius = 20f;
        public bool isActive = true;
    }
}