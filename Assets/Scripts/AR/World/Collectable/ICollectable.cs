using System;
using UnityEngine;

namespace AR.World.Collectable
{
    public interface ICollectable
    {
        public event Action<ICollectable> Interacted; 

        public void Interact();
        
        public bool CanBeCollected(Vector3 playerPosition);
        void IsInsidePlayerARCollider(bool isInside);
        void SetupCollectAction(Action action);
    }
}