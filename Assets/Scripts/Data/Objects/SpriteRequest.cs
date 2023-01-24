using System;
using UnityEngine;

namespace Data.Objects
{
    internal struct SpriteRequest
    {
        public string URL;
        public Action<Sprite> Callback;
    }
}