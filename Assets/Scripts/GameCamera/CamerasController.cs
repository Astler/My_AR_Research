using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using UniRx;

namespace GameCamera
{
    public interface ICamerasController
    {
        ICameraView GetArCameraView();
    }

    public class CamerasController : IDisposable, ICamerasController
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly List<ICameraView> _camerasOnScene;
        private readonly IDataProxy _dataProxy;

        public CamerasController(List<ICameraView> camerasOnScene, IDataProxy dataProxy)
        {
            _camerasOnScene = camerasOnScene;
            _dataProxy = dataProxy;

            _dataProxy.ActiveCameraType.Subscribe(delegate(CameraType type)
            {
                foreach (ICameraView cameraView in camerasOnScene)
                {
                    cameraView.ActivateByCameraType(type);
                }
            }).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        public ICameraView GetArCameraView()
        {
            return _camerasOnScene.FirstOrDefault(it => it.CameraType == CameraType.ArCamera);
        }
    }
}