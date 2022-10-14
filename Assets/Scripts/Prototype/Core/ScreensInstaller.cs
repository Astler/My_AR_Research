using Prototype.Screens.MainScreen;
using Prototype.Screens.PortalsListScreen;
using UnityEngine;

namespace Prototype.Core
{
    public class ScreensInstaller : MonoBehaviour
    {
        [SerializeField] private MainSceneView mainSceneView;
        [SerializeField] private PortalsListScreenView portalsListScreenView;

        private ProjectContext _context;
        
        public MainScreenPresenter MainScreenPresenter { get; private set; }
        public PortalsListScreenPresenter PortalsListPresenter { get; private set; }

        private void Awake()
        {
            _context = FindObjectOfType<ProjectContext>();
            
            MainScreenPresenter = new MainScreenPresenter(mainSceneView, _context);
            PortalsListPresenter = new PortalsListScreenPresenter(portalsListScreenView, _context);
        }
    }
}