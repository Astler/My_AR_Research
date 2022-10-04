using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Prototype.World.Coins
{
    public class CoinView : MonoBehaviour
    {
        private Transform _transform;
        private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody ? _rigidbody : GetComponentInChildren<Rigidbody>();

        public event Action<CoinView> LifetimeOut;

        public void SetPosition(Vector3 position) => _transform.position = position;

        private void Awake()
        {
            _transform = transform;
        }

        private void OnEnable()
        {
            Rigidbody.isKinematic = false;
            StartCoroutine(LifetimeTimer());
        }

        private IEnumerator LifetimeTimer()
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));
            LifetimeOut?.Invoke(this);
        }

        public void SetActive(bool active) => gameObject.SetActive(active);

        public void MoveToPosition(Vector3 targetPosition, Action<CoinView> callback)
        {
            Rigidbody.isKinematic = true;
            StartCoroutine(MoveTo(targetPosition, callback));
        }

        private IEnumerator MoveTo(Vector3 targetPosition, Action<CoinView> callback)
        {
            while (Vector3.Distance(_transform.position, targetPosition) > 0.1f)
            {
                _transform.position = Vector3.MoveTowards(_transform.position, targetPosition, 5f * Time.deltaTime);
                yield return null;
            }

            callback?.Invoke(this);
        }
    }
}