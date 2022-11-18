using Core;
using Core.WebSockets;
using Data;
using Geo;
using Infrastructure.GameStateMachine;
using SceneManagement;
using Screens;
using Zenject;

namespace Installers
{
    public class ProjectContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
            Container.Bind<ScreensInstaller>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.Bind<BackdropView>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ScreenNavigationSystem>().FromNew().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<DataProxy>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<ApiInterface>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WebSocketService>().AsSingle().NonLazy();
        }
    }
}