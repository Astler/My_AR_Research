using System.Collections.Generic;
using Data.Objects;
using UnityEngine;

namespace Assets
{
    [CreateAssetMenu(fileName = "AssetsSO", menuName = "SO/Assets SO", order = 1)]
    public class AssetsScriptableObject : ScriptableObject
    {
        [SerializeField] private GiftModel[] giftModels;

        public IEnumerable<GiftModel> GetGiftsModels() => giftModels;
    }
}