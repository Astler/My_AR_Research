using Core;
using Core.WebSockets;
using Data;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Infrastructure.GameStateMachine;
using SceneManagement;
using Screens;
using Screens.ArModeTab;
using Screens.DropZoneDetailsScreen;
using Screens.Factories;
using Screens.FindDropZonesScreen;
using Toasts;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        [SerializeField] private Transform viewsParent;

        [SerializeField] private RewardCardView rewardView;
        [SerializeField] private DropZoneCardView dropZoneCardView;
        [SerializeField] private HistoryEventCardView historyEventCardView;
        [SerializeField] private ToastView toastView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
            Container.Bind<ScreensInstaller>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<BackdropView>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScreenNavigationSystem>().FromNew().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<DataProxy>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<WebImagesLoader>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ApiInterface>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WebSocketService>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<ToastsController>().AsSingle().NonLazy();
            
            BindFactories();
        }

        private void BindFactories()
        {
            Container.BindFactory<RewardViewInfo, RewardCardView, RewardCardsFactory>()
                .FromPoolableMemoryPool<RewardViewInfo, RewardCardView, RewardCardsPool>(
                    poolBinder => poolBinder
                        .WithInitialSize(30)
                        .FromComponentInNewPrefab(rewardView)
                        .UnderTransform(viewsParent));

            Container.BindFactory<HistoryStepData, HistoryEventCardView, HistoryCardsFactory>()
                .FromPoolableMemoryPool<HistoryStepData, HistoryEventCardView, HistoryCardsPool>(
                    poolBinder => poolBinder
                        .WithInitialSize(30)
                        .FromComponentInNewPrefab(historyEventCardView)
                        .UnderTransform(viewsParent));

            Container.BindFactory<DropZoneViewInfo, DropZoneCardView, DropZonesCardsFactory>()
                .FromPoolableMemoryPool<DropZoneViewInfo, DropZoneCardView, DropZoneCardsPool>(
                    poolBinder => poolBinder
                        .WithInitialSize(30)
                        .FromComponentInNewPrefab(dropZoneCardView)
                        .UnderTransform(viewsParent));

            Container.BindFactory<ToastViewInfo, ToastView, ToastsFactory>()
                .FromPoolableMemoryPool<ToastViewInfo, ToastView, ToastsPool>(
                    poolBinder => poolBinder
                        .WithInitialSize(30)
                        .FromComponentInNewPrefab(toastView)
                        .UnderTransform(viewsParent));
        }

        private class RewardCardsPool : MonoPoolableMemoryPool<RewardViewInfo, IMemoryPool, RewardCardView> { }

        private class HistoryCardsPool : MonoPoolableMemoryPool<HistoryStepData, IMemoryPool, HistoryEventCardView> { }

        private class DropZoneCardsPool : MonoPoolableMemoryPool<DropZoneViewInfo, IMemoryPool, DropZoneCardView> { }

        private class ToastsPool : MonoPoolableMemoryPool<ToastViewInfo, IMemoryPool, ToastView> { }
    }
}