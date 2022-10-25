using UnityEngine;

namespace Playground
{
    public class TransformDebugger : MonoBehaviour
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            Debug.Log($"{name} pos = {_transform.position} rot = {_transform.rotation.eulerAngles} ");
        }
    }
}