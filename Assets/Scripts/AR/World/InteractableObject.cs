using System;

namespace AR.World
{
    public interface IInteractable<out T>
    {
        public event Action<T> Interacted;
        public void Interact();
    }
}