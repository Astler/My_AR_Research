using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Data;
using Plugins.Honeti.I18N.Scripts;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Utils;

namespace Core
{
    public class ApiInterface : IApiInterface
    {
        private const int DefaultRequestTimeoutInSeconds = 15;
        private readonly List<Request> _requestsList = new();

        public static string GetMainEndpointUri()
        {
            return GlobalConstants.EnvironmentType switch
            {
                EnvironmentType.localhost => "http://localhost:3000",
                EnvironmentType.dev => "https://manna-drops.themindstudios.com",
                EnvironmentType.staging => "https://manna-drops.themindstudios.com",
                EnvironmentType.prod => "https://manna-drops.themindstudios.com",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static string GetEndpointUri(string suffix)
        {
            return GetMainEndpointUri() + $"/api/v{GlobalConstants.ApiVersion}/{suffix}";
        }

        #region request queue

        private void AddRequestToList(Request request, bool asFirst = false)
        {
            if (_requestsList.Find(r => r.Key == request.Key) != null)
            {
                Debug.LogWarning("Request (" + request.Key + ") is already in list");
                return;
            }

            if (asFirst)
            {
                _requestsList.Insert(0, request);
            }
            else
            {
                _requestsList.Add(request);
            }

            if (_requestsList.Count == 1)
            {
                TryToSendNextRequest();
            }
        }

        public void TryToSendNextRequest()
        {
            if (_requestsList.Count == 0) return;

            _requestsList[0]?.Action?.Invoke();
        }

        private void DeleteExecutedRequestAndSendNext()
        {
            if (_requestsList.Count == 0) return;

            _requestsList.RemoveAt(0);
            TryToSendNextRequest();
        }

        private void ClearRequestsList()
        {
            _requestsList.Clear();
        }

        #endregion // request queue

        private bool CheckResultForInternetConnection(UnityWebRequest results)
        {
            if (results.result != UnityWebRequest.Result.ConnectionError) return false;
            //TODO: Showing offline popup
            return true;
        }

        private void WebRequestHandler<T>(UnityWebRequest results, string uri, Action<T> onSuccess,
            Action<ResponseStatus> onFailure)
        {
            try
            {
                Debug.Log("Response: " + results.downloadHandler.text);
            }
            catch
            {
                Debug.LogWarning("Response downloadHandler.text empty!");
            }

            Response response;
            T res = default;
            try
            {
                res = JsonUtility.FromJson<T>(results.downloadHandler.text);
                response = res as Response;
            }
            catch
            {
                response = new Response
                {
                    responseStatus = new ResponseStatus
                    {
                        errorMessage = "Json parse error!"
                    }
                };
            }

            if (!string.IsNullOrEmpty(results.error))
            {
                response ??= new Response
                {
                    responseStatus = new ResponseStatus
                    {
                        errorMessage = results.error
                    }
                };

                onFailure?.Invoke(response.responseStatus);

                if (CheckResultForInternetConnection(results))
                {
                    return;
                }

                Debug.LogError("ResponseCode: " + results.responseCode + ". " + results.error + "/" +
                               response.responseStatus?.errorMessage);
            }
            else
            {
                onSuccess?.Invoke(res);
            }

            DeleteExecutedRequestAndSendNext();
        }

        #region base post

        private void Post<T>(string uri, WWWForm form, Action<T> onSuccess, Action<ResponseStatus> onFailure,
            bool isAuthorized = false)
        {
            void WebRequestReturnAction(UnityWebRequest results)
            {
                WebRequestHandler(results, uri, onSuccess, onFailure);
            }

            MainThreadDispatcher.StartCoroutine(PostEntity(uri, form, WebRequestReturnAction, isAuthorized));
        }

        private IEnumerator PostEntity(string uri, WWWForm form, Action<UnityWebRequest> action, bool isAuthorized)
        {
            Debug.Log("UnityWebRequest: " + uri);

            UploadHandlerRaw uploadHandlerRaw = new(form?.data);
            uploadHandlerRaw.contentType = "application/x-www-form-urlencoded";
            DownloadHandler downloadHandler = new DownloadHandlerBuffer();
            UnityWebRequest webRequest = new(uri, "POST", downloadHandler, uploadHandlerRaw);
            webRequest.timeout = DefaultRequestTimeoutInSeconds;
            if (isAuthorized)
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefsHelper.AccessToken);
            }

            yield return webRequest.SendWebRequest();

            action(webRequest);
        }

        private void PostFile<T>(string uri, List<IMultipartFormSection> requestData, Action<T> onSuccess,
            Action<ResponseStatus> onFailure, bool isAuthorized = false)
        {
            void WebRequestReturnAction(UnityWebRequest results)
            {
                WebRequestHandler(results, uri, onSuccess, onFailure);
            }

            MainThreadDispatcher.StartCoroutine(PostData(uri, requestData, WebRequestReturnAction, isAuthorized));
        }

