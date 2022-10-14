using System;
using Prototype.Assets;
using Prototype.Screens;
using UniRx;
using UnityEngine;

namespace Prototype.States.LocationDetection
{
    public class LocationDetectionScreen : ScreenView
    {
        private ProjectContext _context;
        private float _startTime;
        private bool _transitionActive;
        private IDisposable _connectionListener;
        private const float MinStepDuration = 2f;

        protected override void OnAwake()
        {
            _context = SceneFinder.TryGet<ProjectContext>();
        }

        protected override void OnStepStarted()
        {
            Debug.Log("Starting location");
            _startTime = Time.time;
            _transitionActive = false;
            BackdropView.SetActive(false);

            _connectionListener = _context.GetLocationController().NearestPortalZone.Subscribe(
                delegate(PortalZoneModel model)
                {
                    if (model == null || _transitionActive) return;

                    _transitionActive = true;

                    float timeFromStart = Time.time - _startTime;

                    Observable.Timer(TimeSpan.FromSeconds(timeFromStart >= MinStepDuration
                        ? 0
                        : MinStepDuration - timeFromStart)).Subscribe(_ => FinishStep()).AddTo(this);
                }).AddTo(this);
        }

        protected override void OnStepFinished()
        {
            Debug.Log("Finishing location");
            _connectionListener?.Dispose();
        }
    }
}