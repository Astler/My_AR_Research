using System;
using Mapbox.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class CoordinatesUtils
    {
        public static Vector2d ToVector2d(this Vector2 vector2) => new(vector2.x, vector2.y);

        public static Vector2 ToUnityVector(this Vector2d vector2d) => new((float)vector2d.x, (float)vector2d.y);

        public static string ToHumanReadableDistanceFromPlayer(this Vector2 targetPosition, Vector2 playerPosition)
        {
            double distance = Distance(playerPosition.x,
                playerPosition.y,
                targetPosition.x,
                targetPosition.y);

            return distance.DistanceToHuman();
        }

        public static string DistanceToHuman(this double distance)
        {
            string result;

            if (distance < 1f)
            {
                distance *= 1000;

                if (distance < 1f)
                {
                    distance *= 1000;
                    result = distance.ToString("F") + " cm";
                }
                else
                {
                    result = distance.ToString("F") + " m";
                }
            }
            else
            {
                result = distance.ToString("F") + " km";
            }

            return result;
        }

        private static double ToRadians(double angleIn10thofaDegree) => angleIn10thofaDegree * Math.PI / 180;

        public static double Distance(double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            lon1 = ToRadians(lon1);
            lon2 = ToRadians(lon2);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double dlon = lon2 - lon1;
            double dlat = lat2 - lat1;
            double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dlon / 2), 2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            // Radius of earth in
            // kilometers. Use 3956
            // for miles
            double r = 6371;

            // calculate the result
            return (c * r);
        }

        public static Vector2 GetRandomWorldPositionInRadius(Vector2 worldCenter, float radiusInMeters)
        {
            Vector2 randomPosition = Random.insideUnitCircle * radiusInMeters;
            
            double earth = 6378.137;
            double meter = (1 / ((2 *  Math.PI / 360) * earth)) / 1000;  //1 meter in degree

            double newLatitude = worldCenter.x + randomPosition.x * meter;
            double newLongitude = worldCenter.y + randomPosition.y * meter;

            return new Vector2d(newLatitude, newLongitude).ToUnityVector();
        }
    }
}