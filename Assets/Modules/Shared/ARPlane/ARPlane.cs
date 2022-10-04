// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System;
using UnityEngine;

namespace Niantic.ARVoyage
{
    public class ARPlane : MonoBehaviour
    {
        public static Action<ARPlane> PlaneCreated;
        public static Action<ARPlane> PlaneDestroyed;

        [SerializeField] Renderer _renderer;

        private void Start()
        {
            PlaneCreated?.Invoke(this);
        }

        public void Show(bool show)
        {
            _renderer.enabled = show;
        }

        private void OnDestroy()
        {
            PlaneDestroyed?.Invoke(this);
        }
    }
}
