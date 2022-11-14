using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class ComponentPool<T> where T : Component
    {
        private readonly GameObject _gameObject;
        private readonly Stack<T> _components = new Stack<T>();

        public ComponentPool(GameObject gameObject, int initialSize = 0)
        {
            _gameObject = gameObject;
            for (int i = 0; i < initialSize; i++)
            {
                _components.Push(_gameObject.AddComponent<T>());
            }
        }

        public T Get()
        {
            T component = _components.Count == 0 ? _gameObject.AddComponent<T>() : _components.Pop();
            return component;
        }

        public void Release(T component)
        {
            _components.Push(component);
        }
    }
}