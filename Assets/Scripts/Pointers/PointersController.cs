using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Pointers
{
    public interface IPointersController
    {
        void AddTarget(IPointerTarget pointerTarget);
        void RemoveTarget(IPointerTarget pointerTarget);
    }

    public class PointersController : IPointersController, IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly IDataProxy _dataProxy;
        private readonly List<IPointerTarget> _targets = new();
        private readonly ReactiveProperty<IPointerTarget> _currentPointer = new();

        public PointersController(IDataProxy dataProxy, IDropLocationDirectionPointer pointer)
        {
            _dataProxy = dataProxy;

            _dataProxy.PlayerLocationChanged.Subscribe(_ => { UpdateCurrentPointer(); }).AddTo(_compositeDisposable);

            _currentPointer.Subscribe(target =>
            {
                pointer.SetIsVisible(target != null);
                pointer.SetTarget(target);
            }).AddTo(_compositeDisposable);
        }

        public void AddTarget(IPointerTarget pointerTarget)
        {
            _targets.Add(pointerTarget);
            UpdateCurrentPointer();
        }

        public void RemoveTarget(IPointerTarget pointerTarget)
        {
            _targets.Remove(pointerTarget);
            UpdateCurrentPointer();
        }

        [CanBeNull]
        public IPointerTarget GetNearestPointerTarget()
        {
            if (_targets.Count == 0) return null;

            bool isInsideZone = _dataProxy.SelectedPortalZone.Value != null;

            List<IPointerTarget> correctPointers = _targets.Where(it =>
                it.TargetType == (isInsideZone ? TargetType.Present : TargetType.DropZone)).ToList();

            if (correctPointers.Count == 0) return null;

            Vector2 rawPosition = _dataProxy.GetPlayerPosition();
            Vector3 playerPosition = new(rawPosition.x, 0f, rawPosition.y);

            IPointerTarget pointer = correctPointers.OrderBy(it => Vector3.Distance(playerPosition, it.GetPosition()))
                .FirstOrDefault();

            return pointer;
        }

        private void UpdateCurrentPointer()
        {
            IPointerTarget nearest = GetNearestPointerTarget();

            if (_currentPointer.Value == nearest) return;

            _currentPointer.Value = nearest;
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}