using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using GameCamera;
using UnityEngine;
using Utils;
using Zenject;
using Random = UnityEngine.Random;

namespace AR.World
{
    public class GiftsController : MonoBehaviour
    {
        [SerializeField] private int maxGifts = 40;

        private readonly List<GiftView> _spawnedGifts = new();

        private CameraView _cameraView;
        private AssetsScriptableObject _assetsScriptableObject;
        private CoinsController _coinsController;

        [Inject]
        public void Construct(CameraView cameraView, AssetsScriptableObject assetsScriptableObject,
            CoinsController coinsController)
        {
            _coinsController = coinsController;
            _assetsScriptableObject = assetsScriptableObject;
            _cameraView = cameraView;
        }

        public void SpawnGifts(int totalGiftsInPortal, Vector3 portalPosition, Action onComplete)
        {
            StartCoroutine(SpawnPortalGifts(totalGiftsInPortal, portalPosition, onComplete));
        }

        private IEnumerator SpawnPortalGifts(int totalGiftsInPortal, Vector3 portalPosition, Action onComplete)
        {
            Vector3 cameraForward = _cameraView.CameraForwardVector;

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

            GiftView giftView = Instantiate(_assetsScriptableObject.GetGiftsModels().GetRandomElement().giftViewPrefab,
                position,
                rotation);

            giftView.Interacted += OnGiftInteracted;

            _spawnedGifts.Add(giftView);

            return giftView;
        }

        private void OnGiftInteracted(GiftView giftView)
        {
            for (int i = 0; i < Random.Range(20, 50); i++)
            {
                _coinsController.SpawnCoinsAtPosition(giftView.GetPosition());
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