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

        private readonly List<Gift> _spawnedGifts = new();

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

        public Gift SpawnNewGift(Vector3 position, Quaternion rotation)
        {
            if (_spawnedGifts.Count >= maxGifts)
            {
                Destroy(_spawnedGifts[0].gameObject);
                _spawnedGifts.RemoveAt(0);
            }

            Gift gift = Instantiate(context.GetAssets().giftModels.GetRandomElement().giftPrefab, position,
                rotation);

            _spawnedGifts.Add(gift);

            return gift;
        }

        public void DeleteAllGifts()
        {
            foreach (Gift spawnedGift in _spawnedGifts)
            {
                Destroy(spawnedGift.gameObject);
            }

            _spawnedGifts.Clear();
        }

        public void Collect(Gift gift)
        {
            gift.Interact();
            Destroy(gift.gameObject);
            _spawnedGifts.Remove(gift);
            Debug.Log("Collected gift");
        }
    }
}