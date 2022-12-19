using AR.World;
using AR.World.Collectable;
using UnityEngine;

namespace GameCamera
{
    [RequireComponent(typeof(BoxCollider))]
    public class InteractionDetector : MonoBehaviour
    {
        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            _boxCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<ICollectable>()?.IsInsidePlayerARCollider(true);
        }

        private void OnTriggerStay(Collider other)
        {
            other.GetComponent<ICollectable>()?.IsInsidePlayerARCollider(true);
        }

        private void OnTriggerExit(Collider other)
        {
            other.GetComponent<ICollectable>()?.IsInsidePlayerARCollider(false);
        }
    }
}