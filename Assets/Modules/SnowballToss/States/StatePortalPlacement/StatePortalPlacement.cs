// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System.Collections;
using Niantic.ARVoyage;
using Niantic.ARVoyage.SnowballToss;
using UnityEngine;

namespace Modules.SnowballToss.States.StatePortalPlacement
{
    public class StatePortalPlacement : StateBase
    {
        private SnowballTossManager snowballTossManager;

        [Header("State machine")]
        [SerializeField] private GameObject nextState;
        private bool running;
        private float timeStartedState;
        private GameObject thisState;
        private GameObject exitState;
        protected float initialDelay = 0f;

        private Fader fader;

        private AudioManager audioManager;

        void Awake()
        {
            // We're not the first state; start off disabled
            gameObject.SetActive(false);

            snowballTossManager = SceneLookup.Get<SnowballTossManager>();
            fader = SceneLookup.Get<Fader>();
            audioManager = SceneLookup.Get<AudioManager>();
        }

        void OnEnable()
        {
            thisState = this.gameObject;
            exitState = null;
            Debug.Log("Starting " + thisState);
            timeStartedState = Time.time;

            // initialize game state
            snowballTossManager.InitTossGame();
            // create snowball, show snowball toss button
            snowballTossManager.snowballMaker.gameObject.SetActive(true);
            running = true;
        }


        void Update()
        {
            if (!running) return;
            
            if (exitState)
            {
                Exit(exitState);
            }

            // Update toss game
            snowballTossManager.UpdateTossGame();
        }


        private void Exit(GameObject nextState)
        {
            running = false;

            StartCoroutine(ExitRoutine(nextState));
        }

        private IEnumerator ExitRoutine(GameObject nextState)
        {
            Debug.Log("Snowball toss game over, score " + snowballTossManager.gameScore);

            // disable snowball and toss button
            snowballTossManager.snowballMaker.Expire();

            // disable snowballMaker
            snowballTossManager.snowballMaker.gameObject.SetActive(false);

            // expire all snowrings
            snowballTossManager.ExpireAllSnowrings();
            
            yield return null;

            Debug.Log(thisState + " transitioning to " + nextState);

            nextState.SetActive(true);
            thisState.SetActive(false);
        }

    }
}
