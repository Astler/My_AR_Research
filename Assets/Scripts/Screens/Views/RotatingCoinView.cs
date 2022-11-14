using UnityEngine;

namespace Screens.Views
{
    public class RotatingCoinView : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 10f;
        
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            _transform.Rotate(rotateSpeed, 0f, 0f);
        }
    }
}