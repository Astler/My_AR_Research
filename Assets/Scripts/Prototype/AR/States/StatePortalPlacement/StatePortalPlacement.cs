// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System.Collections;
using Niantic.ARVoyage;
using UnityEngine;

namespace Modules.SnowballToss.States.StatePortalPlacement
{
    public class StatePortalPlacement : StateBase
    {
        [Header("State machine")] [SerializeField]
        private GameObject nextState;

        private bool running;
        private GameObject thisState;
        private GameObject exitState;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            thisState = this.gameObject;
            exitState = null;
            Debug.Log("Starting " + thisState);
            running = true;
        }
        
        void Update()
        {
            if (!running) return;

            if (exitState)
            {
                Exit(exitState);
            }
        }
        
        private void Exit(GameObject nextState)
        {
            running = false;
            StartCoroutine(ExitRoutine(nextState));
        }

        private IEnumerator ExitRoutine(GameObject nextState)
        {
            yield return null;

            Debug.Log(thisState + " transitioning to " + nextState);

            nextState.SetActive(true);
            thisState.SetActive(false);
        }
    }
}