        private IEnumerator PostData(string uri, List<IMultipartFormSection> requestData,
            Action<UnityWebRequest> action, bool isAuthorized)
        {
            Debug.Log("UnityWebRequest: " + uri);
            UnityWebRequest webRequest = UnityWebRequest.Post(uri, requestData);
            if (isAuthorized)
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefsHelper.AccessToken);
            }

            webRequest.timeout = DefaultRequestTimeoutInSeconds;

            yield return webRequest.SendWebRequest();

            action(webRequest);
        }

        private void PostJson<T>(string uri, string jsonString, Action<T> onSuccess, Action<ResponseStatus> onFailure,
            bool isAuthorized = false)
        {
            void WebRequestReturnAction(UnityWebRequest results)
            {
                WebRequestHandler(results, uri, onSuccess, onFailure);
            }

            MainThreadDispatcher.StartCoroutine(PostJsonEntity(uri, jsonString, WebRequestReturnAction, isAuthorized));
        }

        private IEnumerator PostJsonEntity(string uri, string jsonString, Action<UnityWebRequest> action,
            bool isAuthorized)
        {
            Debug.Log("UnityWebRequest: " + uri);

            UnityWebRequest webRequest = new(uri, "POST") { timeout = DefaultRequestTimeoutInSeconds };
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            if (isAuthorized)
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefsHelper.AccessToken);
            }

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            action(webRequest);
        }

        #endregion // base post

        #region base get

        private void Get<T>(string uri, Action<T> onSuccess, Action<ResponseStatus> onFailure,
            bool isAuthorized = false)
        {
            void WebRequestReturnAction(UnityWebRequest results)
            {
                WebRequestHandler(results, uri, onSuccess, onFailure);
            }

            MainThreadDispatcher.StartCoroutine(GetEntity(uri, WebRequestReturnAction, isAuthorized));
        }

        private IEnumerator GetEntity(string uri, Action<UnityWebRequest> action, bool isAuthorized)
        {
            Debug.Log("UnityWebRequest: " + uri);
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            if (isAuthorized)
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefsHelper.AccessToken);
            }

            webRequest.timeout = DefaultRequestTimeoutInSeconds;

            yield return webRequest.SendWebRequest();

            action(webRequest);
        }

        #endregion // base get

        #region base put

        private void Put<T>(string uri, WWWForm form, Action<T> onSuccess, Action<ResponseStatus> onFailure,
            bool isAuthorized = false)
        {
            void WebRequestReturnAction(UnityWebRequest results)
            {
                WebRequestHandler(results, uri, onSuccess, onFailure);
            }

            MainThreadDispatcher.StartCoroutine(PutEntity(uri, form, WebRequestReturnAction, isAuthorized));
        }

        private IEnumerator PutEntity(string uri, WWWForm form, Action<UnityWebRequest> action, bool isAuthorized)
        {
            Debug.Log("UnityWebRequest: " + uri);

            UploadHandlerRaw uploadHandlerRaw = new(form?.data);
            uploadHandlerRaw.contentType = "application/x-www-form-urlencoded";
            UnityWebRequest webRequest = new(uri, "PUT", new DownloadHandlerBuffer(), uploadHandlerRaw);
            webRequest.timeout = DefaultRequestTimeoutInSeconds;
            if (isAuthorized)
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefsHelper.AccessToken);
            }

            yield return webRequest.SendWebRequest();

            action(webRequest);
        }

        #endregion // base put

        public void SignIn(Action<SignInResponse> onSuccess, Action<ResponseStatus> onFailure)
        {
            DeviceIDGetter.GetID(delegate(string deviceId)
            {
                WWWForm form = new();
                form.AddField("device_uid", deviceId);
                form.AddField("app_version", GlobalConstants.ApiVersion);
                form.AddField("platform", StringConstants.GetPlatformName(Application.platform));
                form.AddField("locale", I18N.instance.gameLang.ToString());
                form.AddField("client_id", GlobalConstants.GetClientId());
                form.AddField("client_secret", GlobalConstants.GetClientSecret());

                SendRequest();
/*#if UNITY_IOS
                GameCenterController.AuthenticateUser(centerId =>
                {
                    form.AddField("game_center_id", centerId);
                    SendRequest();
                });
#elif UNITY_ANDROID
                PlayCenterController.AuthenticateUser(delegate(string centerId)
                {
                    form.AddField("play_center_id", centerId);
                    SendRequest();
                });
#else
                SendRequest();
#endif*/

                void SendRequest()
                {
                    void NewRequest() => Post(GetEndpointUri("users/sign_in"), form, onSuccess, onFailure);
                    AddRequestToList(new Request("users/sign_in", NewRequest));
                }
            });
        }

        public void GetEventsList(Action<EventsData> onSuccess, Action<ResponseStatus> onFailure)
        {
            void NewRequest() => Get(GetEndpointUri("events"), onSuccess, onFailure, true);
            AddRequestToList(new Request("events", NewRequest));
        }

        private class Request
        {
            public readonly string Key;
            public readonly Action Action;

            public Request(string key, Action action)
            {
                Key = key;
                Action = action;
            }
        }
    }
}