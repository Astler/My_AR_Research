using System.Collections.Generic;
using System.Linq;
using Data.Objects;
using UnityEngine;

namespace Assets
{
    [CreateAssetMenu(fileName = "AssetsSO", menuName = "SO/Assets SO", order = 1)]
    public class AssetsScriptableObject : ScriptableObject
    {
        [SerializeField] private PortalZoneModel[] portalZones;
        [SerializeField] private GiftModel[] giftModels;

        public IEnumerable<PortalZoneModel> GetZonesList() => portalZones.Where(it => it.isActive);
        public IEnumerable<GiftModel> GetGiftsModels() => giftModels;
    }
}