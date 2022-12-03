using System;
using UnityEngine;

namespace ExternalTools.ImagesLoader
{
    public interface IWebImagesLoader
    {
        void TryToLoadSprite(string url, Action<Sprite> callback);
    }
}