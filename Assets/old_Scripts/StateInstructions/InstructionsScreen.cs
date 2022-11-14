using Prototype.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace StateInstructions
{
    public class InstructionsScreen : ScreenView
    {
        [Space, SerializeField] private Button okButton;

        protected override void OnStepStarted()
        {
            //BackdropView.SetActive(true);
        }

        protected override void OnStepFinished()
        {
        }

        protected override void OnAwake()
        {
            okButton.onClick.AddListener(OnClickedOk);
        }

        private void OnDestroy()
        {
            okButton.onClick.RemoveListener(OnClickedOk);
        }

        private void OnClickedOk() => FinishStep();
    }
}