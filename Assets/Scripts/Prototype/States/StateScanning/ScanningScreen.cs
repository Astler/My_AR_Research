using System;
using Modules.Shared.VFX.FeaturePoints;
using Niantic.ARDK.Extensions.Meshing;
using Prototype.Screens;
using UniRx;
using UnityEngine;

namespace Prototype.States.StateScanning
{
    public class ScanningScreen : ScreenView
    {
        private const float MinStepDuration = 2f;
        private const float MinScanningDuration = 5f;

        private ARMeshManager _arMeshUpdater;
        private FeaturePointHelper _featurePointHelper;
        private bool _foundMesh, _startedTransition;
        private float _startTime;

        protected override void OnAwake()
        {
            _featurePointHelper = SceneFinder.TryGet<FeaturePointHelper>();
            _arMeshUpdater = SceneFinder.TryGet<ARMeshManager>();
            _arMeshUpdater.MeshObjectsUpdated += MeshObjectsUpdated;
        }

        protected override void OnStepStarted()
        {
            Debug.Log("Starting scan");
            BackdropView.SetActive(false);

            _startTime = Time.time;
            _startedTransition = false;

            _featurePointHelper.Tracking = true;
            _featurePointHelper.Spawning = true;

            if (Application.isEditor)
            {
                Observable.Timer(TimeSpan.FromSeconds(MinStepDuration)).Subscribe(_ => FinishStep()).AddTo(this);
                return;
            }
        }

        protected override void OnStepFinished()
        {
            Debug.Log("Finishing scan");
            _featurePointHelper.Tracking = false;
            _featurePointHelper.Spawning = false;
        }

        private void OnDestroy()
        {
            _arMeshUpdater.MeshObjectsUpdated -= MeshObjectsUpdated;
        }

        private void MeshObjectsUpdated(MeshObjectsUpdatedArgs args)
        {
            if (args.BlocksUpdated == null) return;

            // Debug.Log("StateScanning found mesh");
            _foundMesh = true;
        }

        private void Update()
        {
            if (_foundMesh && !_startedTransition &&
                Time.time - _startTime > MinScanningDuration)
            {
                _startedTransition = true;
                FinishStep();
            }
        }
    }
}