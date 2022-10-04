// Copyright 2022 Niantic, Inc. All Rights Reserved.

using Prototype;
using UnityEngine;
using UnityEngine.UI;

namespace Niantic.ARVoyage
{
    /// <summary>
    /// Universal button in the upper right corner of all scenes, 
    /// allowing the player to display/hide the current scene's debug menu GUI.
    /// </summary>
    public class DebugMenuButton : MonoBehaviour
    {
        [SerializeField] private Button debugMenuButton;
        [SerializeField] private GameObject debugMenuGUI;
        [SerializeField] private GameObject fullscreenScrim;

        private void OnEnable()
        {
            debugMenuButton.onClick.AddListener(OnDebugMenuButtonClick);
        }

        private void OnDisable()
        {
            debugMenuButton.onClick.RemoveListener(OnDebugMenuButtonClick);
        }

        private void OnDebugMenuButtonClick()
        {
            ShowDebugMenuGUI(!debugMenuGUI.activeSelf);
            ButtonSFX();
        }

        public void ShowDebugMenuGUI(bool show)
        {
            fullscreenScrim.SetActive(show);
            debugMenuGUI.SetActive(show);
        }

        protected void ButtonSFX(string audioKey = AudioKeys.UI_Button_Press)
        {
            AudioManager audioManager = SceneFinder.Instance.TryGet<AudioManager>();
            audioManager?.PlayAudioNonSpatial(audioKey);
        }
    }
}
