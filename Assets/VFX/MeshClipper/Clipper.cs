// Copyright 2022 Niantic, Inc. All Rights Reserved.

using UnityEngine;

namespace Niantic.ARVoyage.Walkabout
{
    /// <summary>
    /// Experimental class to control the Clipper shader material to manage Gameboard occlusion based on elevation and a simulated "water level"
    /// </summary>
    public class Clipper : MonoBehaviour
    {
        [SerializeField] float waterLevel;
        [SerializeField] float surfaceOffset;
        [SerializeField] float surfaceElevation;

        /*
        void Awake()
        {
            GameboardHelper.SurfaceUpdated.AddListener(OnSurfaceUpdated);
        }

        void OnDestroy()
        {
            GameboardHelper.SurfaceUpdated.RemoveListener(OnSurfaceUpdated);
        }

        public void OnSurfaceUpdated(Surface surface)
        {
            surfaceElevation = surface.Elevation;
        }
        */

        void Update()
        {
            waterLevel = surfaceElevation + surfaceOffset;
            Shader.SetGlobalFloat("_WaterLevel", waterLevel);
        }
    }
}
