using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Assets;
using UniRx;
using UnityEngine;

namespace Prototype.Screens
{
    public class GameStepsController : MonoBehaviour
    {
        [SerializeField] private ScreenView instructionScreen;
        [SerializeField] private ScreenView locationSearchScreen;
        [SerializeField] private ScreenView meshScannerScreen;
        [SerializeField] private ScreenView portalPlacementScreen;

        private readonly List<IGameStep> _steps = new();
        private ProjectContext _projectContext;
        private IGameStep _currentStep;

        private void Awake()
        {
            _steps.Add(instructionScreen);
            _steps.Add(locationSearchScreen);
            _steps.Add(meshScannerScreen);
            _steps.Add(portalPlacementScreen);

            _projectContext = SceneFinder.Instance.TryGet<ProjectContext>();

            foreach (IGameStep gameStep in _steps)
            {
                gameStep.Finished += OnStepFinished;
                gameStep.Started += OnStepStarted;
            }
        }

        private void Start()
        {
            foreach (IGameStep gameStep in _steps)
            {
                gameStep.SetActive(false, true);
            }
            
            _projectContext.GetLocationController().SelectedPortalZone.Skip(1).Subscribe(delegate(PortalZoneModel model)
            {
                // if (model == null && _currentStep != null)
                // {
                //     int index = _steps.IndexOf(_currentStep);
                //     int locationIndex = _steps.IndexOf(locationSearchScreen);
                //
                //     if (index <= locationIndex) return;
                //     
                //     _currentStep?.FinishStep();
                //     locationSearchScreen.StartStep();
                // }
            }).AddTo(this);

            _steps.First().StartStep();
        }

        private void OnStepStarted(IGameStep step)
        {
            if (_currentStep != null)
            {
                Debug.LogError(
                    $"Step started {step.GetType().Name} with another active step already {_currentStep.GetType().Name}");
            }

            _currentStep = step;
        }

        private void OnStepFinished(IGameStep step)
        {
            if (_currentStep != step)
            {
                Debug.LogError($"Inactive step finished {step.GetType().Name}");
            }

            _currentStep = null;

            int index = _steps.IndexOf(step);

            if (index >= _steps.Count) return;

            _steps[index + 1].StartStep();
        }
    }
}