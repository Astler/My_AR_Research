using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace ExternalTools.ImagesLoader.Elements
{
    [ExecuteInEditMode]
    public class WebImage : Image
    {
        [SerializeField] private string url;
        private string _loadedUrl;

        private IWebImagesLoader _webImagesLoader;

        [Inject]
        private void Construct(IWebImagesLoader webImagesLoader)
        {
            _webImagesLoader = webImagesLoader;
        }

        public void SetImageUrl(string imageUrl)
        {
            url = imageUrl;
            LoadSpriteByUrl();
        }

        protected override void Start()
        {
            base.Start();
            LoadSpriteByUrl();
        }

        private void LoadSpriteByUrl()
        {
#if UNITY_EDITOR
            if (url.IsNullOrEmpty() || _loadedUrl == url) return;

            WebImagesLoaderEditorInstance.ImagesLoader.TryToLoadSprite(url, loadedSprite =>
            {
                _loadedUrl = sprite ? url : null;
                sprite = loadedSprite;
            });
            return;
#endif
            _webImagesLoader.TryToLoadSprite(url, loadedSprite => { sprite = loadedSprite; });
        }

        public string GetUrl() => url;
    }
}