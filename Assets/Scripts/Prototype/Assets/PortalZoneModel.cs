using System;
using Prototype.Data;
using UnityEngine;

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

        public Vector2 GetPosition() => new((float)longitude, (float)latitude);

        public PortalViewInfo ToViewInfo() => new PortalViewInfo
        {
            Name = name,
            Coordinates = GetPosition()
        };
    }
}