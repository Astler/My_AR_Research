// using System;
// using System.Collections;
// using JetBrains.Annotations;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Screens.Utils
// {
//     [RequireComponent(typeof(ScrollRect))]
//     public class Pagination : MonoBehaviour
//     {
//         [SerializeField] private Transform loader;
//         [SerializeField] [CanBeNull] private Transform topLoaderParent = null;
//         [SerializeField] [CanBeNull] private Transform bottomLoaderParent = null;
//
//         private int totalPages = 1;
//         private Action<int, int, Action> OnUploadPage;
//         private Action<int, Action> OnRefreshPage;
//         
//         private float scrollLimitToLoad = 150f;
//         private float maxLoaderHeight = 150f;
//         private int page = 1;
//         private int perPage = 10;
//         private bool is_loaded = false;
//         private int currentCount = 0;
//         private bool somethingLoading = true;
//         private bool isTryToRefresh;
//
//         private Loader _loader;
//         private LayoutElement loaderLayoutElement;
//         private Image loaderImage;
//         private CanvasGroup loaderCanvasGroup;
//         private ScrollRect scroll;
//         private IEnumerator _loadingCoroutine;
//
//         private void Awake()
//         {
//             scroll = GetComponent<ScrollRect>();
//             scroll.onValueChanged.AddListener(ScrollViewMoved);
//
//             Transform parent = transform.parent;
//             if (topLoaderParent == null) topLoaderParent = parent;
//             if (bottomLoaderParent == null) bottomLoaderParent = parent;
//             
//             loader = Instantiate(loader, bottomLoaderParent);
//             loader.SetAsLastSibling();
//             loader.gameObject.SetActive(false);
//             _loader = loader.GetComponent<Loader>();
//             loaderLayoutElement = loader.gameObject.GetComponent<LayoutElement>();
//             loaderImage = loader.transform.GetChild(0).GetComponent<Image>();
//             loaderCanvasGroup = loader.GetComponent<CanvasGroup>();
//             SetLoaderToDefaultState();
//         }
//
//         public void Initialize(int itemsPerPage, Action<int, Action> RefreshPageAction, Action<int, int, Action> UploadPageAction)
//         {
//             perPage = itemsPerPage;
//             OnRefreshPage = RefreshPageAction;
//             OnUploadPage = UploadPageAction;
//         }
//
//         private void ScrollViewMoved(Vector2 position)
//         {
//             if (somethingLoading) return;
//
//             float scrollPosition = scroll.content.anchoredPosition.y;
//             if (scrollPosition < 0f)
//             {
//                 if (!loader.gameObject.activeSelf)
//                 {
//                     SetLoaderToDefaultState();
//                     loader.SetParent(topLoaderParent);
//                     loader.SetAsFirstSibling();
//                     loader.gameObject.SetActive(true);
//                 }
//                 if (loaderLayoutElement.minHeight < maxLoaderHeight)
//                 {
//                     LoaderGrowing(scrollPosition);
//                 }
//             }
//             else if (scrollPosition > scroll.viewport.rect.y - scroll.content.rect.y)
//             {
//                 scrollPosition -= scroll.viewport.rect.y - scroll.content.rect.y;
//                 if (scrollPosition > 0f)
//                 {
//                     if (IsLastPage()) return;
//                     if (!loader.gameObject.activeSelf)
//                     {
//                         loader.SetParent(bottomLoaderParent);
//                         loader.SetAsLastSibling();
//                         SetLoaderToDefaultState();
//                         loader.gameObject.SetActive(true);
//                     }
//                     if (loaderLayoutElement.minHeight < maxLoaderHeight)
//                     {
//                         LoaderGrowing(scrollPosition);
//                     }
//                 }
//             }
//             else if (loader.gameObject.activeSelf)
//             {
//                 loader.gameObject.SetActive(false);
//                 SetLoaderToDefaultState();
//             }
//
//             if (loaderLayoutElement.minHeight >= maxLoaderHeight)
//             {
//                 if (is_loaded)
//                 {
//                     Debug.Log("Start loading");
//                     is_loaded = false;
//                     isTryToRefresh = scrollPosition < 0f;
//                     MainThreadDispatcher.instance.StopExternalCoroutine(_loadingCoroutine);
//                     _loadingCoroutine = RotateAndHidePreloader();
//                     MainThreadDispatcher.instance.StartExternalCoroutine(_loadingCoroutine);
//                 }
//
//                 Debug.Log("ScrollPosition: " + scrollPosition);
//
//                 if ((scrollPosition > -1f && scrollPosition < 1f)
//                     || (scrollPosition > 0f && scrollPosition < scroll.viewport.rect.y - scroll.content.rect.y + 1f))
//                 {
//                     SendRequest();
//                 }
//             }
//         }
//
//         private void SendRequest()
//         {
//             somethingLoading = true;
//
//             if (isTryToRefresh)
//             {
//                 Debug.Log("Refresh: " + scroll.content.anchoredPosition.y);
//                 page = 1;
//                 OnRefreshPage?.Invoke(perPage, AfterRequestCallback);
//             }
//             else
//             {
//                 Debug.Log("Upload: " + scroll.content.anchoredPosition.y);
//                 page = Mathf.Clamp(page+1,1,totalPages);
//                 OnUploadPage?.Invoke(page, perPage, AfterRequestCallback);
//             }
//         }
//
//         private void SetLoaderToDefaultState()
//         {
//             loaderImage.transform.rotation = Quaternion.Euler(Vector3.zero);
//             loaderImage.fillAmount = 0f;
//             loaderImage.fillClockwise = true;
//             loaderLayoutElement.minHeight = 0f;
//             loaderLayoutElement.minWidth = 0f;
//         }
//
//         private void LoaderGrowing(float scrollPosition)
//         {
//             float param = Mathf.Clamp(Mathf.Abs(scrollPosition), 0f, scrollLimitToLoad) / scrollLimitToLoad;
//             loaderLayoutElement.minHeight = param * maxLoaderHeight;
//             loaderLayoutElement.minWidth = param * maxLoaderHeight * 0.5f;
//             loaderImage.fillAmount = param;
//             loaderCanvasGroup.alpha = param;
//             if (param < 0.1f) loader.gameObject.SetActive(false);
//         }
//
//         private IEnumerator RotateAndHidePreloader()
//         {
//             float time = 0;
//             loaderImage.fillAmount = 1f;
//             loaderImage.fillClockwise = true;
//             while (!is_loaded)
//             {
//                 _loader.RotateLoader();
//                 yield return new WaitForFixedUpdate();
//                 time += Time.deltaTime;
//                 if (time > 5f && !somethingLoading)
//                 {
//                     SendRequest();
//                 }
//             }
//             time = 0;
//             float param;
//             while (time < 0.5f)
//             {
//                 _loader.RotateLoader();
//                 param = Mathf.Lerp(1f, 0f, time / 0.5f);
//                 loaderLayoutElement.minHeight = param * maxLoaderHeight;
//                 loaderLayoutElement.minWidth = param * maxLoaderHeight * 0.5f;
//                 loaderCanvasGroup.alpha = param;
//                 yield return new WaitForFixedUpdate();
//                 time += Time.deltaTime;
//             }
//             somethingLoading = false;
//             loader.gameObject.SetActive(false);
//             SetLoaderToDefaultState();
//         }
//
//         private void AfterRequestCallback()
//         {
//             is_loaded = true;
//         }
//
//         private bool IsLastPage()
//         {
//             return page == totalPages;
//         }
//
//         public void ResetScroll()
//         {
//             Debug.Log("Reset Scroll");
//             is_loaded = true;
//             somethingLoading = false;
//             page = 1;
//             if (scroll != null)
//                 scroll.verticalNormalizedPosition = 1;
//         }
//
//         public void SetTotalPage(int totalAmount)
//         {
//             totalPages = totalAmount > 0 ? totalAmount : 1;
//             Debug.Log("TotalPages: " + totalPages);
//         }
//
//         public void CalculateTotalPage(int total)
//         {
//             if (total > 0)
//             {
//                 float pageAmount = (float) total / perPage;
//                 totalPages = total / perPage;
//                 if ((pageAmount - totalPages) > 0) totalPages++;
//             }
//             else
//             {
//                 totalPages = 1;
//             }
//             Debug.Log("TotalPages: " + totalPages);
//         }
//     }
// }
