using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Prototype.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prototype.World
{
    public class GiftsController : MonoBehaviour
    {
        [SerializeField] private int maxGifts = 40;
        [SerializeField] private ProjectContext context;

        private readonly List<GiftView> _spawnedGifts = new();

        public void SpawnGifts(int totalGiftsInPortal, Vector3 portalPosition, Action onComplete)
        {
            StartCoroutine(SpawnPortalGifts(totalGiftsInPortal, portalPosition, onComplete));
        }

        private IEnumerator SpawnPortalGifts(int totalGiftsInPortal, Vector3 portalPosition, Action onComplete)
        {
            Vector3 cameraForward = context.GetCameraForwardDirection();
            for (int i = 0; i < totalGiftsInPortal; i++)
            {
                SpawnNewGift(portalPosition, Random.rotation)
                    .AddForce(new Vector3(cameraForward.x * Random.Range(-3, 4), Random.Range(0, 2), 2));
                yield return new WaitForSeconds(0.2f);
            }

            onComplete?.Invoke();
        }

        public GiftView SpawnNewGift(Vector3 position, Quaternion rotation)
        {
            if (_spawnedGifts.Count >= maxGifts)
            {
                Destroy(_spawnedGifts[0].gameObject);
                _spawnedGifts.RemoveAt(0);
            }

            GiftView giftView = Instantiate(context.GetAssets().giftModels.GetRandomElement().giftViewPrefab, position,
                rotation);

            giftView.Interacted += OnGiftInteracted;
            
            _spawnedGifts.Add(giftView);

            return giftView;
        }

        private void OnGiftInteracted(GiftView giftView)
        {
            CoinsController coinsController = context.GetCoinsController();

            for (int i = 0; i < Random.Range(20, 50); i++)
            {
                coinsController.SpawnCoinsAtPosition(giftView.GetPosition());
            }
        }

        public void DeleteAllGifts()
        {
            foreach (GiftView spawnedGift in _spawnedGifts)
            {
                Destroy(spawnedGift.gameObject);
            }

            _spawnedGifts.Clear();
        }

        public void Collect(GiftView giftView)
        {
            giftView.Interact();

            Destroy(giftView.gameObject);
            
            _spawnedGifts.Remove(giftView);
            Debug.Log("Collected gift");
        }
    }
}