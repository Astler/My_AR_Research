using System;
using Data;
using UnityEngine;

namespace Utils
{
    public class GlobalConstants
    {
        public const int ApiVersion = 1;
        public const float GeoAccuracy = 0.01f;
        public static EnvironmentType EnvironmentType => EnvironmentType.dev;
        
        public const float CollectDistance = 5.0f;
        public static readonly Vector2 MockPosition = new(48.5089416503906f, 35.077808380127f);
        
        public static string GetClientId()
        {
            return EnvironmentType switch
            {
                EnvironmentType.localhost => "",
                EnvironmentType.dev => "ZCuG5Lej5y5APJWxfxk0LDiA9m2sLFZCuM7EMRejKeU",
                EnvironmentType.staging => "",
                EnvironmentType.prod => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static string GetClientSecret()
        {
            return EnvironmentType switch
            {
                EnvironmentType.localhost => "",
                EnvironmentType.dev => "eMf7FvYhFZuxWDJiN-xlQO4pGNMcoarQoFJrvJa4m14",
                EnvironmentType.staging => "",
                EnvironmentType.prod => "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}