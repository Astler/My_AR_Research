// using Screens.Views;
// using UnityEngine;
// using ViewModels;
//
// namespace Screens.Presenters
// {
//     public class NewsSellPresenter : MonoBehaviour
//     {
//         [SerializeField] private NewsSellView newsSellView;
//
//         public void Awake()
//         {
//             newsSellView.OnDownButtonClick += Application.OpenURL;
//             newsSellView.OnUpButtonClick += Application.OpenURL;
//             
//             newsSellView.SetViewModel();
//         }
//     }
// }