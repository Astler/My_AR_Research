using System;
using UnityEngine;

namespace Prototype.Location
{
    public static class LocationUtils
    {
        public static float CalculateDistance(float lat1, float long1, float lat2, float long2)
        {
            int R = 6371;
            float latRad1 = Mathf.Deg2Rad * lat1;
            float latRad2 = Mathf.Deg2Rad * lat2;
            float deltaLatRad = Mathf.Deg2Rad * (lat2 - lat1);
            float deltaLongRad = Mathf.Deg2Rad * (long2 - long1);
            float a = Mathf.Pow(Mathf.Sin(deltaLatRad / 2), 2) + (Mathf.Pow(Mathf.Sin(deltaLongRad / 2), 2)
                                                                  * Mathf.Cos(latRad1) * Mathf.Cos(latRad2));
            float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
            float totalDist = R * c * 1000;
            return totalDist;
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
                       Math.Pow(Math.Sin(dlon / 2),2);
             
            double c = 2 * Math.Asin(Math.Sqrt(a));
 
            // Radius of earth in
            // kilometers. Use 3956
            // for miles
            double r = 6371;
 
            // calculate the result
            return (c * r);
        }
    }
}