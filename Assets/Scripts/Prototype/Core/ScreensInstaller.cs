using Prototype.Screens.MainScreen;
using UnityEngine;

namespace Prototype.Core
{
    public class ScreensInstaller : MonoBehaviour
    {
        [SerializeField] private MainSceneView mainSceneView;

        private ProjectContext _context;
        
        public MainScreenPresenter MainScreenPresenter { get; private set; }

        private void Awake()
        {
            _context = FindObjectOfType<ProjectContext>();
            
            MainScreenPresenter = new MainScreenPresenter(mainSceneView, _context);
        }
    }
}