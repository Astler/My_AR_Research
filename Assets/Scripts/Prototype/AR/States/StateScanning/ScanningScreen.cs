using System.Collections;
using Modules.Shared.VFX.FeaturePoints;
using Niantic.ARDK.Extensions.Meshing;
using Prototype.Screens;
using TMPro;
using UnityEngine;

namespace Prototype.AR.States.StateScanning
{
    public class ScanningScreen : ScreenView
    {
        [SerializeField] public ARMeshManager arMeshUpdater;
 
        private FeaturePointHelper _featurePointHelper;

        [Header("Pseudo State machine")] [SerializeField]
        private ScreenView nextScreen;

        private bool running;
        private float timeStartedState;
        protected float initialDelay = 1f;

        [Header("GUI")] [SerializeField] private GameObject gui;
        [SerializeField] private TMP_Text scanningText;
 
        private bool foundMesh;

        protected const float minScanningDuration = 5f;

        protected override void OnAwake()
        {
            _featurePointHelper = SceneFinder.TryGet<FeaturePointHelper>();
            arMeshUpdater.MeshObjectsUpdated += MeshObjectsUpdated;
        }

        private void OnEnable()
        {
            BackdropView.SetActive(false);
            
            timeStartedState = Time.time;
            
            _featurePointHelper.Tracking = true;
            _featurePointHelper.Spawning = true;

            running = true;
        }

        private void OnDestroy()
        {
            arMeshUpdater.MeshObjectsUpdated -= MeshObjectsUpdated;
        }

        private void MeshObjectsUpdated(MeshObjectsUpdatedArgs args)
        {
            if (args.BlocksUpdated == null) return;
            
            Debug.Log("StateScanning found mesh");
            foundMesh = true;
        }

        private void Update()
        {
            if (foundMesh &&
                Time.time - timeStartedState > minScanningDuration)
            {
                StartCoroutine(ExitRoutine());
            }
        }
        
        private IEnumerator ExitRoutine()
        {
            // Turn off feature point rendering
            _featurePointHelper.Tracking = false;
            _featurePointHelper.Spawning = false;

            // Fade out GUI
            //TODO FADE OUT GUI
            yield return null;
        
            nextScreen.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}