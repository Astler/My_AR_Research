using System;
using UnityEngine;

namespace AR.World.Collectable
{
    public interface ICollectable
    {
        event Action<ICollectable> Interacted;
        event Action<(ICollectable collectable, bool canBeCollected)> CollectableStatusChanged;

        public void Interact();

        public bool IsCanBeCollected(Vector3 playerPosition);
        void IsInsidePlayerARCollider(bool isInside);
        void SetupCollectAction(Action action);
    }
}