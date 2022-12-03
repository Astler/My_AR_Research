#if UNITY_EDITOR
namespace ExternalTools.ImagesLoader
{
    public static class WebImagesLoaderEditorInstance
    {
        public static IWebImagesLoader ImagesLoader
        {
            get
            {
                WebImagesLoader editorLoader = new();
                editorLoader.Initialize();
                _imagesLoader ??= editorLoader;
                return _imagesLoader;
            }
        }

        private static IWebImagesLoader _imagesLoader;
    }
}
#endif