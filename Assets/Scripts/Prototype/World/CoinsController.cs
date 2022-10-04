using Prototype.Utils;
using Prototype.World.Coins;
using UnityEngine;

namespace Prototype.World
{
    public class CoinsController: MonoBehaviour
    {
        [SerializeField] private float spreadPower = 10f;
        [SerializeField] private float spreadRadius = 5f;
        [SerializeField] private CoinView coinPrefab;
        [SerializeField] private ProjectContext context;
        
        private readonly Pool<CoinView> _coinsPool = new();

        public void SpawnCoinsAtPosition(Vector3 position)
        {
            CoinView coin = _coinsPool.BorrowItem();
            coin.SetActive(true);
            coin.SetPosition(position);
            coin.Rigidbody.AddExplosionForce(spreadPower, position, spreadRadius);
            
            coin.LifetimeOut += OnCoinLifetimeOut;
        }

        private void OnCoinLifetimeOut(CoinView coin)
        {
            coin.LifetimeOut -= OnCoinLifetimeOut;

            coin.MoveToPosition(context.GetPlayerFacePosition(), coin =>
            {
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