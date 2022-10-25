using System;
using Mapbox.Utils;
using Prototype.Data;
using UnityEngine;

namespace Prototype.Assets
{
    [Serializable]
    public class PortalZoneModel
    {
        public string name = "Zone";
        public double latitude;
        public double longitude;
        public float radius = 20f;
        public bool isActive = true;

        public Vector2 GetPosition() => new((float)latitude, (float)longitude);
        public Vector2d GetPosition2d() => new((float)latitude, (float)longitude);

        public PortalViewInfo ToViewInfo() => new PortalViewInfo
        {
            Name = name,
            Coordinates = GetPosition()
        };
    }
}