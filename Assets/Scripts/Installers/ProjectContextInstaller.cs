using Core;
using Core.WebSockets;
using Data;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Infrastructure.GameStateMachine;
using SceneManagement;
using Screens;
using Screens.ArModeTab;
using Screens.Factories;
using Screens.FindDropZonesScreen;
using Screens.RewardsListScreen;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        [SerializeField] private Transform viewsParent;
        
        [SerializeField] private RewardCardView rewardView;
        [SerializeField] private DropZoneCardView dropZoneCardView;
        [FormerlySerializedAs("historyCardView")] [SerializeField] private HistoryEventCardView historyEventCardView;

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
        }

        private class RewardCardsPool : MonoPoolableMemoryPool<RewardViewInfo, IMemoryPool, RewardCardView> { }
        private class HistoryCardsPool : MonoPoolableMemoryPool<HistoryStepData, IMemoryPool, HistoryEventCardView> { }
        private class DropZoneCardsPool : MonoPoolableMemoryPool<DropZoneViewInfo, IMemoryPool, DropZoneCardView> { }
    }
}