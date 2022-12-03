using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Utils;
using Zenject;

namespace ExternalTools.ImagesLoader
{
    public class WebImagesLoader : IWebImagesLoader, IInitializable
    {
        private const bool InDebug = true;

        private readonly Dictionary<string, Sprite> _cacheSpriteDictionary = new();
        private readonly Queue<SpriteRequest> _requestQueue = new();
        private bool _requestInProgress;

        public void Initialize()
        {
            DirectoryInfo info = new(GlobalConstants.CachePath);

            if (!info.Exists)
            {
                Directory.CreateDirectory(GlobalConstants.CachePath);
            }

            FileInfo[] files = info.GetFiles();

            foreach (FileInfo file in files)
            {
                string filename = file.Name;
                Texture2D texture = LoadTexture(GlobalConstants.CachePath + filename);

                if (!texture) continue;

                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0, 0));

                if (!_cacheSpriteDictionary.ContainsKey(filename))
                {
                    _cacheSpriteDictionary.Add(filename, sprite);
                }
            }
        }

        public void TryToLoadSprite(string url, Action<Sprite> callback)
        {
            if (string.IsNullOrEmpty(url))
            {
                if (InDebug)
                {
                    Debug.LogWarning("Requested empty url");
                }

                callback.Invoke(null);
            }

            _requestQueue.Enqueue(new SpriteRequest { URL = url, Callback = callback });
            CheckRequestStack();
        }

        private void CheckRequestStack()
        {
            if (_requestInProgress) return;
            if (_requestQueue.Count <= 0) return;

            SpriteRequest request = _requestQueue.Dequeue();

            _requestInProgress = true;
            string filename = Path.GetFileName(request.URL);
            string[] substr = filename.Split('?');
            filename = substr[0];

            if (_cacheSpriteDictionary.ContainsKey(filename))
            {
                SetImage(request, _cacheSpriteDictionary[filename]);

                if (InDebug)
                {
                    Debug.Log($"Sprite Loaded From Cache: {GlobalConstants.CachePath + filename}");
                }

                return;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                if (InDebug)
                {
                    Debug.LogWarning("Load Sprite error: No internet!");
                }

                SetImage(request, null);
            }
            else
            {
                try
                {
                    MainThreadDispatcher.StartCoroutine(LoadSpriteImage(request));
                }
                catch (Exception exception)
                {
                    if (InDebug)
                    {
                        Debug.LogError("Error raised while loading image from url: " + request.URL + " " + exception);
                    }

                    SetImage(request, null);
                }
            }
        }

        private IEnumerator LoadSpriteImage(SpriteRequest request)
        {
            string filename = Path.GetFileName(request.URL);
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(request.URL);
            yield return uwr.SendWebRequest();

            while (!uwr.isDone)
            {
                if (InDebug)
                {
                    Debug.Log(uwr.downloadProgress);
                }

                yield return null;
            }

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                if (InDebug)
                {
                    Debug.LogWarning($"Load Sprite ({request.URL}) Error: {uwr.error}");
                }

                SetImage(request, null);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                Sprite sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);

                if (!_cacheSpriteDictionary.ContainsKey(filename))
                {
                    _cacheSpriteDictionary.Add(filename, sprite);
                }

                if (InDebug)
                {
                    Debug.Log("Sprite Loaded From Web: " + request.URL +
                              $"\n cached as {GlobalConstants.CachePath + filename}");
                }

                try
                {
                    byte[] itemBgBytes = sprite.texture.EncodeToPNG();
                    File.WriteAllBytes(GlobalConstants.CachePath + filename, itemBgBytes);
                }
                catch (Exception exception)
                {
                    if (InDebug)
                    {
                        Debug.Log($"Image save process finished with error: = {exception}");
                    }
                }

                SetImage(request, sprite);
            }
        }

        private void SetImage(SpriteRequest request, Sprite sprite)
        {
            request.Callback?.Invoke(sprite);
            _requestInProgress = false;
            CheckRequestStack();
        }

        private static Texture2D LoadTexture(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            Texture2D texture2D = new(2, 2);

            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                texture2D.LoadImage(fileData);
            }
            catch (Exception exception)
            {
                if (InDebug)
                {
                    Debug.LogWarning("Error loading sprite from cache: " + filePath + " " + exception);
                }

                texture2D = null;
            }

            return texture2D;
        }
    }
}