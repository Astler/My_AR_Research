using AR.World;
using UnityEngine;

namespace GameCamera
{
    [RequireComponent(typeof(BoxCollider))]
    public class InteractionDetector: MonoBehaviour
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
            ARAnchorFollower follower = other.GetComponent<ARAnchorFollower>();
            
            if (follower)
            {
                follower.SetIsArDistanceSelected(true);
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            ARAnchorFollower follower = other.GetComponent<ARAnchorFollower>();
            
            if (follower)
            {
                follower.SetIsArDistanceSelected(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {            ARAnchorFollower follower = other.GetComponent<ARAnchorFollower>();
            
            if (follower)
            {
                follower.SetIsArDistanceSelected(false);
            }
        }
    }
}