using System.Collections.Generic;
using Prototype.Data;
using UnityEngine;

namespace Prototype.Screens.PortalsListScreen
{
    public class PortalsListScreenPresenter
    {
        private readonly IPortalsListScreenView _view;
        private readonly ProjectContext _context;
        private readonly List<PortalCardView> _cards = new();

        public PortalsListScreenPresenter(IPortalsListScreenView view, ProjectContext context)
        {
            _view = view;
            _context = context;
        }

        public void ShowScreen()
        {
            foreach (PortalCardView portalCardView in _cards)
            {
                portalCardView.DestroyCard();
            }
            
            _cards.Clear();

            foreach (PortalViewInfo portalViewInfo in _context.GetAllPortals())
            {
                PortalCardView portalCardView = Object.Instantiate(_view.GetCardPrefab(), _view.GetListContainer());
                portalCardView.transform.SetAsLastSibling();
                portalCardView.ConfigureView(portalViewInfo);
                
                portalCardView.MoveToClicked += OnMoveToClicked;
                
                _cards.Add(portalCardView);
            }

            _view.ShowScreen();
        }

        private void OnMoveToClicked(Vector2 coordinates)
        {
            OnlineMaps.instance.position = new Vector2(coordinates.y, coordinates.x);
        }
    }
}