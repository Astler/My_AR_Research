using Data;
using GameCamera;
using UnityEngine;
using Utils;
using Zenject;
using Random = UnityEngine.Random;

namespace AR.World
{
    public class CoinsController : MonoBehaviour
    {
        [SerializeField] private float spreadPower = 10f;
        [SerializeField] private CoinView coinPrefab;
     
        private readonly Pool<CoinView> _coinsPool = new();
        
        private IDataProxy _dataProxy;
        private CameraView _cameraView;

        [Inject]
        public void Construct(IDataProxy dataProxy, CameraView cameraView)
        {
            _cameraView = cameraView;
            _dataProxy = dataProxy;
        }
        
        public void SpawnCoinsAtPosition(Vector3 position)
        {
            CoinView coin = _coinsPool.BorrowItem();
            coin.SetActive(true);
            coin.SetPosition(position);

            Vector3 direction = Random.insideUnitSphere.normalized;
            Vector3 upDirection = Vector3.up;

            coin.Rigidbody.velocity = Vector3.zero;
            coin.Rigidbody.AddForce(direction * spreadPower + upDirection, ForceMode.Impulse);

            coin.LifetimeOut += OnCoinLifetimeOut;
        }

        private void OnCoinLifetimeOut(CoinView coin)
        {
            coin.LifetimeOut -= OnCoinLifetimeOut;

            coin.MoveToPosition(_cameraView.GetFacePosition(), coin =>
            {
                _dataProxy.CollectedCoin();
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