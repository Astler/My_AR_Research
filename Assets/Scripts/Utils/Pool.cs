// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace Utils
{
    public class Pool<T>
    {
        private readonly List<T> _pooledObjects = new();
        private readonly List<T> _borrowedObjects = new();

        private Func<T> _createCallback;

        public int PooledCount
        {
            get { return _pooledObjects.Count; }
        }

        public int BorrowedCount
        {
            get { return _borrowedObjects.Count; }
        }

        public void Initialize(Func<T> createCallback, int count)
        {
            _createCallback = createCallback;

            for (int i = 0; i < count; i++)
            {
                T instance = createCallback();
                _pooledObjects.Add(instance);
            }
        }

        public T BorrowItem()
        {
            if (_pooledObjects.Count > 0)
            {
                T borrowedObject = _pooledObjects[0];
                _pooledObjects.Remove(borrowedObject);
                _borrowedObjects.Add(borrowedObject);
                return borrowedObject;
            }

            T newObject = _createCallback();
            _borrowedObjects.Add(newObject);
            return newObject;
        }

        public void ReturnItem(T poolObject)
        {
            _borrowedObjects.Remove(poolObject);
            _pooledObjects.Add(poolObject);
        }

        public void ReturnAll(Action<T> returnCallback)
        {
            for (int i = _borrowedObjects.Count - 1; i >= 0; i--)
            {
                T borrowedObject = _borrowedObjects[i];

                returnCallback(borrowedObject);

                _borrowedObjects.Remove(borrowedObject);
                _pooledObjects.Add(borrowedObject);
            }
        }
    }
}
