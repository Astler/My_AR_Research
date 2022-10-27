using Niantic.ARDK.AR.Anchors;
using UnityEngine;

namespace Playground
{
    public class ARAnchorFollower : MonoBehaviour
    {
        public IARAnchor Anchor;

        private void Update()
        {
            // if (!gameObject.activeSelf) return;
            //
            // if (Anchor == null)
            // {
            //     Destroy(gameObject);
            //     return;
            // }
            //
            // transform.position = Anchor.Transform.ToPosition();
            // transform.rotation = Anchor.Transform.ToRotation();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}