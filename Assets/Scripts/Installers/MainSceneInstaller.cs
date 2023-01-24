using AR;
using AR.FoundationAR;
using AR.World;
using GameCamera;
using Geo;
using Map;
using Pointers;
using Zenject;

namespace Installers
{
    public class MainSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<ZonesController>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<CoinsController>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ARFoundationController>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ARWorldCoordinator>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LocationController>().FromComponentInHierarchy().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MapController>().FromComponentInHierarchy().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<CamerasController>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ICameraView>().FromComponentsInHierarchy().AsCached();
            
            Container.BindInterfacesAndSelfTo<DropLocationDirectionPointer>().FromComponentsInHierarchy().AsCached();
            Container.BindInterfacesAndSelfTo<PointersController>().AsSingle().NonLazy();
        }
    }
}