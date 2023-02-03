using System;
using TMPro;
using UniRx;
using UnityEngine;
using Utils;

namespace AR.World.Collectable
{
    public class MannaBoxView : CollectableItem
    {
        [SerializeField] private Animator boxAnimator;
        [SerializeField] private TMP_Text distanceToPlayer;

        private int _id;
        private static readonly int Nearby = Animator.StringToHash("nearby");
        private static readonly int Collected = Animator.StringToHash("collected");
        public int DropId => _id;

        public void SetBeamData(int id)
        {
            _id = id;
        }

        protected override void OnUpdate()
        {
            Vector3 position = Transform.position;
            
            Vector3 playerPosition = Camera.transform.position;
            double distance = Vector3.Distance(playerPosition, position) / 1000;
            distanceToPlayer.text = distance.DistanceToHuman();
            
            Debug.DrawRay(new Vector3(position.x, 0f, position.z), Vector3.down, Color.red);
        }

        protected override void OnCollectAbilityChanges(bool canBeCollected)
        {
            boxAnimator.SetBool(Nearby, canBeCollected);
        }

        public override void Interact(Action onInteractionFinished)
        {
            boxAnimator.SetBool(Collected, true);

            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(delegate(long l)
            {
                base.Interact(onInteractionFinished);
            }).AddTo(this);
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            boxAnimator.SetBool(Collected, false);
            boxAnimator.SetBool(Nearby, false);
            
        }
    }
}