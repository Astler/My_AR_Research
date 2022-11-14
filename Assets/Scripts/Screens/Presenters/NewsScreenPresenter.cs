using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.MockApi;
using Screens.Interfaces;
using Screens.Views;
using UnityEngine;

namespace Screens.Presenters
{
    public class Sell
    {
        public bool isActive;
        public GameObject view;
    }
    
    public class NewsScreenPresenter
    {
        private readonly INewsScreenView _view;
        private readonly IScreenNavigationSystem _screenNavigationSystem;
        private MockApiInterface _apiInterface;
        private readonly int _itemsPerPage = 10;
        private List<Sell> _sells = new();
        private event Action TimeTick;
        
        public NewsScreenPresenter (INewsScreenView view, IScreenNavigationSystem screenNavigationSystem)
        {
            _view = view;
            _screenNavigationSystem = screenNavigationSystem;
            _apiInterface = new MockApiInterface();
            Initialize();
        }
        
        private void Initialize()
        {
            _view.OnShowCallback += delegate
            {
                _screenNavigationSystem.HideScreenInformation(_view);
            };
            
            // _view.OnGotFocusCallback += ScreenGotFocus;
            

            // _view.GetPagination().Initialize(_itemsPerPage,
            //     (perPage, refreshCallback) => RefreshPage(refreshCallback, perPage),
            //     (page, perPage, uploadCallback) => UploadPage(uploadCallback, page, perPage));
        }

        // private void ScreenGotFocus()
        // {
        //     _view.GetPagination().ResetScroll();
        //     RefreshPage(delegate
        //     {
        //         _screenNavigationSystem.ShowScreenInformation(_view);
        //     }, _itemsPerPage);
        // }
        //
        // private void UploadPage(Action afterUpload, int page, int perPage)
        // {
        //     GetCompletedGames(delegate
        //         {
        //             UpdateView();
        //             afterUpload?.Invoke();
        //         },
        //         false, page, perPage);
        // }
        //
        // private void RefreshPage(Action afterRefresh, int perPage)
        // {
        //     GetCompletedGames(delegate
        //         {
        //             UpdateView();
        //             afterRefresh?.Invoke();
        //         },
        //         true,1, perPage);
        // }
        //
        // private void GetCompletedGames(Action completedCallback, bool isRefresh, int page, int perPage)
        // {
        //     _apiInterface.GetNews(page, perPage, OnSuccess, OnFailure);
        //     
        //     
        //     void OnMultiplayerSuccess(MultiplayerGames multiplayerGames)
        //     {
        //         _multiplayerGames.UpdateData(multiplayerGames, isRefresh);
        //         _view.GetPagination().CalculateTotalPage(multiplayerGames.total);
        //             
        //         completedCallback?.Invoke();
        //     }
        //
        //     void OnSuccess(FeedData feedData)
        //     {
        //         _games.UpdateData(feedData, isRefresh);
        //         _view.GetPagination().CalculateTotalPage(feedData.total);
        //             
        //         completedCallback?.Invoke();
        //     }
        //
        //     void OnFailure(ResponseStatus status)
        //     {
        //         Debug.LogError(status.errorMessage);
        //         completedCallback?.Invoke();
        //     }
        // }
        //
        // private void UpdateView()
        // {
        //     foreach (var sell in _sells)
        //     {
        //         sell.view.SetActive(false);
        //         sell.isActive = false;
        //     }
        //     
        //     CheckAndCreateSells(_games?.completed);
        //     
        // }
        //
        // private void CheckAndCreateSells(SocketDataModels.SocketMessageChallenge[] feedData, GameSellType type)
        // {
        //     Transform sellsParent;
        //     GameObject sellPrefab;
        //     
        //     if (feedData?.Length <= 0) return;
        //
        //     (sellsParent, sellPrefab) = _view.GetSellParentAndPrefab(type);
        //     CreateSellsUsingPull(feedData, sellsParent, sellPrefab);
        // }
        //
        // private void CreateSellsUsingPull(SocketDataModels.SocketMessageChallenge[] feedData, Transform sellsParent,
        //     GameObject sellPrefab)
        // {
        //     foreach (var sellData in feedData)
        //     {
        //         Sell someSell = new Sell();
        //         CheckAndCreateSomeSell(ref someSell, sellsParent, sellPrefab);
        //
        //         someSell.presenter.Initialize(sellData);
        //     }
        // }
        //
        // private void CheckAndCreateSomeSell(ref Sell someSell, Transform sellsParent, GameObject sellPrefab)
        // {
        //     foreach (var sell in _sells.Where(sell => !sell.isActive))
        //     {
        //         someSell = sell;
        //         someSell.view.transform.SetParent(sellsParent);
        //     }
        //
        //     if (someSell.presenter == null)
        //     {
        //         someSell.view = Object.Instantiate(sellPrefab, sellsParent);
        //         someSell.presenter = new FeedSellPresenter(
        //             someSell.view.GetComponent<FeedSellView>(), _authDataProxy, _localStorageHelper,
        //             _apiInterface, _screenNavigationSystem, _locationService, null, ref TimeTick);
        //         _sells.Add(someSell);
        //     }
        //
        //     someSell.isActive = true;
        //     someSell.view.SetActive(true);
        //     someSell.view.transform.SetAsLastSibling();
        // }

    }
}