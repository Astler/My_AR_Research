using System;
using System.Collections;
using UnityEngine;

namespace Items
{
    public class GiftsPortalView : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private ParticleSystem openParticle;
        [SerializeField] private ParticleSystem idleParticle;
        [SerializeField] private ParticleSystem closeParticle;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
            openParticle.gameObject.SetActive(false);
            idleParticle.gameObject.SetActive(false);
            closeParticle.gameObject.SetActive(false);
        }

        public void OpenPortal(Action onOpen)
        {
            StartCoroutine(OpenPortalCoroutine(onOpen));
        }

        private IEnumerator OpenPortalCoroutine(Action onOpen)
        {
            idleParticle.gameObject.SetActive(false);
            closeParticle.gameObject.SetActive(false);
            openParticle.gameObject.SetActive(true);
            openParticle.Play();
            yield return new WaitForSeconds(openParticle.main.duration);
            openParticle.gameObject.SetActive(false);
            idleParticle.gameObject.SetActive(true);
            idleParticle.Play();
            onOpen?.Invoke();
        }

        public void ClosePortal()
        {
            StartCoroutine(ClosePortalCoroutine());
        }

        private IEnumerator ClosePortalCoroutine()
        {
            openParticle.gameObject.SetActive(false);
            idleParticle.gameObject.SetActive(false);
            closeParticle.gameObject.SetActive(true);
            closeParticle.Play();
            yield return new WaitForSeconds(closeParticle.main.duration);
            closeParticle.gameObject.SetActive(false);
        }

        public void SetPosition(Vector3 position)
        {
            _transform.position = position;
        }

        public Vector3 GetSpawnPointPosition() => spawnPoint.position;
        public Vector3 GetPosition() => _transform.position;
    }
}