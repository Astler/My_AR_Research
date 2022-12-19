using Core;
using Core.WebSockets;
using Data;
using Data.Objects;
using ExternalTools.ImagesLoader;
using Geo;
using Infrastructure.GameStateMachine;
using SceneManagement;
using Screens;
using Screens.Factories;
using Screens.RewardsListScreen;
using UnityEngine;
using Utils;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        [SerializeField] private RewardCardView rewardView;
        [SerializeField] private Transform viewsParent;

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
        }

        private class RewardCardsPool : MonoPoolableMemoryPool<RewardViewInfo, IMemoryPool, RewardCardView> { }
    }
}