// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.Shared.ARPlane
{
    /// <summary>
    /// Helper for accessing and utilizing the current collection of ARPlanes in the session
    /// </summary>
    public class ARPlaneHelper : MonoBehaviour
    {
        public static Action<ARPlaneHelper> PlanesChanged;

        [Tooltip("Should ARPlanes be shown in the scene?")]
        [SerializeField] private bool showPlanes;

        [Tooltip("Should ARPlanes gameObjects be active in the scene?")]
        [SerializeField] private bool planeObjectsActive = true;

        private HashSet<Niantic.ARVoyage.ARPlane> planes = new();

        public int NumPlanes => planes.Count;

        /// <summary>
        /// Get a copy of the the current planes
        /// </summary>
        public List<Niantic.ARVoyage.ARPlane> GetPlanes()
        {
            return new List<Niantic.ARVoyage.ARPlane>(planes);
        }

        /// <summary>
        /// Should AR planes be rendered?
        /// </summary>
        public void SetShowPlanes(bool showPlanes)
        {
            if (this.showPlanes != showPlanes)
            {
                this.showPlanes = showPlanes;

                foreach (Niantic.ARVoyage.ARPlane plane in planes)
                {
                    plane.Show(showPlanes);
                }
            }
        }

        /// <summary>
        /// Should the AR plane objects be active?
        /// </summary>
        public void SetPlaneObjectsActive(bool planeObjectsActive)
        {
            if (this.planeObjectsActive != planeObjectsActive)
            {
                this.planeObjectsActive = planeObjectsActive;

                foreach (Niantic.ARVoyage.ARPlane plane in planes)
                {
                    plane.gameObject.SetActive(planeObjectsActive);
                }
            }
        }

        private void Awake()
        {
            // Listen to plane created and destroy events for the lifetime of this helper
            Niantic.ARVoyage.ARPlane.PlaneCreated += OnPlaneCreated;
            Niantic.ARVoyage.ARPlane.PlaneDestroyed += OnPlaneDestroyed;
        }

        private void OnPlaneCreated(Niantic.ARVoyage.ARPlane plane)
        {
            // Track the plane
            planes.Add(plane);
            Debug.Log("OnPlaneCreated: " + plane.name);

            // Set initial plane state
            plane.Show(showPlanes);
            plane.gameObject.SetActive(planeObjectsActive);

            PlanesChanged.Invoke(this);
        }

        private void OnPlaneDestroyed(Niantic.ARVoyage.ARPlane plane)
        {
            Debug.Log("OnPlaneDestroyed: " + plane.name);
            planes.Remove(plane);

            PlanesChanged.Invoke(this);
        }

        private void OnDestroy()
        {
            Niantic.ARVoyage.ARPlane.PlaneCreated -= OnPlaneCreated;
            Niantic.ARVoyage.ARPlane.PlaneDestroyed -= OnPlaneDestroyed;
        }

        // Comment in to test
        // void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.S))
        //     {
        //         SetShowPlanes(!showPlanes);
        //     }

        //     if (Input.GetKeyDown(KeyCode.A))
        //     {
        //         SetPlaneObjectsActive(!planeObjectsActive);
        //     }
        // }
    }
}
