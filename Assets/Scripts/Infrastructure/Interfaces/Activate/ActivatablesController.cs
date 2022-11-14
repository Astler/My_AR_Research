using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace Infrastructure.Interfaces.Activate
{
    public class ActivatablesController : IInitializable, IDisposable
    {
        private readonly IActivatablesQueue _activatablesQueue;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public ActivatablesController(IActivatablesQueue activatablesQueue)
        {
            _activatablesQueue = activatablesQueue;
        }

        public void Initialize()
        {
            Observable.TimerFrame(1).Subscribe(delegate(long l)
            {
                IEnumerable<IActivatable> activatablesQueue = _activatablesQueue.GetActivatablesQueue();
                foreach (IActivatable activatable in activatablesQueue)
                {
                    activatable.Activate();
                }
            }).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}