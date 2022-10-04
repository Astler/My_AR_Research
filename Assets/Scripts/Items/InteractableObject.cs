using System;

namespace Items
{
    public interface IInteractable<out T>
    {
        public event Action<T> Interacted;
        public void Interact();
    }
}