//using Sounds;
using UnityEngine;
using Utils.StateMachine;

namespace Infrastructure.GameStateMachine.GameStates
{
    public class GameLoopState : IState
    {
        /*private readonly SoundsManager _soundsManager;

        public GameLoopState(SoundsManager soundsManager)
        {
            _soundsManager = soundsManager;
        }*/

        public void Enter()
        {
            DynamicGI.UpdateEnvironment();
            //_soundsManager.PlayAmbiences();
        }

        public void Exit()
        {
        }
    }
}