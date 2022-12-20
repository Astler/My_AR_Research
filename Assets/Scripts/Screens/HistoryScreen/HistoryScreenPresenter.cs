using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Objects;
using Screens.Factories;
using UniRx;

namespace Screens.HistoryScreen
{
    public class HistoryScreenPresenter
    {
        private readonly IHistoryScreenView _screenView;
        private readonly IDataProxy _dataProxy;
        private readonly HistoryCardsFactory _historyCardsFactory;
        private readonly List<HistoryCardView> _rewardsList = new();
        private IDisposable _rewardsListener;

        public HistoryScreenPresenter(IHistoryScreenView screenView, IDataProxy dataProxy,
            HistoryCardsFactory historyCardsFactory)
        {
            _screenView = screenView;
            _dataProxy = dataProxy;
            _historyCardsFactory = historyCardsFactory;

            Initialize();
        }

        private void Initialize()
        {
            _screenView.OnShowCallback += OnShow;
            _screenView.OnLostFocusCallback += OnLostFocus;
        }

        private void OnLostFocus() => _rewardsListener?.Dispose();

        private void OnShow(object _)
        {
            _rewardsListener = _dataProxy.SessionHistory.ObserveCountChanged().Subscribe(_ => LoadHistory());
            LoadHistory();
        }

        private void LoadHistory()
        {
            foreach (HistoryCardView rewardView in _rewardsList)
            {
                rewardView.Dispose();
            }

            foreach (HistoryStepData stepData in _dataProxy.SessionHistory.Reverse())
            {
                stepData.Parent = _screenView.GetListContainer();
                HistoryCardView cardView = _historyCardsFactory.Create(stepData);
                _rewardsList.Add(cardView);
            }
        }
    }
}