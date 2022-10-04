using System.Collections;
using Prototype.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace StateInstructions
{
    public class InstructionsScreen : ScreenView
    {
        [SerializeField] private ScreenView nextScreen;

        [Space, SerializeField] private Button okButton;

        private void OnEnable()
        {
            okButton.onClick.AddListener(OnClickedOk);
            BackdropView.SetActive(true);
        }

        private void OnDisable()
        {
            okButton.onClick.RemoveListener(OnClickedOk);
        }

        private void OnClickedOk() => StartCoroutine(ExitRoutine());

        private IEnumerator ExitRoutine()
        {
            if (nextScreen)
            {
                nextScreen.SetActive(true);
                yield return null;
            }

            SetActive(false);
        }
    }
}