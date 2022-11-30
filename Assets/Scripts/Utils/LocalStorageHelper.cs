using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Utils
{
    public interface ILocalStorageHelper
    {
        void LoadSprite(string url, Action<Sprite> callback);
    }

    public class LocalStorageHelper : ILocalStorageHelper, IInitializable
    {
        private readonly Dictionary<string, Sprite> _cacheSpriteDictionary = new();
        private readonly Queue<SpriteRequest> _requestQueue = new();

        private bool _requestInProgress;

        public void LoadSprite(string url, Action<Sprite> callback)
        {
            if (string.IsNullOrEmpty(url)) callback.Invoke(null);

            _requestQueue.Enqueue(new SpriteRequest { URL = url, Callback = callback });
            CheckRequestStack();
        }

        private void CheckRequestStack()
        {
            if (_requestInProgress) return;
            if (_requestQueue.Count <= 0) return;

            SpriteRequest request = _requestQueue.Dequeue();

            if (request.URL.IsNullOrEmpty())
            {
                Debug.LogWarning("Load Sprite error: Url is empty!");
                SetImage(request, null);
                return;
            }

            _requestInProgress = true;
            string filename = Path.GetFileName(request.URL);
            string[] substr = filename.Split('?');
            filename = substr[0];

            bool inCache = _cacheSpriteDictionary.ContainsKey(filename);

            if (inCache)
            {
                SetImage(request, _cacheSpriteDictionary[filename]);
                return;
            }

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogWarning("Load Sprite error: No internet!");
                SetImage(request, null);
            }
            else
            {
                MainThreadDispatcher.StartCoroutine(LoadSpriteImage(request));
            }
        }

        private IEnumerator LoadSpriteImage(SpriteRequest request)
        {
            string filename = Path.GetFileName(request.URL);
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(request.URL);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogWarning($"Load Sprite ({request.URL}) Error: {uwr.error}");
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

                Debug.Log("Sprite Loaded: " + request.URL +
                          $"\n cached as {Application.persistentDataPath + $"/Cache/{filename}"}");

                try
                {
                    byte[] itemBgBytes = sprite.texture.EncodeToPNG();
                    File.WriteAllBytes(Application.persistentDataPath + $"/Cache/{filename}", itemBgBytes);
                }
                catch (Exception e)
                {
                    Debug.Log($"error = {e}");
                    Console.WriteLine(e);
                    throw;
                }
                
                SetImage(request, sprite);
            }
        }

        private void SetImage(SpriteRequest request, Sprite sprite)
        {
            try
            {
                request.Callback?.Invoke(sprite);
            }
            catch
            {
                Debug.LogWarning("Set sprite failed");
            }

            _requestInProgress = false;
            CheckRequestStack();
        }

        private Texture2D LoadTexture(string FilePath)
        {
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails

            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2); // Create new "empty" texture
                if (Tex2D.LoadImage(FileData)) // Load the imagedata into the texture (size is set automatically)
                    return Tex2D; // If data = readable -> return texture
            }

            return null; // Return null if load failed
        }

        /*public void CheckAllChallengesIconsDownloaded(ChallengesData data, Action onSuccess)
        {
            _compositionRoot.StartCoroutine(CheckAllIconsDownloaded(data, onSuccess));
        }*/

        /*private IEnumerator CheckAllIconsDownloaded(ChallengesData data, Action onSuccess)
        {
            while (data.challenges.Any(p => p.icon == null))
            {
                yield return new WaitForFixedUpdate();
            }
            onSuccess?.Invoke();
        }*/

        public void Initialize()
        {
            DirectoryInfo info = new(GlobalConstants.CachePath);

            FileInfo[] files = info.GetFiles();

            foreach (FileInfo file in files)
            {
                string filename = file.Name;
                
                if (!filename.EndsWith(".png")) continue;
                
                Texture2D texture = LoadTexture(GlobalConstants.CachePath + filename);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0, 0));

                if (!_cacheSpriteDictionary.ContainsKey(filename))
                {
                    _cacheSpriteDictionary.Add(filename, sprite);
                }
            }
        }
    }
}