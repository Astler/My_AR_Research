using System;
using UnityEngine;

namespace Utils
{
    internal struct SpriteRequest
    {
        public string URL;
        public Action<Sprite> Callback;
    }
}