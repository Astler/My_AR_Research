using System;
using UnityEngine;
#if UNITY_IOS
#endif

namespace Utils
{
#if UNITY_IOS
    using Device = UnityEngine.iOS.Device;
#endif
    public static class DeviceIDGetter
    {
        public static void GetID(Action<string> callback)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //TODO IMPORT IOS STUFF
                // AppTrackingTransparency.OnAuthorizationRequestDone += AppTransparencyRequestDone;
                // AppTrackingTransparency.RequestTrackingAuthorization();
            }
            else
            {
                callback?.Invoke(SystemInfo.deviceUniqueIdentifier + (Application.isEditor ? "_editor" : ""));
            }

//             void AppTransparencyRequestDone(AppTrackingTransparency.AuthorizationStatus authorizationStatus)
//             {
// #if UNITY_IOS
//                 AppTrackingTransparency.OnAuthorizationRequestDone -= AppTransparencyRequestDone;
//                 if (Device.advertisingIdentifier == "00000000-0000-0000-0000-000000000000")
//                 {
//                     if (!PlayerPrefs.HasKey("LocalDeviceID"))
//                     {
//                         string device = "local_id_";
//                         for (int i = 0; i < 20; i++)
//                         {
//                             device += UnityEngine.Random.Range(0, 10);
//                         }
//
//                         PlayerPrefs.SetString("LocalDeviceID", device);
//                     }
//
//                     callback?.Invoke(PlayerPrefs.GetString("LocalDeviceID"));
//                 }
//                 else
//                 {
//                     callback?.Invoke(Device.advertisingIdentifier);
//                 }
// #endif
        }
    }
}