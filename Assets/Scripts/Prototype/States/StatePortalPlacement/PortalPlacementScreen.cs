using Prototype.Screens;
using UniRx;

namespace Prototype.States.StatePortalPlacement
{
    public class PortalPlacementScreen : ScreenView
    {
        private ProjectContext _context;

        protected override void OnAwake()
        {
            _context = SceneFinder.TryGet<ProjectContext>();
        }

        protected override void OnStepStarted()
        {
            base.OnStepStarted();
        }
    }
}