using System;
using Prototype.Utils;
using Prototype.World.Coins;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prototype.World
{
    public class CoinsController : MonoBehaviour
    {
        [SerializeField] private float spreadPower = 10f;
        [SerializeField] private float spreadRadius = 5f;
        [SerializeField] private CoinView coinPrefab;
        [SerializeField] private ProjectContext context;

        private readonly Pool<CoinView> _coinsPool = new();

        public Action CollectedCoin;

        public static Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));

        public void SpawnCoinsAtPosition(Vector3 position)
        {
            CoinView coin = _coinsPool.BorrowItem();
            coin.SetActive(true);
            // coin.Transform.localScale = Vector3.one;
            coin.SetPosition(position);

            Vector3 direction = Random.insideUnitSphere.normalized;
            Vector3 upDirection = Vector3.up;
            //(context.GetCameraForwardDirection() - coin.Transform.position).normalized;

            coin.Rigidbody.velocity = Vector3.zero;
            coin.Rigidbody.AddForce(direction * spreadPower + upDirection, ForceMode.Impulse);

            coin.LifetimeOut += OnCoinLifetimeOut;
        }

        private void OnCoinLifetimeOut(CoinView coin)
        {
            coin.LifetimeOut -= OnCoinLifetimeOut;

            // coin.Transform.DOScale(0, 1f);

            coin.MoveToPosition(context.GetPlayerFacePosition(), coin =>
            {
                CollectedCoin?.Invoke();
                coin.SetActive(false);
                _coinsPool.ReturnItem(coin);
            });
        }

        private void Awake()
        {
            _coinsPool.Initialize(() =>
            {
                CoinView coin = Instantiate(coinPrefab);
                coin.SetActive(false);
                return coin;
            }, 50);
        }
    }
}