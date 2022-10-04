using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Prototype
{
    public class SceneFinder : MonoBehaviour
    {
        private static SceneFinder _instance;

        public static SceneFinder Instance =>
            _instance ? _instance : _instance = Instantiate(new GameObject()).AddComponent<SceneFinder>();

        private readonly Dictionary<Type, Object> _lookup = new();

        public T TryGet<T>(bool warnIfNotFound = true) where T : Object
        {
            if (_lookup.TryGetValue(typeof(T), out Object entry))
            {
                return entry as T;
            }

            T objectOnScene = FindObjectOfType<T>();

            if (objectOnScene)
            {
                _lookup[typeof(T)] = objectOnScene;
                return objectOnScene;
            }

            if (warnIfNotFound)
            {
                Debug.LogWarning(nameof(SceneFinder) + " didn't find object of type " + typeof(T).Name);
            }

            return null;
        }
    }
}