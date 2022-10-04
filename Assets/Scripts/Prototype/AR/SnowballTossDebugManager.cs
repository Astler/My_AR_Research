// Copyright 2022 Niantic, Inc. All Rights Reserved.

using Niantic.ARDK.Extensions.Meshing;
using Niantic.ARVoyage;
using UI.DebugMenu;
using UnityEngine;

namespace Prototype.AR
{
    public class SnowballTossDebugManager : MonoBehaviour
    {
        // Used to cleanly map features to menu checkbox indices
        private enum CheckboxIndex
        {
            VisualizePersistentMesh = 0,
        }

        ARMeshManager arMeshManager;

        [SerializeField] DebugMenuGUI debugMenuGUI;


        public bool VisualizePersistentMesh
        {
            get { return !arMeshManager.UseInvisibleMaterial; }

            set { arMeshManager.UseInvisibleMaterial = !value; }
        }

        void Start()
        {
            // SDK
            arMeshManager = FindObjectOfType<ARMeshManager>();
            // Set initial checkbox values
            debugMenuGUI.SetChecked((int)CheckboxIndex.VisualizePersistentMesh, VisualizePersistentMesh);
        }

        void OnEnable()
        {
            // Subscribe to events
            DebugMenuGUI.EventDebugOption1Checkbox += OnEventDebugOption1Checkbox; // persistent mesh
            DebugMenuGUI.EventDebugOption5Button += OnEventDebugOption5Button; // clear mesh
        }

        void OnDisable()
        {
            // Unsubscribe from events
            DebugMenuGUI.EventDebugOption1Checkbox -= OnEventDebugOption1Checkbox;
            DebugMenuGUI.EventDebugOption5Button -= OnEventDebugOption5Button;
        }


        // persistent mesh
        private void OnEventDebugOption1Checkbox()
        {
            Debug.Log("OnEventDebugOption1Checkbox");
            VisualizePersistentMesh = !VisualizePersistentMesh;
        }

        // clear mesh
        private void OnEventDebugOption5Button()
        {
            Debug.Log("OnEventDebugOption5Button");
            ClearPersistentMesh();
        }

        private void ClearPersistentMesh()
        {
            arMeshManager.ClearMeshObjects();
        }
    }
}