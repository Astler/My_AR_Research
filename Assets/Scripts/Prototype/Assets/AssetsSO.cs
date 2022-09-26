using UnityEngine;

namespace Prototype.Assets
{
    [CreateAssetMenu(fileName = "AssetsSO", menuName = "SO/Assets SO", order = 1)]
    public class AssetsSO : ScriptableObject
    {
        public GiftModel[] giftModels;
    }
}