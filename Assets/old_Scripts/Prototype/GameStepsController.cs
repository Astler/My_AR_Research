using System.Collections.Generic;
using System.Linq;
using Prototype.Screens;
using UnityEngine;

namespace old_Scripts.Prototype.Core
{
    public class GameStepsController : MonoBehaviour
    {
        [SerializeField] private ScreenView instructionScreen;
        [SerializeField] private ScreenView locationSearchScreen;
        [SerializeField] private ScreenView meshScannerScreen;
        [SerializeField] private ScreenView portalPlacementScreen;

        private readonly List<IGameStep> _steps = new();
        private IGameStep _currentStep;

        private void Awake()
        {
            _steps.Add(instructionScreen);
            _steps.Add(locationSearchScreen);
            _steps.Add(meshScannerScreen);
            _steps.Add(portalPlacementScreen);

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